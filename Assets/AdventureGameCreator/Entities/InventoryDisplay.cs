using UnityEngine.UI;
using AdventureGameCreator.Entities;

namespace AdventureGameCreator.UI
{
    public class InventoryDisplay
    {
        private Text _display;
        private Inventory _inventory;


        /// <summary>
        /// default constructor
        /// </summary>
        private InventoryDisplay() { }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="display">The inventory Text game object</param>
        /// <param name="inventory">The inventory</param>
        public InventoryDisplay(Text display, Inventory inventory)
        {
            _display = display;
            _inventory = inventory;
        }


        /// <summary>
        /// Toggles the display of the inventory
        /// </summary>
        public void Toggle()
        {
            if (_display.gameObject.activeInHierarchy)
            {
                Disable();
            }
            else
            {
                Enable();
            }
        }

        /// <summary>
        /// Enables the display of the inventory and items
        /// </summary>
        private void Enable()
        {
            _display.gameObject.SetActive(true);
            DisplayInventoryItems();
        }

        /// <summary>
        /// Clears all items from the display
        /// </summary>
        private void Clear()
        {
            _display.text = string.Empty;
        }

        /// <summary>
        /// Disables the display of inventory and items
        /// </summary>
        public void Disable()
        {
            Clear();
            _display.gameObject.SetActive(false);
        }

        /// <summary>
        /// Handles the Updated method for the inventory's item collection
        /// </summary>
        public void InventoryItems_Updated()
        {
            DisplayInventoryItems();
        }

        /// <summary>
        /// Displays each item in the player's inventory
        /// </summary>
        private void DisplayInventoryItems()
        {
            Clear();

            foreach (Item item in _inventory.Items)
            {
                _display.text += item.Name + "\n";
            }
        }
    }
}
