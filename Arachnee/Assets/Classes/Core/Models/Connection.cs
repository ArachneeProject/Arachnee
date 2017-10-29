using System;

namespace Assets.Classes.Core.Models
{
    public class Connection
    {
        public string ConnectedId { get; set; }

        public ConnectionType Type { get; set; }

        public string Label { get; set; }
    }
}
