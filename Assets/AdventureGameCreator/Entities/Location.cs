using System.Collections.Generic;
using AdventureGameCreator.Collections.Generic;

namespace AdventureGameCreator.Entities
{
    public class Location 
    {
        public int id;
        public string title;
        public string description;
        public bool isSearchable;
        public bool searched;

        public List<Connection> connections = new List<Connection>();
        public ObservableList<Item> items = new ObservableList<Item>();

        /// <summary>
        /// Sets non-visible items to visible
        /// </summary>
        public void Search()
        {
            searched = true;

            foreach (Item item in items)
            {
                if (!item.isVisible)
                {
                    item.isVisible = true;

                    items[items.IndexOf(item)] = item;      // TODO:    May change this if I add an ObservableEntity
                }
            }
        }
    }
}