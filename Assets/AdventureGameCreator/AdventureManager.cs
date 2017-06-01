using UnityEngine;
using UnityEngine.UI;
using AdventureGameCreator.Entities;

namespace AdventureGameCreator
{

    // NOTE:    Consider refactoring player input so that delegates are handled in the same way as
    //          the ObservableList delegates

    // NOTE:    Consider calculating option key based on connection descriptor / item name at run time?
    //          e.g first letter of string, second if first is taken etc
    //          Need to have some reserved characters for actions, such as [E]xamine, [L]ook etc


    // NOTE:    Could add a [RequiredComponent] of type AdventureDataService
    //          this would be an interface, allowing different types of the
    //          data service to be used, XML, database, www etc
    //          Would mean exposing the underyling data service though

    public class AdventureManager : MonoBehaviour
    {
        // UI components for display
        public Text _location;
        public Text _story;

        public Text _inventory;

        // configuration
        private const string dataFilePath = "/StreamingAssets/XML/adventure_data.xml";
        private const int startLocation = 0;

        // private fields
        private Player _player = null;
        private Adventure _adventure = null;
        private Location _currentLocation = null;

        // delegate for managing keyboard input
        private delegate void OnKeyPress(string key);
        private OnKeyPress onKeyPress;

        /// <summary>
        /// Called when the object becomes enabled and active
        /// </summary>
        private void OnEnable()
        {
            SubscribeDelegates();
        }

        /// <summary>
        /// Called when the object becomes disabled and inactive
        /// </summary>
        private void OnDisable()
        {
            UnSubscribeDelegates();
        }

        /// <summary>
        /// Subscribe our delegates
        /// </summary>
        private void SubscribeDelegates()
        {
            // connection options
            onKeyPress += OnConnectionSelected;

            // item options
            onKeyPress += OnItemSelected;
        }

        /// <summary>
        /// UnSubscribe our delegates
        /// </summary>
        private void UnSubscribeDelegates()
        {
            // connection options
            onKeyPress -= OnConnectionSelected;

            // item options
            onKeyPress -= OnItemSelected;
        }

        /// <summary>
        /// Use this for initialisation
        /// </summary>
        private void Start()
        {
            // create new player
            _player = new Player();

            // wire up inventory and location item delegates
            _player.inventory.items.Updated += InventoryItems_Updated;

            // load adventure data
            _adventure = Adventure.Load(Application.dataPath + dataFilePath);

            Begin();


            _currentLocation.items.Updated += LocationItems_Updated;
        }

        /// <summary>
        /// Handles the Updated method for the inventory's item collection
        /// </summary>
        private void InventoryItems_Updated()
        {
            DisplayInventoryItems();
        }

        /// <summary>
        /// Handles the Updated method for the location's item collection
        /// </summary>
        private void LocationItems_Updated()
        {
            DisplayCurrentLocation();
        }

        /// <summary>
        /// Begins the adventure
        /// </summary>
        private void Begin()
        {
            MoveToLocation(startLocation);
            DisplayInventoryItems();
        }

        /// <summary>
        /// Displays the current location to the player
        /// </summary>
        private void DisplayCurrentLocation()
        {
            _location.text = _currentLocation.title;
            _story.text = _currentLocation.description;

            DisplayConnectionOptions();
            DisplayItems();
        }
        
        /// <summary>
        /// Displays each connection option to the player
        /// </summary>
        private void DisplayConnectionOptions()
        {
            string connectionOption;

            _story.text += "\n\n";

            foreach (Connection connection in _currentLocation.connections)
            {
                connectionOption = "[ " + connection.key + " ] " + connection.descriptor + "   ";

                _story.text += connectionOption;
            }
        }

        /// <summary>
        /// Displays each item option to the player
        /// </summary>
        private void DisplayItems()
        {
            string itemOption;

            _story.text += "\n\n";

            foreach (Item item in _currentLocation.items)
            {
                itemOption = "[ " + item.key + " ] " + item.name + " ";

                _story.text += itemOption;
            }
        }

        /// <summary>
        /// Displays each item in the player's inventory
        /// </summary>
        private void DisplayInventoryItems()
        {
            _inventory.text = string.Empty;

            foreach (Item item in _player.inventory.items)
            {
                _inventory.text += item.name + "\n";
            }
        }


        /// <summary>
        /// Updates the player's current location
        /// </summary>
        /// <param name="locationID">The ID of the location</param>
        private void MoveToLocation(int locationID)
        {
            _currentLocation = _adventure.locations[locationID];
            DisplayCurrentLocation();
        }

        /// <summary>
        /// Checks to see if a connection has been selected
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void OnConnectionSelected(string key)
        {
            foreach (Connection connection in _currentLocation.connections)
            {
                if (connection.key.ToUpper() == key.ToUpper())
                {
                    MoveToLocation(connection.id);

                    break;
                }
            }
        }

        /// <summary>
        /// Checks to see if an item has been selected
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void OnItemSelected(string key)
        {
            foreach (Item item in _currentLocation.items)
            {
                if (item.key.ToUpper() == key.ToUpper())
                {
                    TakeItem(item);

                    break;
                }
            }
        }

        /// <summary>
        /// Adds the specified item to the player's inventory and removes it from the location
        /// </summary>
        /// <param name="item">The item to take</param>
        private void TakeItem(Item item)
        {
            _player.inventory.items.Add(item);
            _currentLocation.items.Remove(item);
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        private void Update()
        {
            CheckForPlayerInput();
        }

        /// <summary>
        /// Checks for keyboard interaction
        /// </summary>
        private void CheckForPlayerInput()
        {
            if (Input.anyKeyDown)
            {
                string input = Input.inputString;

                if (input.Length > 0)
                {
                    input = input.Substring(0, 1);

                    // NOTE:    Refactor as it will be need for other actions for each location
                    onKeyPress(input);
                }
            }
        }
    }
}