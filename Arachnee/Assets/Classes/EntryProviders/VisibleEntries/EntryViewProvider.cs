using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using Object = UnityEngine.Object;

namespace Assets.Classes.EntryProviders.VisibleEntries
{
    public class EntryViewProvider : EntryProvider
    {
        private readonly Dictionary<string, EntryView> _cachedEntryViews = new Dictionary<string, EntryView>();
        private readonly Dictionary<string, ConnectionView> _cachedConnectionViews = new Dictionary<string, ConnectionView>();

        public Dictionary<Type, EntryView> EntryViewPrefabs { get; }

        public Dictionary<ConnectionFlags, ConnectionView> ConnectionViewPrefabs { get; }

        public EntryViewProvider()
        {
            EntryViewPrefabs = new Dictionary<Type, EntryView>();
            ConnectionViewPrefabs = new Dictionary<ConnectionFlags, ConnectionView>();
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            EntryView entryViewPrefab;
            if (BiggerProvider == null
                || !BiggerProvider.TryGetEntry(entryId, out entry)
                || !EntryViewPrefabs.TryGetValue(entry.GetType(), out entryViewPrefab))
            {
                entry = DefaultEntry.Instance;
                return false;
            }

            // instantiate EntryView GameObject
            var entryView = Object.Instantiate(entryViewPrefab);
            entryView.Entry = entry;
            entryView.gameObject.name = entry.ToString() + " (" + entry.Id + ")";
            _cachedEntryViews.Add(entryId, entryView);

            // instantiate ConnectionView GameObjects
            foreach (var connection in entry.Connections.Where(c => _cachedEntryViews.ContainsKey(c.ConnectedId)))
            {
                ConnectionView connectionViewPrefab;
                var connectionViewId = Connection.GetIdentifier(entry.Id, connection.ConnectedId, connection.Flags);

                if (_cachedConnectionViews.ContainsKey(connectionViewId)
                || !ConnectionViewPrefabs.TryGetValue(connection.Flags, out connectionViewPrefab))
                {
                    continue;
                }

                var connectionView = Object.Instantiate(connectionViewPrefab);
                connectionView.Left = _cachedEntryViews[entry.Id];
                connectionView.Right = _cachedEntryViews[connection.ConnectedId];
                connectionView.gameObject.name = Connection.GetIdentifier(entry.Id, connection.ConnectedId, connection.Flags);

                _cachedConnectionViews.Add(connectionViewId, connectionView);
            }

            return true;
        }
        
        public override Queue<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            return BiggerProvider.GetSearchResults<TEntry>(searchQuery);
        }

        public bool TryGetEntryView(string entryId, out EntryView entryView)
        {
            if (_cachedEntryViews.ContainsKey(entryId))
            {
                entryView = _cachedEntryViews[entryId];
                if (entryView != null)
                {
                    return true;
                }

                // gameobject associated to the entryId was destroyed by somebody else
                _cachedEntryViews.Remove(entryId);
                CachedEntries.Remove(entryId);
            }

            Entry e;
            if (base.TryGetEntry(entryId, out e))
            {
                entryView = _cachedEntryViews[entryId];
                return true;
            }

            entryView = null;
            return false;
        }

        /// <summary>
        /// Returns a list of connectionViews having at least one of the given connection flags.
        /// </summary>
        /// <param name="entry">Entry to get the connectionView from.</param>
        /// <param name="flags">Connection flags.</param>
        /// <returns>The list of corresponding connectionViews.</returns>
        public List<ConnectionView> GetConnectionViews(Entry entry, ConnectionFlags flags)
        {
            var results = new List<ConnectionView>();

            foreach (var connection in entry.Connections.Where(c => (c.Flags & flags) != 0))
            {
                string connectionId = Connection.GetIdentifier(entry.Id, connection.ConnectedId, connection.Flags);

                if (_cachedConnectionViews.ContainsKey(connectionId))
                {
                    results.Add(_cachedConnectionViews[connectionId]);
                    continue;
                }

                Entry e;
                if (TryGetEntry(entry.Id, out e)
                    && TryGetEntry(connection.ConnectedId, out e)
                    && _cachedConnectionViews.ContainsKey(connectionId))
                {
                    results.Add(_cachedConnectionViews[connectionId]);
                }
            }

            return results;
        }

        public IEnumerable<EntryView> GetAvailableEntryViews<TEntry>() where TEntry : Entry
        {
            // return gameobjects that weren't destroyed by somebody else
            return _cachedEntryViews.Values.Where(v => v != null && v.Entry is TEntry);
        }

        /// <summary>
        /// Runs a search to get a queue of entryViews corresponding to the given query. First entryView in the queue is the best result.
        /// </summary>
        /// <typeparam name="TEntry">Type of entry the resulting entryviews have to contains.</typeparam>
        /// <param name="searchQuery">The query to search for.</param>
        /// <returns>The queue of entryviews.</returns>
        public Queue<EntryView> GetEntryViewResults<TEntry>(string searchQuery) where TEntry : Entry
        {
            var queue = new Queue<EntryView>();
            var results = GetSearchResults<TEntry>(searchQuery);
            foreach (var result in results)
            {
                EntryView v;
                if (TryGetEntryView(result.Id, out v))
                {
                    queue.Enqueue(v);
                }
            }

            return queue;
        }
    }
}