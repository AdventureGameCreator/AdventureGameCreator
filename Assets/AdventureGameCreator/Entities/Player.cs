namespace AdventureGameCreator.Entities
{
    public class Player
    {
        private Inventory _inventory;

        private Item _selectedItem;


        /// <summary>
        /// Default constructor
        /// </summary>
        public Player()
        {
            _inventory = new Inventory();
        }


        /// <summary>
        /// Returns the player's inventory
        /// </summary>
        public Inventory Inventory
        {
            get { return _inventory; }
        }

        /// <summary>
        /// Returns the player's currently selected item
        /// </summary>
        /// <remarks>The item does </remarks>
        public Item SelectedItem
        {
            get { return _selectedItem; }
        }

        /// <summary>
        /// Adds the specified item to the inventory
        /// </summary>
        /// <param name="item">The item to take</param>
        public void Take(Item item)
        {
            _inventory.Items.Add(item);
        }

        /// <summary>
        /// Removes the specified item from the inventory
        /// </summary>
        /// <param name="item">The item to drop</param>
        public void Drop(Item item)
        {
            _inventory.Items.Remove(item);
        }

        /// <summary>
        /// Performs a search action at the location
        /// </summary>
        /// <param name="location">The location to search</param>
        public void Search(ref Location location)
        {
            location.Search();
        }


        public void SelectItem(Item item)
        {
            _selectedItem = item;
        }

        public void UnselectItem()
        {
            _selectedItem = null;
        }

    }
}