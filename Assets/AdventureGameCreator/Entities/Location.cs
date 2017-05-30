using System.Collections.Generic;

namespace AdventureGameCreator.Entities
{
    public class Location 
    {
        public int id;
        public string title;
        public string description;

        public List<Connection> connections = new List<Connection>();
        public List<Item> items = new List<Item>();
    }
}