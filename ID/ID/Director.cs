using System;

public class Director : Person
{
    // Stores the single Director object.
    // This implements the Singleton pattern.
    private static Director instance = null;

    // Stores the number of managers in the company.
    private int managerCounter;

    // Private constructor prevents other classes
    // from creating Directors directly.
    private Director(
        int id,
        string name,
        string username,
        string password,
        string role,
        double salary)
        : base(id, name, username, password,role, salary)
    {
        managerCounter = 0;
    }

    // Creates the Director if one does not already exist.
    // If one already exists, the existing Director is returned.
    public static Director GetInstance(
        int id,
        string name,
        string username,
        string password,
        string role,
        double salary)
    {
        if (instance == null)
        {
            instance = new Director(
                id,
                name,
                username,
                password,
                role,
                salary
            );
        }

        return instance;
    }

    // Returns the existing Director instance.
    public static Director GetInstance()
    {
        return instance;
    }

    // Returns the current number of managers.
    public int GetManagerCounter()
    {
        return managerCounter;
    }

    // Updates the number of managers.
    public void SetManagerCounter(int managerCounter)
    {
        this.managerCounter = managerCounter;
    }

 

    // Displays the Director's information.
    public override void DisplayInfo()
    {
        base.DisplayInfo();

        Console.WriteLine(
            "   Number of managers: " +
            managerCounter
        );
    }
}