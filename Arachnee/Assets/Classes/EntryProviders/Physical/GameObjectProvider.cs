using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using Object = UnityEngine.Object;

namespace Assets.Classes.EntryProviders.Physical
{
    public class GameObjectProvider : EntryProvider
    {
        private readonly Dictionary<string, Vertex> _cachedVertices = new Dictionary<string, Vertex>();
        private readonly Dictionary<string, Edge> _cachedEdges = new Dictionary<string, Edge>();

        public Dictionary<Type, Vertex> VertexPrefabs { get; private set; }

        public Dictionary<ConnectionFlags, Edge> EdgePrefabs { get; private set; }

        public GameObjectProvider()
        {
            VertexPrefabs = new Dictionary<Type, Vertex>();
            EdgePrefabs = new Dictionary<ConnectionFlags, Edge>();
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            Vertex vertexPrefab;
            if (BiggerProvider == null
                || !BiggerProvider.TryGetEntry(entryId, out entry)
                || !VertexPrefabs.TryGetValue(entry.GetType(), out vertexPrefab))
            {
                entry = DefaultEntry.Instance;
                return false;
            }

            // instantiate Vertex GameObject
            var vertex = Object.Instantiate(vertexPrefab);
            vertex.Entry = entry;
            vertex.gameObject.name = entry.ToString() + " (" + entry.Id + ")";
            _cachedVertices.Add(entryId, vertex);

            // instantiate Edge GameObjects
            foreach (var connection in entry.Connections.Where(c => _cachedVertices.ContainsKey(c.ConnectedId)))
            {
                Edge edgePrefab;
                var edgeId = Connection.GetIdentifier(entry.Id, connection.ConnectedId, connection.Flags);

                if (_cachedEdges.ContainsKey(edgeId)
                || !EdgePrefabs.TryGetValue(connection.Flags, out edgePrefab))
                {
                    continue;
                }

                var edge = Object.Instantiate(edgePrefab);
                edge.Left = _cachedVertices[entry.Id];
                edge.Right = _cachedVertices[connection.ConnectedId];
                edge.gameObject.name = Connection.GetIdentifier(entry.Id, connection.ConnectedId, connection.Flags);

                _cachedEdges.Add(edgeId, edge);
            }

            return true;
        }
        
        public override Queue<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            return BiggerProvider.GetSearchResults<TEntry>(searchQuery);
        }

        public bool TryGetVertex(string entryId, out Vertex vertex)
        {
            if (_cachedVertices.ContainsKey(entryId))
            {
                vertex = _cachedVertices[entryId];
                if (vertex != null)
                {
                    return true;
                }

                // gameobject associated to the entryId was destroyed by somebody else
                _cachedVertices.Remove(entryId);
                CachedEntries.Remove(entryId);
            }

            Entry e;
            if (base.TryGetEntry(entryId, out e))
            {
                vertex = _cachedVertices[entryId];
                return true;
            }

            vertex = null;
            return false;
        }

        /// <summary>
        /// Returns a list of edges having at least one of the given connection flags.
        /// </summary>
        /// <param name="entry">Entry to get the edge from.</param>
        /// <param name="flags">Connection flags.</param>
        /// <returns>The list of corresponding edges.</returns>
        public List<Edge> GetEdges(Entry entry, ConnectionFlags flags)
        {
            var results = new List<Edge>();

            foreach (var connection in entry.Connections.Where(c => (c.Flags & flags) != 0))
            {
                string connectionId = Connection.GetIdentifier(entry.Id, connection.ConnectedId, connection.Flags);

                if (_cachedEdges.ContainsKey(connectionId))
                {
                    results.Add(_cachedEdges[connectionId]);
                    continue;
                }

                Entry e;
                if (TryGetEntry(entry.Id, out e)
                    && TryGetEntry(connection.ConnectedId, out e)
                    && _cachedEdges.ContainsKey(connectionId))
                {
                    results.Add(_cachedEdges[connectionId]);
                }
            }

            return results;
        }

        public IEnumerable<Vertex> GetAvailableVertices<TEntry>() where TEntry : Entry
        {
            // return gameobjects that weren't destroyed by somebody else
            return _cachedVertices.Values.Where(v => v != null && v.Entry is TEntry);
        }

        /// <summary>
        /// Runs a search to get a queue of vertices corresponding to the given query. First vertex in the queue is the best result.
        /// </summary>
        /// <typeparam name="TEntry">Type of entry the resulting vertices have to contains.</typeparam>
        /// <param name="searchQuery">The query to search for.</param>
        /// <returns>The queue of vertices.</returns>
        public Queue<Vertex> GetVerticesResults<TEntry>(string searchQuery) where TEntry : Entry
        {
            var queue = new Queue<Vertex>();
            var results = GetSearchResults<TEntry>(searchQuery);
            foreach (var result in results)
            {
                Vertex v;
                if (TryGetVertex(result.Id, out v))
                {
                    queue.Enqueue(v);
                }
            }

            return queue;
        }
    }
}