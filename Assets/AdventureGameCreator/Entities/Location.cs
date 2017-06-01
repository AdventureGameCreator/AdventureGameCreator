using System.Collections.Generic;
using AdventureGameCreator.Collections.Generic;

namespace AdventureGameCreator.Entities
{
    public class Location 
    {
        public int id;
        public string title;
        public string description;

        public List<Connection> connections = new List<Connection>();
        public ObservableList<Item> items = new ObservableList<Item>();
    }
}