using System;
using System.Collections.Generic;

public class Company
{
    // Stores all people who work in the company.
    private List<Person> people;

    // Stores the number of managers in the company.
    private int managerCounter;

    // Maximum number of people allowed in the company.
    private const int MAX_PEOPLE = 20;

    // Constructor
    public Company()
    {
        // Start with an empty list of people.
        people = new List<Person>();

        // At the beginning there are no managers.
        managerCounter = 0;
    }

    // Returns the list containing all people.
    public List<Person> GetPeople()
    {
        return people;
    }

    // Returns the total number of people in the company.
    public int GetEmployeeCount()
    {
        return people.Count;
    }

    // Returns the current number of managers.
    public int GetManagerCount()
    {
        return managerCounter;
    }

    
    // ADD PERSON
   

    // Adds a new person to the company.
    public bool AddPerson(Person person)
    {
        // Check if the maximum number of people was reached.
        if (people.Count >= MAX_PEOPLE)
        {
            Console.WriteLine(
                "The company cannot have more than 20 people."
            );

            return false;
        }

        // Check if another person already has the same ID.
        if (FindById(person.GetId()) != null)
        {
            Console.WriteLine(
                "A person with this ID already exists."
            );

            return false;
        }

        // Check if another person already has the same username.
        if (FindByUsername(person.GetUsername()) != null)
        {
            Console.WriteLine(
                "This username already exists."
            );

            return false;
        }

        // The company can have only one director.
        if (person is Director)
        {
            if (FindDirector() != null)
            {
                Console.WriteLine(
                    "There can only be one director."
                );

                return false;
            }
        }

        // An employee must have an existing manager.
        if (person is Employee employee)
        {
            // Search for the manager using the manager's name.
            Person manager =
                FindByName(employee.GetManagerName());

            // The person must exist and must be a Manager.
            if (manager == null ||
                !(manager is Manager))
            {
                Console.WriteLine(
                    "The selected manager does not exist."
                );

                return false;
            }
        }

        // All checks passed, so add the person to the list.
        people.Add(person);

        // If the new person is a manager,
        // increase the manager counter.
        if (person is Manager)
        {
            managerCounter++;
        }

        // Recalculate the number of employees
        // working under each manager.
        UpdateTeamSizes();

        // Update the manager counter stored by the director.
        UpdateDirectorCounter();

        return true;
    }

   
    // DELETE PERSON
  

