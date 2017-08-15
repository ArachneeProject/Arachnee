using System.Collections.Generic;

namespace Assets.Classes.GraphElements
{
    public abstract class Entry
    {
        public string Id { get; set; }
        
        // C# 6.0 not supported
        private List<Connection>  _connections = new List<Connection>();

        public List<Connection> Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }

        public override string ToString()
        {
            return Id;
        }
    }
}