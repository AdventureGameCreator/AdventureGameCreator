namespace AdventureGameCreator.Entities
{
    public class Item
    {
        private string _name;
        private string _key;
        private bool _isVisible;
        private bool _selected;


        /// <summary>
        /// Returns the item's name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Returns the item's key
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// Indicates whether the item is visible
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        /// <summary>
        /// Indicates whether the item is selected by the player
        /// </summary>
        public bool IsSelected
        {
            get { return _selected; }
            set { _selected = value; }
        }
    }
}