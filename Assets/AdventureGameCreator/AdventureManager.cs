using UnityEngine;
using UnityEngine.UI;
using AdventureGameCreator.Entities;
using AdventureGameCreator.Collections.Generic;
using AdventureGameCreator.UI;
using System;
using System.Collections.Generic;

namespace AdventureGameCreator
{
    // NOTE:    Could be useful as a method for finding specific list items
    //          Item selectedItem = list.Find(delegate (Item item) { return item.IsSelected == true; });

    // NOTE:    public fields removed but setters are required within properties in order to deserialize the xml data - I do not like!

    // NOTE:    Consider "actions" in more depth
    //          game actions = save / load / quit
    //          player actions = attack
    //          item actions = examine?  / use 
    //          actions may / may not be relevant in all conditions, for example, death / lose location - no point searching / inventory

    // NOTE:    Consider refactoring player input so that delegates are handled in the same way as
    //          the ObservableList delegates

    // NOTE:    Consider calculating option key based on connection descriptor / item name at run time?
    //          e.g first letter of string, second if first is taken etc
    //          Need to have some reserved characters for actions, such as [E]xamine, [L]ook etc

    // NOTE:    Could add a [RequiredComponent] of type AdventureDataService
    //          this would be an interface, allowing different types of the
    //          data service to be used, XML, database, www etc
    //          Would mean exposing the underyling data service though


    // TODO:    Refactor
    public class AdventureManager : MonoBehaviour
    {
        // UI components for display
        [SerializeField] private Text _locationTitle;
        [SerializeField] private Text _locationDescription;

        [SerializeField] private Text _inventory;

        // configuration
        private const string dataFilePath = "/StreamingAssets/XML/adventure_data.xml";
        private const int startLocation = 0;

        private static List<String> _reservedKeys = new List<string> { "S", "I" };  // NOTE: Must be uppercase

        // enums
        private enum ActionState { AtLocation, LocationItemSelected, ExaminingLocationItem, ViewingInventory, InventoryItemSelected, ExaminingInventoryItem };

        // private fields
        private Player _player = null;
        private Adventure _adventure = null;
        private Location _currentLocation = null;

        private InventoryDisplay _inventoryDisplay = null;

        private ActionState _actionState = 0;

        private bool _optionSelected = false;
        
        // delegate for managing keyboard input
        private delegate void OnKeyPress(string key);
        private OnKeyPress onKeyPress;


        /// <summary>
        /// Returns a list containing the reserved keys
        /// </summary>
        public static List<String> ReservedKeys
        {
            get { return _reservedKeys; }
        }


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
            // option selected
            onKeyPress += Option_Selected;
        }

