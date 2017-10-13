using System.Collections.Generic;

namespace Assets.Classes.GraphElements
{
    public abstract class Entry
    {
        public string Id { get; set; }
        
        public List<Connection> Connections { get; set; } = new List<Connection>();

        public override string ToString()
        {
            return Id;
        }
    }
}