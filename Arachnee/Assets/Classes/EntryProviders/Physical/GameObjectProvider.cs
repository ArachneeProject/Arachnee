using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using Assets.Classes.Utils;
using UnityEngine;
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

        public override Stack<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            return BiggerProvider.GetSearchResults<TEntry>(searchQuery);
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
                _cachedEdges.Add(edgeId, edge);
            }

            return true;
        }

        public bool TryGetVertex(string entryId, out Vertex vertex)
        {
            Entry e;
            if (base.TryGetEntry(entryId, out e))
            {
                vertex = _cachedVertices[entryId];
                return true;
            }

            vertex = null;
            return false;
        }
        
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

        public IEnumerable<Vertex> GetVertices<TEntry>() where TEntry : Entry
        {
            return _cachedVertices.Values.Where(v => v.Entry is TEntry);
        }

        public Vertex GetVertex(string entryId)
        {
            Entry e;
            return base.TryGetEntry(entryId, out e) ? _cachedVertices[entryId] : null;
        }
    }
}