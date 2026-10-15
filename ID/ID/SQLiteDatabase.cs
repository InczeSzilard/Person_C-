
using System;
using System.Data.SQLite;
using System.IO;
using ID;

public class SQLiteDatabase
{
    // Stores the connection string used to connect to company.db.
    private readonly string connectionString;

    // CONSTRUCTOR
    // Receives the path of the existing SQLite database.
    public SQLiteDatabase(string databasePath)
    {
        // Convert the database path to an absolute path.
        string fullPath = Path.GetFullPath(databasePath);

        // Check that the database file already exists.
        // This prevents accidentally using a missing database file.
        if (!File.Exists(fullPath))
            throw new FileNotFoundException(
                "Database not found.", fullPath);

        // Create the connection string for SQLite version 3.
        connectionString =
            $"Data Source={fullPath};Version=3;";
    }

    // OPEN CONNECTION
    // Opens a connection to the SQLite database.
    private SQLiteConnection OpenConnection()
    {
        var connection = new SQLiteConnection(connectionString);

        // Establish the connection.
        connection.Open();

        return connection;
    }

    // READ PEOPLE
    // Displays every column of every record in the People table.
    // This method is useful for debugging the database contents.
    public void ReadPeople()
    {
        // Automatically dispose of database resources when finished.
        using (var connection = OpenConnection())
        using (var command = new SQLiteCommand(
            "SELECT * FROM People", connection))
        using (var reader = command.ExecuteReader())
        {
            // Read each database record.
            while (reader.Read())
            {
                // Loop through all columns in the current record.
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Display NULL when the database value is NULL.
                    // Otherwise, convert the value to a string.
                    string value = reader.IsDBNull(i)
                        ? "NULL"
                        : Convert.ToString(reader.GetValue(i)) ?? "";

                    // Display the column name and its value.
                    Console.Write(
                        $"{reader.GetName(i)}: {value}  ");
                }

                Console.WriteLine();
            }
        }
    }

    // LOAD PEOPLE INTO COMPANY
    // Reads database records and creates the appropriate C# objects.
    public void LoadPeopleIntoCompany(Company company)
    {
        using (var connection = OpenConnection())

        // Load managers first, then the director, and finally employees.
        // Employees require their managers to exist before they are added.
        using (var command = new SQLiteCommand(@"
            SELECT id, name, username, password,
                   role, salary, manager_name
            FROM People
            ORDER BY
                CASE LOWER(TRIM(role))
                    WHEN '2' THEN 1
                    WHEN 'manager' THEN 1
                    WHEN '3' THEN 2
                    WHEN 'director' THEN 2
                    WHEN '1' THEN 3
                    WHEN 'employee' THEN 3
                    ELSE 4
                END,
                id;", connection))
        using (var reader = command.ExecuteReader())
        {
            // Process each record returned by SQLite.
            while (reader.Read())
            {
                // Read the person's ID.
                int id = Convert.ToInt32(reader["id"]);

                // Read the person's common information.
                string name = ReadString(reader, "name");
                string username = ReadString(reader, "username");
                string password = ReadString(reader, "password");

                // Read the person's role.
                string role = ReadString(reader, "role");

                // Read the person's salary.
                double salary = ReadDouble(reader, "salary");

                // Read the assigned manager's name, if applicable.
                string managerName =
                    ReadString(reader, "manager_name");

                // This variable can reference any Person subclass.
                Person person;

                // Determine which object to create based on the role.
                // Both numeric and descriptive role values are supported.
                switch (role.Trim().ToLowerInvariant())
                {
                    case "1":
                    case "employee":

                        // Create an employee with their manager.
                        person = new Employee(
                            id, name, username, password,
                            "Employee", salary, managerName);
                        break;

                    case "2":
                    case "manager":

                        // Create a manager.
                        person = new Manager(
                            id, name, username, password,
                            "Manager", salary);
                        break;

                    case "3":
                    case "director":

                        // Get the single Director instance.
                        person = Director.GetInstance(
                            id, name, username, password,
                            "Director", salary);
                        break;

                    default:

                        // Skip records with an unsupported role.
                        Console.WriteLine(
                            $"Unknown role '{role}' for {name} (ID {id}).");
                        continue;
                }

                // Add the object to the Company collection.
                // Company.AddPerson validates IDs, usernames,
                // the director limit, managers and company capacity.
                if (!company.AddPerson(person))
                {
                    Console.WriteLine(
                        $"Could not load {name} (ID {id}). " +
                        "Check duplicate IDs/usernames, manager assignments, " +
                        "or the company capacity.");
                }
            }
        }

        // Display the number of people successfully loaded.
        Console.WriteLine(
            $"Loaded {company.GetEmployeeCount()} people from SQLite.");
    }

    // ADD PERSON
    // Inserts a new person into the People table.
    public bool AddPerson(Person person)
    {
        using (var connection = OpenConnection())
        using (var command = new SQLiteCommand(@"
            INSERT INTO People
                (id, name, username, password, role,
                 salary, manager_name)
            VALUES
                (@id, @name, @username, @password, @role,
                 @salary, @manager)", connection))
        {
            // Assign values to the SQL parameters.
            // Parameters prevent user input from being treated as SQL.
            AddParameters(command, person);

            // Execute the INSERT statement.
            // Return true if a record was inserted.
            return command.ExecuteNonQuery() > 0;
        }
    }

    // UPDATE PERSON
    // Updates an existing person's information using their ID.
    public bool UpdatePerson(
        int id,
        string name,
        string username,
        string password,
        double salary,
        string managerName)
    {
        using (var connection = OpenConnection())
        using (var command = new SQLiteCommand(@"
            UPDATE People
            SET name = @name,
                username = @username,
                password = @password,
                salary = @salary,
                manager_name = @manager
            WHERE id = @id", connection))
        {
            // Set the ID used to locate the record.
            command.Parameters.AddWithValue("@id", id);

            // Set the updated personal information.
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@salary", salary);

            // Store the new manager name, or an empty string.
            // For employees, this should be a valid manager.
            command.Parameters.AddWithValue(
                "@manager", managerName ?? "");

            // Execute the UPDATE statement.
            // Returns true when a matching record is updated.
            return command.ExecuteNonQuery() > 0;
        }
    }

    // DELETE PERSON
    // Deletes a record from the People table using its ID.
    public bool DeletePerson(int id)
    {
        using (var connection = OpenConnection())
        using (var command = new SQLiteCommand(
            "DELETE FROM People WHERE id = @id", connection))
        {
            // Specify which person should be deleted.
            command.Parameters.AddWithValue("@id", id);

            // Execute the DELETE statement.
            // Return true if a record was deleted.
            return command.ExecuteNonQuery() > 0;
        }
    }

    // ADD PARAMETERS
    // Copies a Person object's information into SQL parameters.
    // Used when inserting a new person into the database.
    private static void AddParameters(
        SQLiteCommand command, Person person)
    {
        command.Parameters.AddWithValue(
            "@id", person.GetId());

        command.Parameters.AddWithValue(
            "@name", person.GetName());

        command.Parameters.AddWithValue(
            "@username", person.GetUsername());

        command.Parameters.AddWithValue(
            "@password", person.GetPassword());

        // Store the role returned by the Person object.
        command.Parameters.AddWithValue(
            "@role", person.GetRole());

        command.Parameters.AddWithValue(
            "@salary", person.GetSalary());

        // Only employees have a manager name.
        string managerName = "";

        if (person is Employee employee)
        {
            managerName = employee.GetManagerName();
        }

        // Store an empty value for people without a manager.
        command.Parameters.AddWithValue(
            "@manager", managerName ?? "");
    }

    // READ STRING
    // Safely reads a database column as a string.
    // Returns an empty string if the column contains SQL NULL.
    private static string ReadString(
        SQLiteDataReader reader, string column)
    {
        return reader[column] == DBNull.Value
            ? ""
            : Convert.ToString(reader[column]) ?? "";
    }

    // READ DOUBLE
    // Safely reads a database column as a double.
    // Returns zero if the column contains SQL NULL.
    private static double ReadDouble(
        SQLiteDataReader reader, string column)
    {
        return reader[column] == DBNull.Value
            ? 0
            : Convert.ToDouble(reader[column]);
    }
}
