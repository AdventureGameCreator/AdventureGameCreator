using System.Collections.Generic;

namespace AdventureGameCreator.Entities
{
    [System.Serializable]
    public class Location 
    {
        public int id;
        public string title;
        public string description;

        public List<Connection> connections = new List<Connection>();
    }
}