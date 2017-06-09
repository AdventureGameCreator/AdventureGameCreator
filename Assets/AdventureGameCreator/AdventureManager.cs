using UnityEngine;
using UnityEngine.UI;
using AdventureGameCreator.Entities;
using AdventureGameCreator.UI;
using System;

namespace AdventureGameCreator
{
    // NOTE:    public fields removed but setters are required within properties in order to deserialize the xml data - I do not like!

    // NOTE:    Consider "actions" in more depth
    //          game actions = save / load / quit
    //          player actions = attack / drop item
    //          item actions = examine?  / use 
    //          actions may / may not be relevant in all conditions, for example, death / lose location - no point searching

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
        [SerializeField] private Text _locationTitle;
        [SerializeField] private Text _locationDescription;

        [SerializeField] private Text _inventory;

        // configuration
        private const string dataFilePath = "/StreamingAssets/XML/adventure_data.xml";
        private const int startLocation = 0;

        // private fields
        private Player _player = null;
        private Adventure _adventure = null;
        private Location _currentLocation = null;

        private InventoryDisplay _inventoryDisplay = null;

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
            UnsubscribeDelegates();
        }

        /// <summary>
        /// Subscribe our delegates
        /// </summary>
        private void SubscribeDelegates()
        {
            // connection options
            onKeyPress += Connection_Selected;

            // item options
            onKeyPress += Item_Selected;

            // action options
            onKeyPress += Action_Selected;
        }

        /// <summary>
        /// Unsubscribe our delegates
        /// </summary>
        private void UnsubscribeDelegates()
        {
            // connection options
            onKeyPress -= Connection_Selected;

            // item options
            onKeyPress -= Item_Selected;

            // action options
            onKeyPress -= Action_Selected;
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

            // set start location
            _currentLocation = _adventure.Locations[startLocation];

            // instantiate the inventory display
            _inventoryDisplay = new InventoryDisplay(_inventory, _player.Inventory);

            // hide inventory
            _inventoryDisplay.Disable();

            // wire up inventory and location item delegates
            _player.Inventory.Items.Updated += _inventoryDisplay.InventoryItems_Updated;
            _adventure.Locations.Changed += Location_Changed;       // TODO:    This won't work in the same way as location/items, may need an ObservableEntity
            _currentLocation.Items.Updated += LocationItems_Updated;
            _currentLocation.Items.Changed += LocationItems_Changed;

            Begin();
        }

        /// <summary>
        /// Handles the Changed method for the adventure's location collection
        /// </summary>
        private void Location_Changed(int obj)
        {
            DisplayCurrentLocation();
        }

        /// <summary>
        /// Handles the Updated method for the location's item collection
        /// </summary>
        private void LocationItems_Updated()
        {
            DisplayCurrentLocation();
        }

        /// <summary>
        /// Handles the Changed method for the location's item collection
        /// </summary>
        private void LocationItems_Changed(int index)
        {
            DisplayCurrentLocation();
        }

        /// <summary>
        /// Begins the adventure
        /// </summary>
        private void Begin()
        {
            DisplayCurrentLocation();
        }

        /// <summary>
        /// Displays the current location to the player
        /// </summary>
        private void DisplayCurrentLocation()
        {
            _locationTitle.text = _currentLocation.Title;
            _locationDescription.text = _currentLocation.Description;

            DisplayItems();
            DisplayConnectionOptions();
            DisplayActions();
        }

        /// <summary>
        /// Displays available actions
        /// </summary>
        private void DisplayActions()
        {
            // TODO:    This needs much more work
            //          Available actions should be based on target, e.g. location or item
            string actionOption;

            _locationDescription.text += "\n\n";

            if (_currentLocation.IsSearchable)
            {
                if (!_currentLocation.IsSearched)
                {
                    // actionOption = "[ " + actionOption.key + " ] " + actionOption.descriptor + "   ";
                    actionOption = "[S] Search   ";

                    _locationDescription.text += actionOption;
                }
            }

            // actionOption = "[ " + actionOption.key + " ] " + actionOption.descriptor + "   ";
            actionOption = "[I] Inventory   ";

            _locationDescription.text += actionOption;
        }

        /// <summary>
        /// Displays each connection option to the player
        /// </summary>
        private void DisplayConnectionOptions()
        {
            string connectionOption;

            _locationDescription.text += "\n\n";

            foreach (Connection connection in _currentLocation.Connections)
            {
                connectionOption = "[ " + connection.Key + " ] " + connection.Descriptor + "   ";

                _locationDescription.text += connectionOption;
            }
        }

        /// <summary>
        /// Displays each item option to the player
        /// </summary>
        private void DisplayItems()
        {
            string itemOption;

            _locationDescription.text += "\n\n";

            foreach (Item item in _currentLocation.Items)
            {
                if (item.IsVisible)
                {
                    itemOption = "[ " + item.Key + " ] " + item.Name + " ";

                    _locationDescription.text += itemOption;
                }
            }
        }

        /// <summary>
        /// Updates the player's current location
        /// </summary>
        /// <param name="locationID">The ID of the location</param>
        private void MoveToLocation(int locationID)
        {
            _currentLocation = _adventure.Locations[locationID];
            DisplayCurrentLocation();
        }

        /// <summary>
        /// Checks to see if a connection has been selected
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void Connection_Selected(string key)
        {
            foreach (Connection connection in _currentLocation.Connections)
            {
                if (connection.Key.ToUpper() == key.ToUpper())
                {
                    MoveToLocation(connection.ID);

                    break;
                }
            }
        }

        /// <summary>
        /// Checks to see if an item has been selected
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void Item_Selected(string key)
        {
            foreach (Item item in _currentLocation.Items)
            {
                if (item.IsVisible)
                {
                    if (item.Key.ToUpper() == key.ToUpper())
                    {
                        _player.Take(item);

                        _currentLocation.Items.Remove(item);

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if an action has been selected
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void Action_Selected(string key)
        {
            // TODO:    This needs much more work 
            //          Available actions should be based on target, e.g. location or item
            if (key.ToUpper() == "S")
            {
                if (_currentLocation.IsSearchable)
                {
                    if (!_currentLocation.IsSearched)
                    {
                        _player.Search(ref _currentLocation);
                        DisplayCurrentLocation();       // TODO:    May need an ObservableEntity instead of doing this
                    }
                }
            }

            if (key.ToUpper() == "I")
            {
                _inventoryDisplay.Toggle();
            }
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