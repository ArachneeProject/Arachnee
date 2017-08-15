using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using UnityEngine;

namespace Assets.Classes.EntryProviders.Physical
{
    /// <summary>
    /// Decorator pattern to add Physics behaviour on a Connection.
    /// </summary>
    public class PhysicalConnection : Connection
    {
        /// <summary>
        /// The game object Physics apply on.
        /// </summary>
        public GameObject GameObject { get; set; }

        public Connection Connection { get; set; }
    }
}
