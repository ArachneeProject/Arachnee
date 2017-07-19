using Arachnee.GraphElements;
using UnityEngine;

namespace Assets.CSharp.EntryProviders.Physical
{
    /// <summary>
    /// Decorator pattern to add Physics behaviour on an entry.
    /// </summary>
    public class PhysicalEntry : Entry
    {
        /// <summary>
        /// The game object Physics apply on.
        /// </summary>
        public GameObject GameObject { get; set; }

        public Entry Entry { get; set; }
    }
}
