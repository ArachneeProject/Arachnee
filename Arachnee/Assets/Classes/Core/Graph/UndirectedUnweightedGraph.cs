using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Classes.Core.Graph
{
    public class UndirectedUnweightedGraph<T>
    {
        private readonly Dictionary<T, HashSet<T>> _adjacencyCollection = new Dictionary<T, HashSet<T>>();

        public int VertexCount => _adjacencyCollection.Keys.Count;
        public int EdgeCount => _adjacencyCollection.Values.Sum(adjacencyCollectionValue => adjacencyCollectionValue.Count) / 2;
        public IEnumerable<T> Vertices => _adjacencyCollection.Keys;
        
        public virtual bool AddVertex(T vertex)
        {
            var alreadyPresent = _adjacencyCollection.ContainsKey(vertex);
            if (alreadyPresent)
            {
                return false;
            }

            _adjacencyCollection[vertex] = new HashSet<T>();
            return true;
        }

        public virtual bool ContainsVertex(T vertex)
        {
            return _adjacencyCollection.Keys.Contains(vertex);
        }

        public virtual bool AddVerticesAndEdgeRange(ICollection<Tuple<T, T>> edges)
        {
            bool added = true;

            foreach (var edge in edges)
            {
                var source = edge.Item1;
                var target = edge.Item2;

                if (source.Equals(target))
                {
                    added = false;
                    continue;
                }

                if (!_adjacencyCollection.ContainsKey(source))
                {
                    _adjacencyCollection[source] = new HashSet<T>();
                }

                if (!_adjacencyCollection.ContainsKey(target))
                {
                    _adjacencyCollection[target] = new HashSet<T>();
                }

                added &= _adjacencyCollection[source].Add(target);
                added &= _adjacencyCollection[target].Add(source);
            }

            return added;
        }
        
        public virtual List<T> GetShortestPath(T sourceVertex, T targetVertex)
        {
            return null;
        }
        
        public virtual bool ContainsEdge(T vertex1, T vertex2)
        {
            return _adjacencyCollection.ContainsKey(vertex1)
                   && _adjacencyCollection[vertex1].Contains(vertex2)
                   && _adjacencyCollection.ContainsKey(vertex2)
                   && _adjacencyCollection[vertex2].Contains(vertex1);
        }

        
    }
}