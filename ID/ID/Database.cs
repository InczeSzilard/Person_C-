using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class Database
{
    // Stores the full path to the database file.
    private string fileName;

    // Creates a Database object and finds Database.txt
    // in the application's output directory.
    public Database(string fileName)
    {
        this.fileName = Path.Combine(
            AppContext.BaseDirectory,
            fileName
        );

        Console.WriteLine("Database path:");
        Console.WriteLine(this.fileName);

        // Stop the program if the database file does not exist.
        if (!File.Exists(this.fileName))
        {
            throw new FileNotFoundException(
                "Database file was not found.",
                this.fileName
            );
        }
    }

    
    // LOAD COMPANY
    

    // Reads all people from Database.txt
    // and adds them to the Company object.
    public void LoadCompany(Company company)
    {
        string[] lines = File.ReadAllLines(fileName);

        
        // FIRST PASS
        // Load managers and director first.
        

        foreach (string line in lines)
        {
            // Ignore empty lines.
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Separate the fields using '|'.
            string[] data = line.Split('|');

            // A manager/director needs at least 6 fields.
            if (data.Length < 6)
                continue;

            int id = int.Parse(data[0]);
            string name = data[1];
            string username = data[2];
            string password = data[3];

            // Use invariant culture so salary values such as
            // 5000.50 are read consistently.
            double salary = double.Parse(
                data[4],
                CultureInfo.InvariantCulture
            );

            string role = data[5];

            // Create managers first.
            if (role == "Manager")
            {
                Manager manager = new Manager(
                    id,
                    name,
                    username,
                    password,
                    role,
                    salary
                );

                company.AddPerson(manager);
            }

            // Create the director.
            else if (role == "Director")
            {
                Director director = Director.GetInstance(
                    id,
                    name,
                    username,
                    password,
                    role,
                    salary
                );

                company.AddPerson(director);
            }
        }

        
        // SECOND PASS
        // Load employees after managers exist.
     

        foreach (string line in lines)
        {
            // Ignore empty lines.
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] data = line.Split('|');

            // An employee needs 7 fields because
            // the seventh field contains the manager name.
            if (data.Length < 7)
                continue;

            int id = int.Parse(data[0]);
            string name = data[1];
            string username = data[2];
            string password = data[3];

            double salary = double.Parse(
                data[4],
                CultureInfo.InvariantCulture
            );

            string role = data[5];
            string managerName = data[6];

            // Create the employee and assign its manager.
            if (role == "Employee")
            {
                Employee employee = new Employee(
                    id,
                    name,
                    username,
                    password,
                    role,
                    salary,
                    managerName
                );

                company.AddPerson(employee);
            }
        }

        // Recalculate manager team sizes.
        company.UpdateTeamSizes();

        // Update the director's manager counter.
        company.UpdateDirectorCounter();
    }

    
    // SAVE COMPANY
   

    // Saves all company information to Database.txt.
    public void SaveCompany(Company company)
    {
        List<string> lines = new List<string>();

        // Convert every person into one database line.
        foreach (Person person in company.GetPeople())
        {
            string line =
                person.GetId() + "|" +
                person.GetName() + "|" +
                person.GetUsername() + "|" +
                person.GetPassword() + "|" +
                person.GetRole() + "|" +
                person.GetSalary().ToString(
                    CultureInfo.InvariantCulture
                );
               

            // Employees also store their manager's name.
            if (person is Employee employee)
            {
                line += "|" + employee.GetManagerName();
            }
            else
            {
                // Managers and directors do not need a manager.
                line += "|";
            }

            lines.Add(line);
        }

        // Write all records back to Database.txt.
        File.WriteAllLines(fileName, lines);
    }
}