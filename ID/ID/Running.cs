using System;
using System.Collections.Generic;

namespace ID
{
    // Main class responsible for running the console application
    internal static class Running
    {
        // Company object containing all employees, managers and director
        private static Company company = new Company();

        // Database object used to load and save information
        private static Database database;
        // Caesar encryption object used for passwords
        private static ICaesar caesar = new Caesar();
        // Main method - program starts here

        public static void Main(string[] args)
        {
            try
            {
                // Create the database object and locate Database.txt
                database = new Database("Database.txt");

                // Load all people from the database file
                database.LoadCompany(company);

                bool running = true;

                // Main menu loop
                while (running)
                {
                    Console.Clear();

                    Console.WriteLine("1. Login");
                    Console.WriteLine("2. Company information");
                    Console.WriteLine("3. Total salary");
                    Console.WriteLine("4. Manager counter");
                    Console.WriteLine("5. Add person");
                    Console.WriteLine("6. Update person");
                    Console.WriteLine("7. Delete person");
                    Console.WriteLine("8. Show managers");
                    Console.WriteLine("9. Show employees");
                    Console.WriteLine("10. Save & Exit");

                    Console.Write("Choose: ");
                    string choice = Console.ReadLine();

                    // Decide which operation to perform
                    // based on the user's menu choice
                    switch (choice)
                    {
                        case "1":
                            Login();
                            break;

                        case "2":
                            CompanyInformation();
                            break;

                        case "3":
                            TotalSalary();
                            break;

                        case "4":
                            ManagerCounter();
                            break;

                        case "5":
                            AddPerson();
                            break;

                        case "6":
                            UpdatePerson();
                            break;

                        case "7":
                            DeletePerson();
                            break;

                        case "8":
                            ShowManagers();
                            break;

                        case "9":
                            ShowEmployees();
                            break;

                        case "10":
                            // Save all changes before closing
                            database.SaveCompany(company);
                            running = false;
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            Pause();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Display the complete error if something goes wrong
                Console.WriteLine();
                Console.WriteLine("===== ERROR =====");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("=================");
                Console.ReadLine();
            }
        }

        
        // LOGIN
        

        // Allows a person to log into the system
        private static void Login()
        {
            Console.WriteLine("============== LOGIN ==============");

            Console.Write("Username: ");
            string username = Console.ReadLine() ?? "";

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? "";

            // Find the person using the entered username
            Person person = company.FindByUsername(username);

            if (person == null)
            {
                Console.WriteLine("Username not found.");
                Pause();
                return;
            }

            // Encode the entered password using Caesar encryption
            string encodedPassword = caesar.Encode(password);

            // Compare the encoded password with the stored password
            if (encodedPassword != person.GetPassword())
            {
                Console.WriteLine("Incorrect password.");
                Pause();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Login successful!");
            Console.WriteLine();

            // Display basic information
            Console.WriteLine("Name: " + person.GetName());
            Console.WriteLine("Username: " + person.GetUsername());
            Console.WriteLine("Role: " + person.GetRole());
            Console.WriteLine("Salary: " +
                person.GetSalary().ToString("F2"));

            // Employees have a manager
            if (person is Employee employee)
            {
                Console.WriteLine(
                    "Manager: " + employee.GetManagerName()
                );
            }

            // Managers have a team size
            if (person is Manager manager)
            {
                Console.WriteLine(
                    "Team size: " + manager.GetTeamSize()
                );
            }

            // Directors have the number of managers
            if (person is Director director)
            {
                Console.WriteLine(
                    "Number of managers: " +
                    director.GetManagerCounter()
                );
            }

            Pause();
        }

        
        // COMPANY INFORMATION
        

        // Displays information about the whole company
        private static void CompanyInformation()
        {
            company.DisplayCompanyInformation();
            Pause();
        }

        
        // TOTAL SALARY
        

        // Calculates and displays the total salary
        private static void TotalSalary()
        {
            Console.WriteLine(
                "Total salary paid by company: " +
                company.GetTotalSalary().ToString("F2")
            );

            Pause();
        }

        
        // MANAGER COUNTER
       

        // Displays the number of managers
        // and checks whether the counter is correct
        private static void ManagerCounter()
        {
            Console.WriteLine(
                "Manager counter: " +
                company.GetManagerCount()
            );

            // Compare the stored counter with the
            // actual number of managers
            if (company.ValidateManagerCounter())
            {
                Console.WriteLine(
                    "Manager counter is correct."
                );
            }
            else
            {
                Console.WriteLine(
                    "ERROR: Manager counter is incorrect."
                );
            }

            Pause();
        }

        
        // ADD PERSON
       

        // Adds a new employee, manager or director
        private static void AddPerson()
        {
            Console.WriteLine(
                "============== ADD PERSON =============="
            );

            // The company can contain a maximum of 20 people
            if (company.GetEmployeeCount() >= 20)
            {
                Console.WriteLine(
                    "The company already has 20 people."
                );

                Pause();
                return;
            }

            // Read and validate the ID
            Console.Write("ID: ");

            if (!int.TryParse(
                    Console.ReadLine(),
                    out int id))
            {
                Console.WriteLine("Invalid ID.");
                Pause();
                return;
            }

            // Check whether the ID is already used
            if (company.FindById(id) != null)
            {
                Console.WriteLine(
                    "ID already exists."
                );

                Pause();
                return;
            }

            Console.Write("Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Username: ");
            string username = Console.ReadLine() ?? "";

            // Usernames must be unique
            if (company.FindByUsername(username) != null)
            {
                Console.WriteLine(
                    "Username already exists."
                );

                Pause();
                return;
            }

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? "";

            Console.Write("Salary: ");

            // Validate salary
            if (!double.TryParse(
                    Console.ReadLine(),
                    out double salary))
            {
                Console.WriteLine("Invalid salary.");
                Pause();
                return;
            }

            // Ask the user which type of person to create
            Console.WriteLine();
            Console.WriteLine("1. Employee");
            Console.WriteLine("2. Manager");
            Console.WriteLine("3. Director");
            Console.Write("Choose role: ");

            string role = Console.ReadLine() ?? "";

            // Passwords are encoded before being stored
            string encodedPassword =
                caesar.Encode(password);

            Person person;

            
            // EMPLOYEE
            

            if (role == "1")
            {
                Console.WriteLine();
                Console.WriteLine("Available managers:");

                List<Person> people = company.GetPeople();

                bool managerExists = false;

                // Display all available managers
                foreach (Person p in people)
                {
                    if (p is Manager manager)
                    {
                        managerExists = true;

                        Console.WriteLine(
                            manager.GetName()
                        );
                    }
                }

                // An employee must have a manager
                if (!managerExists)
                {
                    Console.WriteLine(
                        "There are no managers."
                    );

                    Console.WriteLine(
                        "Create a manager before creating an employee."
                    );

                    Pause();
                    return;
                }

                Console.Write(
                    "Manager name: "
                );

                string managerName =
                    Console.ReadLine() ?? "";

                // Find the selected manager
                Person selectedManager =
                    company.FindByName(managerName);

                if (selectedManager == null ||
                    !(selectedManager is Manager))
                {
                    Console.WriteLine(
                        "Manager not found."
                    );

                    Pause();
                    return;
                }

                // Create the employee object
                person = new Employee(
                    id,
                    name,
                    username,
                    encodedPassword,
                    salary,
                    managerName
                );
            }

            
            // MANAGER
            

            else if (role == "2")
            {
                // Create a manager object
                person = new Manager(
                    id,
                    name,
                    username,
                    encodedPassword,
                    salary
                );
            }

           
            // DIRECTOR
           

            else if (role == "3")
            {
                // Only one director is allowed
                if (company.FindDirector() != null)
                {
                    Console.WriteLine(
                        "There can only be one director."
                    );

                    Pause();
                    return;
                }

                // Create the director using the Singleton method
                person = Director.GetInstance(
                    id,
                    name,
                    username,
                    encodedPassword,
                    salary
                );
            }

            else
            {
                Console.WriteLine(
                    "Invalid role."
                );

                Pause();
                return;
            }

           
            // ADD TO COMPANY
            

            // Add the newly created person to the company
            if (company.AddPerson(person))
            {
                // Save the updated company to the database
                database.SaveCompany(company);

                Console.WriteLine(
                    "Person added successfully."
                );
            }
            else
            {
                Console.WriteLine(
                    "Person could not be added."
                );
            }

            Pause();
        }

        
        // UPDATE PERSON
        

        // Updates an existing person's information
        private static void UpdatePerson()
        {
            Console.WriteLine(
                "============== UPDATE PERSON =============="
            );

            Console.Write("ID: ");

            // Validate ID
            if (!int.TryParse(
                    Console.ReadLine(),
                    out int id))
            {
                Console.WriteLine("Invalid ID.");
                Pause();
                return;
            }

            // Find the person
            Person person = company.FindById(id);

            if (person == null)
            {
                Console.WriteLine(
                    "Person not found."
                );

                Pause();
                return;
            }

            Console.WriteLine(
                "Current name: " +
                person.GetName()
            );

            Console.Write("New name: ");
            string name = Console.ReadLine() ?? "";

            Console.WriteLine(
                "Current username: " +
                person.GetUsername()
            );

            Console.Write("New username: ");
            string username = Console.ReadLine() ?? "";

            Console.Write(
                "New password: "
            );

            string password =
                Console.ReadLine() ?? "";

            Console.Write(
                "New salary: "
            );

            // Validate new salary
            if (!double.TryParse(
                    Console.ReadLine(),
                    out double salary))
            {
                Console.WriteLine(
                    "Invalid salary."
                );

                Pause();
                return;
            }

            string managerName = "";

            // Only employees need a manager
            if (person is Employee)
            {
                Console.Write(
                    "New manager name: "
                );

                managerName =
                    Console.ReadLine() ?? "";
            }

            // Encode the new password
            string encodedPassword =
                caesar.Encode(password);
            // Update the person in the company
            bool success =
                company.UpdatePerson(
                    id,
                    name,
                    username,
                    encodedPassword,
                    salary,
                    managerName
                );

            if (success)
            {
                // Save changes to the database
                database.SaveCompany(company);

                Console.WriteLine(
                    "Person updated successfully."
                );
            }
            else
            {
                Console.WriteLine(
                    "Person could not be updated."
                );
            }

            Pause();
        }

        
        // DELETE PERSON
        

        // Deletes a person using their ID
        private static void DeletePerson()
        {
            Console.WriteLine(
                "============== DELETE PERSON =============="
            );

            Console.Write("ID: ");

            // Validate ID
            if (!int.TryParse(
                    Console.ReadLine(),
                    out int id))
            {
                Console.WriteLine(
                    "Invalid ID."
                );

                Pause();
                return;
            }

            // Delete the person from the company
            if (company.DeletePerson(id))
            {
                // Save the changes
                database.SaveCompany(company);

                Console.WriteLine(
                    "Person deleted successfully."
                );
            }
            else
            {
                Console.WriteLine(
                    "Person could not be deleted."
                );
            }

            Pause();
        }

  
        // SHOW MANAGERS
      

        // Displays all managers
        private static void ShowManagers()
        {
            Console.WriteLine(
                "============== MANAGERS =============="
            );

            foreach (Person person in company.GetPeople())
            {
                if (person is Manager manager)
                {
                    Console.WriteLine();

                    Console.WriteLine(
                        "ID: " + manager.GetId()
                    );

                    Console.WriteLine(
                        "Name: " + manager.GetName()
                    );

                    Console.WriteLine(
                        "Username: " +
                        manager.GetUsername()
                    );

                    Console.WriteLine(
                        "Salary: " +
                        manager.GetSalary().ToString("F2")
                    );

                    Console.WriteLine(
                        "Team size: " +
                        manager.GetTeamSize()
                    );
                }
            }

            Pause();
        }

        
        // SHOW EMPLOYEES
       

        // Displays all employees
        private static void ShowEmployees()
        {
            Console.WriteLine(
                "============== EMPLOYEES =============="
            );

            foreach (Person person in company.GetPeople())
            {
                if (person is Employee employee)
                {
                    Console.WriteLine();

                    Console.WriteLine(
                        "ID: " + employee.GetId()
                    );

                    Console.WriteLine(
                        "Name: " + employee.GetName()
                    );

                    Console.WriteLine(
                        "Username: " +
                        employee.GetUsername()
                    );

                    Console.WriteLine(
                        "Manager: " +
                        employee.GetManagerName()
                    );

                    Console.WriteLine(
                        "Salary: " +
                        employee.GetSalary().ToString("F2")
                    );
                }
            }

            Pause();
        }

      
        // PAUSE
        // Waits for the user to press Enter
        // before returning to the menu
        private static void Pause()
        {
            Console.WriteLine();
            Console.Write(
                "Press Enter to continue..."
            );

            Console.ReadLine();
        }
    }
}