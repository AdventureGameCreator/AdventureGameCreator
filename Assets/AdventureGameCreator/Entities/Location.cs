using System.Collections.Generic;
using AdventureGameCreator.Collections.Generic;
using System.Xml.Serialization;

namespace AdventureGameCreator.Entities
{
    public class Location
    {
        private int _id;
        private string _title;
        private string _description;
        private bool _isSearchable;
        private bool _isSearched;

        private List<Connection> _connections = new List<Connection>();
        private ObservableList<Item> _items = new ObservableList<Item>();


        /// <summary>
        /// Returns the location's ID
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Returns the location's title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Returns the location's description
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Indicates whether the location is searchable
        /// </summary>
        public bool IsSearchable
        {
            get {  return _isSearchable; }
            set { _isSearchable = value; }
        }

        /// <summary>
        /// Indicates whether the location has been searched
        /// </summary>
        public bool IsSearched
        {
            get { return _isSearched; }
        }

        /// <summary>
        /// Returns the location's connections
        /// </summary>
        public List<Connection> Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }

        /// <summary>
        /// Returns the location's items
        /// </summary>
        public ObservableList<Item> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        
        /// <summary>
        /// Sets non-visible items to visible
        /// </summary>
        public void Search()
        {
            _isSearched = true;

            foreach (Item item in Items)
            {
                if (!item.IsVisible)
                {
                    item.IsVisible = true;

                    Items[Items.IndexOf(item)] = item;      // TODO:    May change this if I add an ObservableEntity
                }
            }
        }
    }
}