    // Deletes a person using their ID.
    public bool DeletePerson(int id)
    {
        // Find the person with the given ID.
        Person person = FindById(id);

        // If the person does not exist, deletion fails.
        if (person == null)
        {
            Console.WriteLine(
                "Person not found."
            );

            return false;
        }

        // The director cannot be deleted.
        if (person is Director)
        {
            Console.WriteLine(
                "The director cannot be deleted."
            );

            return false;
        }

        // Check if the person is a manager.
        if (person is Manager manager)
        {
            // A manager cannot be deleted while
            // employees are still assigned to them.
            foreach (Person p in people)
            {
                if (p is Employee employee)
                {
                    // Compare manager names without considering
                    // upper/lower case.
                    if (employee.GetManagerName()
                        .Equals(
                            manager.GetName(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine(
                            "Cannot delete this manager."
                        );

                        Console.WriteLine(
                            "Employees are still assigned to this manager."
                        );

                        return false;
                    }
                }
            }

            // The manager can be deleted,
            // so decrease the manager counter.
            managerCounter--;
        }

        // Remove the person from the company.
        people.Remove(person);

        // Recalculate manager team sizes.
        UpdateTeamSizes();

        // Update the director's manager counter.
        UpdateDirectorCounter();

        return true;
    }

    
    // UPDATE PERSON
   

    // Updates the information of an existing person.
    public bool UpdatePerson(
        int id,
        string name,
        string username,
        string password,
        double salary,
        string managerName)
    {
        // Find the person using their ID.
        Person person = FindById(id);

        // If the person does not exist, update fails.
        if (person == null)
        {
            return false;
        }

        // Find whether the new username is already used.
        Person usernamePerson =
            FindByUsername(username);

        // The username can belong to the same person,
        // but not to another person.
        if (usernamePerson != null &&
            usernamePerson.GetId() != id)
        {
            Console.WriteLine(
                "This username is already used."
            );

            return false;
        }

        // If the person is an employee,
        // validate the new manager.
        if (person is Employee employee)
        {
            Person manager =
                FindByName(managerName);

            // The manager must exist and must be a Manager.
            if (manager == null ||
                !(manager is Manager))
            {
                Console.WriteLine(
                    "Manager does not exist."
                );

                return false;
            }

            // Update the employee's manager.
            employee.SetManagerName(managerName);
        }

        // Update the common Person information.
        person.SetName(name);
        person.SetUsername(username);
        person.SetPassword(password);
        person.SetSalary(salary);

        // Recalculate team sizes in case
        // an employee changed manager.
        UpdateTeamSizes();

        return true;
    }

    
    // FIND BY ID
   

    // Searches for a person using their ID.
    public Person FindById(int id)
    {
        foreach (Person person in people)
        {
            if (person.GetId() == id)
            {
                return person;
            }
        }

        // Return null if no matching person was found.
        return null;
    }

    
    // FIND BY USERNAME
   

    // Searches for a person using their username.
    public Person FindByUsername(string username)
    {
        foreach (Person person in people)
        {
            // Username comparison ignores upper/lower case.
            if (person.GetUsername()
                .Equals(
                    username,
                    StringComparison.OrdinalIgnoreCase))
            {
                return person;
            }
        }

        return null;
    }

    
    // FIND BY NAME
    

    // Searches for a person using their name.
    public Person FindByName(string name)
    {
        foreach (Person person in people)
        {
            // Name comparison ignores upper/lower case.
            if (person.GetName()
                .Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase))
            {
                return person;
            }
        }

        return null;
    }

    
    // FIND DIRECTOR
   

    // Searches for the director of the company.
    public Director FindDirector()
    {
        foreach (Person person in people)
        {
            // Check whether the person is a Director.
            if (person is Director director)
            {
                return director;
            }
        }

        // No director was found.
        return null;
    }

    // TOTAL SALARY
   

    // Calculates the total salary paid by the company.
    public double GetTotalSalary()
    {
        double total = 0;

        foreach (Person person in people)
        {
            // Add each person's salary to the total.
            total += person.GetSalary();
        }

        return total;
    }

    
    // UPDATE MANAGER TEAM SIZES
    

    // Recalculates how many employees work
    // under each manager.
    public void UpdateTeamSizes()
    {
        // First reset every manager's team size to 0.
        foreach (Person person in people)
        {
            if (person is Manager manager)
            {
                manager.SetTeamSize(0);
            }
        }

        // Go through all employees.
        foreach (Person person in people)
        {
            if (person is Employee employee)
            {
                // Compare the employee's manager
                // with every manager in the company.
                foreach (Person managerPerson in people)
                {
                    if (managerPerson is Manager manager)
                    {
                        // If the employee belongs to this manager,
                        // increase the manager's team size.
                        if (employee.GetManagerName()
                            .Equals(
                                manager.GetName(),
                                StringComparison.OrdinalIgnoreCase))
                        {
                            manager.SetTeamSize(
                                manager.GetTeamSize() + 1
                            );
                        }
                    }
                }
            }
        }
    }

    
    // UPDATE DIRECTOR MANAGER COUNTER
    

    // Updates the number of managers stored
    // inside the Director object.
    public void UpdateDirectorCounter()
    {
        // Find the director.
        Director director = FindDirector();

        // If a director exists, update their counter.
        if (director != null)
        {
            director.SetManagerCounter(
                managerCounter
            );
        }
    }

   
    // CHECK MANAGER COUNTER
   
    // Checks whether managerCounter matches
    // the actual number of Manager objects.
    public bool ValidateManagerCounter()
    {
        int realManagerCount = 0;

        // Count all Manager objects in the company.
        foreach (Person person in people)
        {
            if (person is Manager)
            {
                realManagerCount++;
            }
        }

        // Return true if both values are equal.
        return realManagerCount == managerCounter;
    }

   
    // DISPLAY COMPANY
   

    // Displays general company information
    // and information about every person.
    public void DisplayCompanyInformation()
    {
        Console.WriteLine();
        Console.WriteLine(
            "========== COMPANY INFORMATION =========="
        );

        // Display the number of people.
        Console.WriteLine(
            "Total people: " + people.Count + " / 20"
        );

        // Display the number of managers.
        Console.WriteLine(
            "Total managers: " + managerCounter
        );

        // Display the total salary.
        Console.WriteLine(
            "Total salary: " +
            GetTotalSalary().ToString("F2")
        );

        Console.WriteLine();

        // Display information about every person.
        foreach (Person person in people)
        {
            person.DisplayInfo();
            Console.WriteLine();
        }

        Console.WriteLine(
            "=========================================="
        );
    }
}