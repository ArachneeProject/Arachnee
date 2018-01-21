using System;
using System.Collections.Generic;

namespace Assets.Classes.Core.Graph
{
    public class GraphAlgorithms<T>
    {
        private readonly Dictionary<T, Func<T, ICollection<T>>> _cachedFunctions = new Dictionary<T, Func<T, ICollection<T>>>();

        public HashSet<TVertex> BreadthFirstSearch<TVertex>(IGraph<TVertex> graph, TVertex sourceVertex, Action<TVertex> discoverAccessibleFunc = null)
        {
            var accessibleVertices = new HashSet<TVertex>();

            if (!graph.ContainsVertex(sourceVertex))
            {
                return accessibleVertices;
            }

            var queue = new Queue<TVertex>();
            queue.Enqueue(sourceVertex);

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                if (accessibleVertices.Contains(vertex))
                {
                    continue;
                }

                discoverAccessibleFunc?.Invoke(vertex);

                accessibleVertices.Add(vertex);

                foreach (var child in graph.GetChildren(vertex))
                {
                    if (!accessibleVertices.Contains(child))
                    {
                        queue.Enqueue(child);
                    }
                }
            }

            return accessibleVertices;
        }

        public Func<T, ICollection<T>> ComputeShortestPathAndGetQueryFunc(IGraph<T> graph, T sourceVertex)
        {
            if (_cachedFunctions.ContainsKey(sourceVertex))
            {
                return _cachedFunctions[sourceVertex];
            }
            
            var parents = new Dictionary<T, T>();

            var queue = new Queue<T>();
            queue.Enqueue(sourceVertex);

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();
                foreach (var child in graph.GetChildren(vertex))
                {
                    if (parents.ContainsKey(child))
                    {
                        continue;
                    }

                    parents[child] = vertex;
                    queue.Enqueue(child);
                }
            }

            Func<T, ICollection<T>> shortestPathFunc = v => 
            {
                var path = new List<T>();

                var current = v;
                while (!current.Equals(sourceVertex))
                {
                    path.Add(current);
                    current = parents[current];
                };

                path.Add(sourceVertex);
                path.Reverse();

                return path;
            };

            _cachedFunctions[sourceVertex] = shortestPathFunc;

            return shortestPathFunc;
        }
    }
}