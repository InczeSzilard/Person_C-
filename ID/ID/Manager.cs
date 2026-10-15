using System;
using System.Security.Principal;

public class Manager : Person
{
    // Stores the number of employees assigned to this manager.
    private int teamSize;

    // Creates a new Manager.
    public Manager(
        int id,
        string name,
        string username,
        string password,
        string role,
        double salary)
        : base(id, name, username, password,role, salary)
    {
        // A new manager starts with no employees.
        teamSize = 0;
    }

    // Returns the number of employees managed by this manager.
    public int GetTeamSize()
    {
        return teamSize;
    }

    // Updates the number of employees in the manager's team.
    public void SetTeamSize(int teamSize)
    {
        this.teamSize = teamSize;
    }

  

    // Displays the manager's information.
    public override void DisplayInfo()
    {
        base.DisplayInfo();

        Console.WriteLine(
            "   Team size: " + teamSize
        );
    }
}