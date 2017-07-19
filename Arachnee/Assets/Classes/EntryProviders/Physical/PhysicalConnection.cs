using Assets.Classes.GraphElements;
using UnityEngine;

namespace Assets.Classes.EntryProviders.Physical
{
    public class PhysicalConnection : Connection
    {
        public GameObject GameObject { get; set; }

        public Connection Connection { get; set; }
    }
}
