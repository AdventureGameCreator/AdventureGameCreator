namespace AdventureGameCreator.Entities
{
    public class Connection
    {
        private int _id;
        private string _descriptor;
        private string _key;


        /// <summary>
        /// Returns the connection's id
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Returns the connection's descriptor
        /// </summary>
        public string Descriptor
        {
            get { return _descriptor; }
            set { _descriptor = value; }
        }

        /// <summary>
        /// Returns the connection's key
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
    }
}