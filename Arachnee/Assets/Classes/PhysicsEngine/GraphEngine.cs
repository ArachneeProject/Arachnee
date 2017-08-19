using System.Collections.Generic;
using Assets.Classes.EntryProviders.Physical;
using UnityEngine;

namespace Assets.Classes.PhysicsEngine
{
    public abstract class GraphEngine : MonoBehaviour
    {
        protected HashSet<Vertex> Vertices = new HashSet<Vertex>();
        protected HashSet<Edge> Edges = new HashSet<Edge>();

        public abstract void Add(Vertex vertex);

        public abstract void Add(Edge edge);

        public abstract void Remove(Vertex vertex);

        public abstract void Remove(Edge edge);
    }
}