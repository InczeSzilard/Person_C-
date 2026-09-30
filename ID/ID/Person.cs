using System;

public abstract class Person
{
    // Common information shared by all people in the company.
    private int id;
    private string name;
    private string username;
    private string password;
    private double salary;

    // Creates a Person with the basic information.
    public Person(
        int id,
        string name,
        string username,
        string password,
        double salary)
    {
        this.id = id;
        this.name = name;
        this.username = username;
        this.password = password;
        this.salary = salary;
    }

    // Returns the person's ID.
    public int GetId()
    {
        return id;
    }

    // Returns the person's name.
    public string GetName()
    {
        return name;
    }

    // Returns the person's username.
    public string GetUsername()
    {
        return username;
    }

    // Returns the stored password.
    public string GetPassword()
    {
        return password;
    }

    // Returns the person's salary.
    public double GetSalary()
    {
        return salary;
    }

    // Changes the person's name.
    public void SetName(string name)
    {
        this.name = name;
    }

    // Changes the person's username.
    public void SetUsername(string username)
    {
        this.username = username;
    }

    // Changes the person's password.
    public void SetPassword(string password)
    {
        this.password = password;
    }

    // Changes the person's salary.
    public void SetSalary(double salary)
    {
        this.salary = salary;
    }

    // Each child class must provide its own role.
    public abstract string GetRole();

    // Displays the common information of a person.
    // Child classes can override this method to display
    // additional information.
    public virtual void DisplayInfo()
    {
        Console.WriteLine(
            "ID: " + id +
            " | Name: " + name +
            " | Username: " + username +
            " | Salary: " + salary +
            " | Role: " + GetRole()
        );
    }
}