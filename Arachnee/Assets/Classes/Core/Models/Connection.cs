using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Classes.Core.Models
{
    public class Connection
    {
        public string ConnectedId { get; set; }

        public ConnectionType Type { get; set; }

        public string Label { get; set; }

        /// <summary>
        /// Utility method to get all available ConnectionType values.
        /// </summary>
        public static ICollection<ConnectionType> AllTypes()
        {
            var types = Enum.GetValues(typeof(ConnectionType)).Cast<ConnectionType>();
            return new HashSet<ConnectionType>(types);
        }
    }
}