        /// <summary>
        /// Unsubscribe our delegates
        /// </summary>
        private void UnsubscribeDelegates()
        {
            // option selected
            onKeyPress -= Option_Selected;
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
            _actionState = ActionState.AtLocation;
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
            string actionOption = string.Empty;

            switch (_actionState)
            {
                case ActionState.AtLocation:

                    _locationDescription.text += "\n\n";

                    if (_currentLocation.IsSearchable)
                    {
                        if (!_currentLocation.IsSearched)
                        {
                            // actionOption = "[ " + actionOption.key + " ] " + actionOption.descriptor + "   ";
                            actionOption = "[S]earch   ";

                            _locationDescription.text += actionOption;
                        }
                    }

                    // actionOption = "[ " + actionOption.key + " ] " + actionOption.descriptor + "   ";
                    actionOption = "[I]nventory   ";

                    _locationDescription.text += actionOption;

                    break;

                case ActionState.LocationItemSelected:

                    _locationDescription.text += "\n\n";

                    // actionOption = "[ " + actionOption.key + " ] " + actionOption.descriptor + "   ";
                    actionOption = "[T]ake, [E]xamine, [U]se item, [C]ancel";

                    _locationDescription.text += actionOption;

                    break;

                case ActionState.ExaminingLocationItem:

                    // TODO:    I don't like the duplicated options to handle the [E]xamine option being selected

                    _locationDescription.text += "\n\n";

                    // actionOption = "[ " + actionOption.key + " ] " + actionOption.descriptor + "   ";
                    actionOption = "[T]ake, [U]se item, [C]ancel";

                    _locationDescription.text += actionOption;

                    break;

                case ActionState.ViewingInventory:

                    _locationDescription.text += "\n\n";

                    // actionOption = "[ " + actionOption.key + " ] " + actionOption.descriptor + "   ";
                    actionOption = "[I]nventory   ";

                    _locationDescription.text += actionOption;

                    break;

                case ActionState.InventoryItemSelected:

                    _locationDescription.text += "\n\n";

                    // actionOption = "[ " + actionOption.key + " ] " + actionOption.descriptor + "   ";
                    actionOption = "[D]rop, [E]xamine, [U]se item, [C]ancel";

                    _locationDescription.text += actionOption;

                    break;

                case ActionState.ExaminingInventoryItem:

                    // TODO:    I don't like the duplicated options to handle the [E]xamine option being selected

                    _locationDescription.text += "\n\n";

                    // actionOption = "[ " + actionOption.key + " ] " + actionOption.descriptor + "   ";
                    actionOption = "[D]rop, [U]se item, [C]ancel";

                    _locationDescription.text += actionOption;

                    break;
            }
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
            // TODO:    Refactor
            _currentLocation.Items.Updated -= LocationItems_Updated;
            _currentLocation.Items.Changed -= LocationItems_Changed;

            _currentLocation = _adventure.Locations[locationID];

            _currentLocation.Items.Updated += LocationItems_Updated;
            _currentLocation.Items.Changed += LocationItems_Changed;

            DisplayCurrentLocation();
        }

        /// <summary>
        /// Handles adventure option selection
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void Option_Selected(string key)
        {
            key = key.ToUpper();

            // connection options
            CheckConnectionOptions(key);

            // item options
            CheckItemOptions(key);

            // inventory item options
            CheckInventoryItemOptions(key);

            // action options
            CheckActionOptions(key);
        }

