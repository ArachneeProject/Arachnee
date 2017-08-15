using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.GraphElements;
using UnityEngine;

namespace Assets.Classes.PhysicsEngine
{
    public class Adapter
    {
        private readonly List<Vertex> _vertexCache = new List<Vertex>();
        private readonly List<Edge> _edgeCache = new List<Edge>();

        public Vertex Adapt(Entry entry)
        {
            var vertex = _vertexCache.FirstOrDefault(v => v.Id == entry.Id);
            if (vertex != null)
            {
                return vertex;
            }

            // instantiate vertex
            // ...

            // instantiate valid edges
            // ...
            // add them to cache

            // add vertex to cache

            // return v;
            throw new NotImplementedException();
        }
    }
}