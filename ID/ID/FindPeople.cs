
using System.Collections.Generic;
using System.Linq;

namespace ID
{
    // Class responsible for finding people by name
    internal class FindPeople
    {
        // Finds a person using LINQ
        public Person FindWithLinq(List<Person> people, string name)
        {
            // Returns the first person whose name matches
            // the specified name, or null if no match is found
            return people.FirstOrDefault(p => p.GetName() == name);
        }

        // Finds a person without using LINQ
        public Person FindWithoutLinq(List<Person> people, string name)
        {
            // Loop through every person in the list
            foreach (Person person in people)
            {
                // Check whether the person's name matches
                // the specified name
                if (person.GetName() == name)
                {
                    // Return the person if a match is found
                    return person;
                }
            }

            // Return null if no person with the specified
            // name exists in the list
            return null;
        }
    }
}
