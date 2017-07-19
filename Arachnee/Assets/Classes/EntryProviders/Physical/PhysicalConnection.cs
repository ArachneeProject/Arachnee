using Arachnee.GraphElements;
using UnityEngine;

namespace Assets.CSharp.EntryProviders.Physical
{
    public class PhysicalConnection : Connection
    {
        public GameObject GameObject { get; set; }

        public Connection Connection { get; set; }
    }
}
