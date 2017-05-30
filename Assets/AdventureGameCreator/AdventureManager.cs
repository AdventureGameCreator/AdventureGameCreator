using UnityEngine;
using UnityEngine.UI;
using AdventureGameCreator.Entities;

namespace AdventureGameCreator
{
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

            // load adventure data
            _adventure = Adventure.Load(Application.dataPath + dataFilePath);

            Begin();
        }
        
        /// <summary>
        /// Begins the adventure
        /// </summary>
        private void Begin()
        {
            UpdateLocation(startLocation);
            DisplayInventoryItems();    // NOTE: Not sure if this is a sensible place for this, may move
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
        /// 
        /// </summary>
        private void DisplayItems()
        {
            // TODO: Change this
            _story.text += "\n\n";

            foreach (Item item in _currentLocation.items)
            {
                _story.text += "[ " + item.key + " ] " + item.name + " ";
            }
        }

        /// <summary>
        /// Displays each item in the player's inventory
        /// </summary>
        private void DisplayInventoryItems()
        {
            foreach (Item item in _player.inventory.items)
            {
                _inventory.text += item.name + "\n";
            }
        }

        /// <summary>
        /// Updates the current player location
        /// </summary>
        /// <param name="locationID">The ID of the location</param>
        private void UpdateLocation(int locationID)
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
            // NOTE: Consider - calculating key based on all of the connections text at run time?
            //                  e.g first letter of string, second if first is taken etc

            foreach (Connection connection in _currentLocation.connections)
            {
                if (connection.key.ToUpper() == key.ToUpper())
                {
                    UpdateLocation(connection.id);
                }
            }
        }


        /// <summary>
        /// Checks to see if an item has been selected
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void OnItemSelected(string key)
        {
            // NOTE: Consider - calculating key based on all of the connections text at run time?
            //                  e.g first letter of string, second if first is taken etc
            //                  For actions this could be the first letter of the verb 
            //                  For items this could be the first letter of the noun

            foreach (Item item in _currentLocation.items)
            {
                if (item.key.ToUpper() == key.ToUpper())
                {
                    // TODO: Refactor
                    // TODO: This needs expanding on, for now we will just take the item
                    _player.inventory.items.Add(item);

                    // remove item from location (do this first)
                    _currentLocation.items.Remove(item);

                    // update the current location
                    UpdateLocation(_currentLocation.id);        // TODO: Need to do this at the moment as the items are concatenated to the story text

                    // TODO: Need to check ObservableCollections<T>, can raise an event from those
                    //       For now, call a method to update
                    UpdateInventory();

                    // exit the loop
                    break;
                }
            }
        }

        /// <summary>
        /// Refreshes the content of the inventory
        /// </summary>
        private void UpdateInventory()
        {
            _inventory.text = string.Empty;

            DisplayInventoryItems();
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