        /// <summary>
        /// Checks to see if a connection has been selected
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void CheckConnectionOptions(string key)
        {
            if (!_optionSelected)
            {
                if (_actionState == ActionState.AtLocation)
                {
                    foreach (Connection connection in _currentLocation.Connections)
                    {
                        if (connection.Key.ToUpper() == key)
                        {
                            _optionSelected = true;

                            MoveToLocation(connection.ID);

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if an item has been selected
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void CheckItemOptions(string key)
        {
            // TODO:    Refactor this, too long
            if (!_optionSelected)
            {
                if (_actionState == ActionState.AtLocation)
                {
                    foreach (Item item in _currentLocation.Items)
                    {
                        if (item.IsVisible)
                        {
                            if (item.Key.ToUpper() == key)
                            {
                                _optionSelected = true;

                                _actionState = ActionState.LocationItemSelected;

                                _player.SelectItem(item);

                                DisplayCurrentLocation();

                                break;
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Checks to see if an inventory item has been selected
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void CheckInventoryItemOptions(string key)
        {
            if (!_optionSelected)
            {
                if (_actionState == ActionState.ViewingInventory)
                {
                    foreach (Item item in _player.Inventory.Items)
                    {
                        if (item.Key.ToUpper() == key)
                        {
                            _optionSelected = true;

                            _actionState = ActionState.InventoryItemSelected;

                            _player.SelectItem(item);

                            DisplayCurrentLocation();

                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Checks to see if an action has been selected
        /// </summary>
        /// <param name="key">The key which was pressed</param>
        private void CheckActionOptions(string key)
        {
            if (!_optionSelected)
            {
                switch (_actionState)
                {
                    case ActionState.AtLocation:

                        switch (key)
                        {
                            case "S":

                                Search();
                                break;

                            case "I":

                                ToggleInventory(ActionState.ViewingInventory);
                                break;
                        }
                        break;

                    case ActionState.LocationItemSelected:

                        switch(key)
                        {
                            case "T":
                                TakeItem();
                                break;

                            case "E":
                                ExamineItem(ActionState.ExaminingLocationItem);
                                break;

                            case "U":
                                UseItem();
                                break;

                            case "C":
                                CancelItemSelection(ActionState.AtLocation);
                                break;
                        }
                        break;

                    case ActionState.ExaminingLocationItem:

                        switch (key)
                        {
                            case "T":
                                TakeItem();
                                break;

                            case "U":
                                UseItem();
                                break;

                            case "C":
                                CancelItemSelection(ActionState.AtLocation);
                                break;
                        }

                        break;

                    case ActionState.ViewingInventory:

                        switch (key)
                        {
                            case "I":
                                ToggleInventory(ActionState.AtLocation);
                                break;
                        }
                        break;

                    case ActionState.InventoryItemSelected:

                        switch (key)
                        {
                            case "D":
                                DropItem();
                                break;

                            case "E":
                                ExamineItem(ActionState.ExaminingInventoryItem);
                                break;

                            case "U":
                                UseItem();
                                break;

                            case "C":
                                CancelItemSelection(ActionState.ViewingInventory);
                                break;
                        }

                        break;

                    case ActionState.ExaminingInventoryItem:

                        switch (key)
                        {
                            case "D":
                                DropItem();
                                break;

                            case "U":
                                UseItem();
                                break;

                            case "C":
                                CancelItemSelection(ActionState.ViewingInventory);
                                break;
                        }

                        break;
                }
            }
        }

        private void Search()
        {
            _optionSelected = true;

            if (_currentLocation.IsSearchable)
            {
                if (!_currentLocation.IsSearched)
                {
                    _player.Search(ref _currentLocation);
                    DisplayCurrentLocation();
                }
            }
        }

        private void ToggleInventory(ActionState actionState)
        {
            _optionSelected = true;

            _actionState = actionState;

            _inventoryDisplay.Toggle();
        }


        private void TakeItem()
        {
            _optionSelected = true;

            _actionState = ActionState.AtLocation;

            _player.Take(_player.SelectedItem);

            _currentLocation.Items.Remove(_player.SelectedItem);

            _player.UnselectItem();
        }


        private void DropItem()
        {
            _optionSelected = true;

            _actionState = ActionState.ViewingInventory;

            _player.Drop(_player.SelectedItem);

            _currentLocation.Items.Add(_player.SelectedItem);

            _player.UnselectItem();
        }


        private void UseItem()
        {
            _optionSelected = true;
            throw new NotImplementedException("Using items has not yet been implemented.");
        }

        /// <summary>
        /// Cancel the selection of an item
        /// </summary>
        /// <param name="actionState">The ActionState to return to</param>
        private void CancelItemSelection(ActionState actionState)
        {
            _optionSelected = true;

            _player.UnselectItem();

            _actionState = actionState;

            DisplayCurrentLocation();
        }

        private void ExamineItem(ActionState actionState)
        {
            _optionSelected = true;

            _actionState = actionState;

            DisplayCurrentLocation();

            _locationDescription.text += "\n\n" + _player.SelectedItem.Detail;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        private void Update()
        {
            CheckForPlayerInput();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                if (Input.GetKeyDown(e.keyCode))
                {
                    //  Debug.Log("Down: " + e.keyCode);
                }
            }
            else if (e.type == EventType.keyUp)
            {
                if (Input.GetKeyUp(e.keyCode))
                {
                    _optionSelected = false;       // NOTE:    Resets option selected bool
                }
            }
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

                    onKeyPress(input);
                }
            }
        }
    }
}