using System;
using System.Collections.Generic;
using AdventureGameCreator.Data.DataServices;
using AdventureGameCreator.Exceptions;

namespace AdventureGameCreator.Entities
{
    public class Adventure
    {
        public List<Location> locations = new List<Location>();

        /// <summary>
        /// Loads and validates the adventure data for the specified file path
        /// </summary>
        /// <param name="dataFilePath">The file path for the adventure data</param>
        /// <returns></returns>
        public static Adventure Load(string dataFilePath)
        {


            // Adventure adventure = XMLDataService.Instance.Load<Adventure>(dataFilePath);


            Type[] extraTypes = new Type[] { typeof(Weapon), typeof(Equipment) };

            Adventure adventure = XMLDataService.Instance.Load<Adventure>(dataFilePath, extraTypes);

            // validate adventure data
            ValidateAdventureData(adventure);

            return adventure;
        }

        /// <summary>
        /// Validates the adventure data
        /// </summary>
        private static void ValidateAdventureData(Adventure adventure)
        {
            foreach (Location location in adventure.locations)
            {
                List<string> keys = new List<string>();

                // TODO: Refactor to remove duplication and better handle exceptions
                foreach (Connection connection in location.connections)
                {
                    if (!keys.Contains(connection.key.ToUpper()))
                    {
                        keys.Add(connection.key.ToUpper());
                    }
                    else
                    {
                        throw new DuplicateConnectionKeyFoundException("Connection Key '" + connection.key.ToUpper() + "' already exists for Location : '" + location.title + "'");
                    }
                }

                foreach(Item item in location.items)
                {
                    if (!keys.Contains(item.key.ToUpper()))
                    {
                        keys.Add(item.key.ToUpper());
                    }
                    else
                    {
                        throw new DuplicateConnectionKeyFoundException("Item Key '" + item.key.ToUpper() + "' already exists for Location : '" + location.title + "'");
                    }
                }

            }
        }
    }
}