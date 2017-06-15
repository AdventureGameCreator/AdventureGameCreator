using System;
using System.Collections.Generic;
using AdventureGameCreator.Data.DataServices;
using AdventureGameCreator.Exceptions;
using AdventureGameCreator.Collections.Generic;

namespace AdventureGameCreator.Entities
{
    public class Adventure
    {
        private ObservableList<Location> _locations;


        /// <summary>
        /// Default constructor
        /// </summary>
        public Adventure()
        {
            _locations = new ObservableList<Location>();
        }


        /// <summary>
        /// Returns the locations
        /// </summary>
        public ObservableList<Location> Locations
        {
            get { return _locations; }
            set { _locations = value; }
        }


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
            foreach (Location location in adventure.Locations)
            {
                List<string> keys = new List<string>();

                // TODO:    Refactor to remove duplication and better handle exceptions
                foreach (Connection connection in location.Connections)
                {
                    if (!keys.Contains(connection.Key.ToUpper()))
                    {
                        keys.Add(connection.Key.ToUpper());
                    }
                    else
                    {
                        throw new DuplicateKeyException("Connection Key '" + connection.Key.ToUpper() + "' already exists for Location : '" + location.Title + "'");
                    }

                    // NOTE:    Hard coded and duplicated, for now.
                    if (connection.Key.ToUpper() == "S")
                    {
                        throw new ReservedKeyException("Connection ID : '" + connection.ID + "' uses a reserved key for Location : '" + location.Title + "'");
                    }

                    if (connection.Key.ToUpper() == "I")
                    {
                        throw new ReservedKeyException("Connection ID : '" + connection.ID + "' uses a reserved key for Location : '" + location.Title + "'");
                    }
                }

                foreach (Item item in location.Items)
                {
                    if (!keys.Contains(item.Key.ToUpper()))
                    {
                        keys.Add(item.Key.ToUpper());
                    }
                    else
                    {
                        throw new DuplicateKeyException("Item Key '" + item.Key.ToUpper() + "' already exists for Location : '" + location.Title + "'");
                    }

                    // NOTE:    Hard coded and duplicated, for now.
                    if (item.Key.ToUpper() == "S")
                    {
                        throw new ReservedKeyException("Item : '" + item.Name + "' uses a reserved key for Location : '" + location.Title + "'");
                    }
                    
                    if (item.Key.ToUpper() == "I")
                    {
                        throw new ReservedKeyException("Item : '" + item.Name + "' uses a reserved key for Location : '" + location.Title + "'");
                    }
                }

            }
        }
    }
}