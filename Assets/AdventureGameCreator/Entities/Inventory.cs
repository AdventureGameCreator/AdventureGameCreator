using AdventureGameCreator.Collections.Generic;

namespace AdventureGameCreator.Entities
{
    public class Inventory
    {
        private ObservableList<Item> _items;


        /// <summary>
        /// Default constructor
        /// </summary>
        public Inventory()
        {
            _items = new ObservableList<Item>();
        }


        /// <summary>
        /// Returns the inventory items
        /// </summary>
        public ObservableList<Item> Items
        {
            get { return _items; }
        }
    }
}