namespace AdventureGameCreator.Entities
{
    public class Player
    {
        private Inventory _inventory;

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
        /// Adds the specified item to the inventory
        /// </summary>
        /// <param name="item">The item to take</param>
        public void Take(Item item)
        {
            _inventory.items.Add(item);
        }

        /// <summary>
        /// Removes the specified item from the inventory
        /// </summary>
        /// <param name="item">The item to drop</param>
        public void Drop(Item item)
        {
            _inventory.items.Remove(item);
        }

        /// <summary>
        /// Performs a search action at the location
        /// </summary>
        /// <param name="location">The location to search</param>
        public void Search(ref Location location)
        {
            location.Search();
        }
    }
}