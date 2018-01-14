using System;
using System.Collections.Generic;

namespace Assets.Classes.Core.Graph
{
    public interface IGraph<T>
    {
        bool AddVertex(T vertex);
        
        bool ContainsVertex(T vertex);

        bool AddVerticesAndEdgeRange(ICollection<Tuple<T, T>> edges);

        bool ContainsEdge(T sourceVertex, T targetVertex);

        HashSet<T> GetChildren(T vertex);
    }
}