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

        private static List<string> _validationKeys = new List<string>();

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
            Type[] extraTypes = new Type[] { typeof(Weapon), typeof(Equipment) };

            Adventure adventure = XMLDataService.Instance.Load<Adventure>(dataFilePath, extraTypes);

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
                _validationKeys.Clear();

                ValidateConnectionData(location);
                ValidateItemData(location);
            }
        }

        /// <summary>
        /// Validates connection data
        /// </summary>
        /// <param name="location">The location containing the connections</param>
        private static void ValidateConnectionData(Location location)
        {
            foreach (Connection connection in location.Connections)
            {
                DuplicateKeyValidation(location.Title, connection.Key.ToUpper());
                ReservedKeyValidation(location.Title, connection.Key.ToUpper());
            }
        }

        /// <summary>
        /// Validates item data
        /// </summary>
        /// <param name="location">The location containing the items</param>
        private static void ValidateItemData(Location location)
        {
            foreach (Item item in location.Items)
            {
                DuplicateKeyValidation(location.Title, item.Key.ToUpper());
                ReservedKeyValidation(location.Title, item.Key.ToUpper());
            }
        }

        /// <summary>
        /// Checks for duplicate key usage
        /// </summary>
        /// <param name="location">The name of the location</param>
        /// <param name="key">The key to validate</param>
        private static void DuplicateKeyValidation(string location, string key)
        {
            if (!_validationKeys.Contains(key))
            {
                _validationKeys.Add(key);
            }
            else
            {
                throw new DuplicateKeyException("Key '" + key + "' already exists for Location '" + location + "'");
            }
        }

        /// <summary>
        /// Checks for reserved key usage
        /// </summary>
        /// <param name="location">The name of the location</param>
        /// <param name="key">The key to validate</param>
        private static void ReservedKeyValidation(string location, string key)
        {
            if (AdventureManager.ReservedKeys.Contains(key))
            {
                throw new ReservedKeyException("Key '" + key + "' is used by Location '" + location + "'");
            }
        }
    }
}