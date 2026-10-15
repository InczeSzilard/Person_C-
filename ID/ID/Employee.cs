using System;

public class Employee : Person
{
    // Stores the name of the manager responsible for this employee.
    private string managerName;

    // Creates a new Employee.
    public Employee(
        int id,
        string name,
        string username,
        string password,
        string role,
        double salary,
        string managerName)
        : base(id, name, username, password,role, salary)
    {
        this.managerName = managerName;
    }

    // Returns the employee's manager name.
    public string GetManagerName()
    {
        return managerName;
    }

    // Changes the employee's manager.
    public void SetManagerName(string managerName)
    {
        this.managerName = managerName;
    }

   

    // Displays the employee's information.
    public override void DisplayInfo()
    {
        base.DisplayInfo();

        Console.WriteLine(
            "   Manager: " + managerName
        );
    }
}