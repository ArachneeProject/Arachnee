using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.GraphElements;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Classes.EntryProviders.Physical
{
    public class PhysicalProvider : EntryProvider
    {
        private readonly List<PhysicalConnection> _cachedConnections = new List<PhysicalConnection>();

        public Dictionary<Type, GameObject> EntryPrefabs { get; private set; }
        public Dictionary<ConnectionFlags, GameObject> ConnectionPrefabs { get; private set; }

        public PhysicalProvider()
        {
            EntryPrefabs = new Dictionary<Type, GameObject>();
            ConnectionPrefabs = new Dictionary<ConnectionFlags, GameObject>();
        }

        public override Stack<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            throw new NotImplementedException();
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            Entry internalEntry;
            GameObject entryPrefab;
            if (BiggerProvider == null 
            || !BiggerProvider.TryGetEntry(entryId, out internalEntry)
            || !EntryPrefabs.TryGetValue(internalEntry.GetType(), out entryPrefab))
            {
                entry = DefaultEntry.Instance;
                return false;
            }

            // instantiate Entry GameObject
            var pEntry = new PhysicalEntry
            {
                Id = internalEntry.Id,
                Connections =  internalEntry.Connections,
                Entry = internalEntry,
                GameObject = Object.Instantiate(entryPrefab)
            };
            
            // The idea here is to instantiate only the Connection GameObjects having their opposite entry already instantiated.
            // (a) find all connections where the opposite entry already has an associated GameObject
            // (b) but where the connection-GameObject in itself is still not instantiated

            // TODO:well this Linq query is a bit hardcore, better split it into separate variables
            foreach (var connection in internalEntry.Connections.Where(connection =>
                GetAvailableEntries<PhysicalEntry>().Select(p => p.Entry.Id)
                    .Contains(connection.GetOppositeOf(internalEntry.Id)) // (a)
                && !this._cachedConnections.Select(c => c.Id)
                    .Contains(connection.Id))) // (b)
            {
                GameObject connectionPrefab;
                if (!ConnectionPrefabs.TryGetValue(connection.Flags, out connectionPrefab))
                {
                    Debug.LogError("Prefab not found for connection " + connection.Id 
                                 + " with flags " + Convert.ToString((int)connection.Flags, 2));
                    continue;
                }

                var pConnection = new PhysicalConnection
                {
                    Id = connection.Id,
                    Left = connection.Left,
                    Right = connection.Right,
                    Flags = connection.Flags,
                    Connection = connection,
                    GameObject = Object.Instantiate(connectionPrefab)
                };

                // TODO: assign vertices to edge

                _cachedConnections.Add(pConnection);
            }

            entry = pEntry;
            return true;
        }

        #region PhysicalEntries

        /// <summary>
        /// Gets the <see cref="PhysicalEntry"/> corresponding to the given id.
        /// </summary>
        /// <param name="entryId">Id of the entry.</param>
        /// <param name="physicalEntry">The resulting <see cref="PhysicalEntry"/>.</param>
        /// <returns>Whether or not the function succedeed.</returns>
        public bool TryGetPhysicalEntry(string entryId, out PhysicalEntry physicalEntry)
        {
            physicalEntry = null;
            Entry e;
            if (!base.TryGetEntry(entryId, out e))
            {
                return false;
            }

            physicalEntry = e as PhysicalEntry;
            return physicalEntry != null;
        }

        /// <summary>
        /// Gets all <see cref="PhysicalEntry"/> available in this <see cref="IEntryProvider"/>.
        /// </summary>
        /// <typeparam name="TEntry">Type of the internal <see cref="Entry"/> in the <see cref="PhysicalEntry"/>.</typeparam>
        /// <returns>The resulting collection of <see cref="PhysicalEntry"/>.</returns>
        public IEnumerable<PhysicalEntry> GetAvailablePhysicalEntries<TEntry>() where TEntry : Entry
        {
            return base.GetAvailableEntries<PhysicalEntry>().Where(p => p.Entry is TEntry);
        }

        /// <summary>
        /// Gets all <see cref="PhysicalEntry"/> connected to the given entry id by at least one of the given connection flags.
        /// </summary>
        /// <typeparam name="TEntry">Type of the internal <see cref="Entry"/> in the connected <see cref="PhysicalEntry"/>.</typeparam>
        /// <param name="entryId">The entry id.</param>
        /// <param name="connectionFlags">Types of connections.</param>
        /// <param name="entries">The resulting collection.</param>
        /// <returns>Whether or not the function succedeed.</returns>
        public bool TryGetConnectedPhysicalEntries<TEntry>(string entryId, ConnectionFlags connectionFlags, out IEnumerable<PhysicalEntry> entries) 
            where TEntry : Entry
        {
            IEnumerable<PhysicalEntry> res;
            if (!base.TryGetConnectedEntries<PhysicalEntry>(entryId, connectionFlags, out res))
            {
                entries = Enumerable.Empty<PhysicalEntry>();
                return false;
            }

            entries = res.Where(p => p.Entry is TEntry);
            return true;
        }

        #endregion PhysicalEntries

        #region PhysicalConnections

        /// <summary>
        /// Gets all <see cref="PhysicalConnection"/> available, having at least one of the given ConnectionFlag.
        /// </summary>
        /// <param name="flags">Connection flags to filter by.</param>
        /// <returns>The collection of <see cref="PhysicalConnection"/>.</returns>
        public IEnumerable<PhysicalConnection> GetAvailablePhysicalConnections(ConnectionFlags flags)
        {
            return _cachedConnections.Where(p => (p.Flags | flags) != 0);
        }

        #endregion PhysicalConnections
    }
}
