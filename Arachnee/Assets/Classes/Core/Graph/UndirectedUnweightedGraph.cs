using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Logging;

namespace Assets.Classes.Core.Graph
{
    public class UndirectedUnweightedGraph<T> : IGraph<T>
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
                    Logger.LogError($"Self edge on \"{source}\" is ignored.");
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

        /// <summary>
        /// Returns a collection of <see cref="T"/> representing the path from the given source to the given target.<br/> 
        /// The collection doesn't contain the source, but include the target as the last <see cref="T"/> of the collection.<br/> 
        /// Thus, an empty collection means no path exists between the source and the target.
        /// </summary>
        /// <param name="sourceVertex"></param>
        /// <param name="targetVertex"></param>
        /// <returns></returns>
        public virtual List<T> GetShortestPath(T sourceVertex, T targetVertex)
        {
            if (sourceVertex == null)
            {
                throw new ArgumentNullException(nameof(sourceVertex));
            }
            if (targetVertex == null)
            {
                throw new ArgumentNullException(nameof(targetVertex));
            }

            if (!this.ContainsVertex(sourceVertex))
            {
                Logger.LogError($"\"{sourceVertex}\" doesn't exist.");
                return new List<T>();
            }

            if (!this.ContainsVertex(targetVertex))
            {
                Logger.LogError($"\"{targetVertex}\" doesn't exist.");
                return new List<T>();
            }

            if (sourceVertex.Equals(targetVertex))
            {
                Logger.LogWarning($"Asking for shortest path between a vertex and itself \"{sourceVertex}\" returns an empty path.");
                return new List<T>();
            }

            var algo = new GraphAlgorithms<T>();
            var queryFunc = algo.ComputeShortestPathAndGetQueryFunc(this, sourceVertex);
            var result = queryFunc.Invoke(targetVertex).ToList();

            return result;
        }
        
        public virtual bool ContainsEdge(T sourceVertex, T targetVertex)
        {
            return _adjacencyCollection.ContainsKey(sourceVertex)
                   && _adjacencyCollection[sourceVertex].Contains(targetVertex)
                   && _adjacencyCollection.ContainsKey(targetVertex)
                   && _adjacencyCollection[targetVertex].Contains(sourceVertex);
        }

        public HashSet<T> GetChildren(T vertex)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            if (ContainsVertex(vertex))
            {
                return _adjacencyCollection[vertex];
            }

            Logger.LogError($"\"{vertex}\" doesn't exist.");
            return new HashSet<T>();
        }
    }
}