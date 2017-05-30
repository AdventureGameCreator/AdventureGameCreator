using System;
using System.Collections.Generic;
using AdventureGameCreator.Data.DataServices;
using AdventureGameCreator.Exceptions;

namespace AdventureGameCreator.Entities
{
    public class Adventure
    {
        public List<Location> locations = new List<Location>();

        // TOOD: Complete Documentation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFilePath"></param>
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
        /// 
        /// </summary>
        private static void ValidateAdventureData(Adventure adventure)
        {
            foreach (Location location in adventure.locations)
            {
                List<string> keys = new List<string>();

                foreach (Connection connection in location.connections)
                {
                    if (!keys.Contains(connection.key.ToUpper()))
                    {
                        keys.Add(connection.key.ToUpper());
                    }
                    else
                    {
                        throw new DuplicateConnectionKeyFoundException("Key '" + connection.key.ToUpper() + "' already exists for Location : '" + location.title + "'");
                    }
                }
            }
        }
    }
}