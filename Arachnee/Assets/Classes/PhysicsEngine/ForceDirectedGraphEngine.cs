using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Classes.PhysicsEngine
{
    public class ForceDirectedGraphEngine : GraphEngine
    {
        public float maxSquaredDistanceOfCoulombRepulsion = 10000;
        public float coulombRepulsion = 150;
        public float hookeAttraction = 1;
        public Vector3 centerOfGraph = Vector3.zero;

        private readonly HashSet<Edge> _incompleteEdges = new HashSet<Edge>();

        public override void Add(Vertex vertex)
        {
            this.Vertices.Add(vertex);

            var completeEdges = _incompleteEdges.Where(e => 
                this.Vertices.Contains(e.Left) && this.Vertices.Contains(e.Right)).ToList();
            foreach (var completeEdge in completeEdges)
            {
                this.Edges.Add(completeEdge);
                _incompleteEdges.Remove(completeEdge);
            }
        }

        public override void Add(Edge edge)
        {
            if (this.Vertices.Contains(edge.Left) && this.Vertices.Contains(edge.Right))
            {
                this.Edges.Add(edge);
            }
            else
            {
                _incompleteEdges.Add(edge);
            }
        }

        public override void Remove(Vertex vertex)
        {
            this.Vertices.Remove(vertex);

            var incompleteEdges = this.Edges.Where(e => 
                !this.Vertices.Contains(e.Left) || !this.Vertices.Contains(e.Right)).ToList();
            foreach (var incompleteEdge in incompleteEdges)
            {
                this.Edges.Remove(incompleteEdge);
                _incompleteEdges.Add(incompleteEdge);
            }
        }

        public override void Remove(Edge edge)
        {
            _incompleteEdges.Remove(edge);
            this.Edges.Remove(edge);
        }

        void FixedUpdate()
        {
            // TODO: can be improved from n² to n(n-1)/2 computations
            foreach (var vertex in this.Vertices.Where(vertex => vertex.Rigidbody != null))
            {
                // repulsion
                foreach (var otherVertex in this.Vertices)
                {
                    if (otherVertex == vertex)
                    {
                        continue;
                    }

                    float squaredDistance = MiniMath.GetSquaredDistance(vertex.transform.position, otherVertex.transform.position);
                    if (squaredDistance > maxSquaredDistanceOfCoulombRepulsion
                    || Math.Abs(squaredDistance) < 0.001)
                    {
                        continue;
                    }
                    
                    Vector3 repulsion = this.coulombRepulsion*
                                        (vertex.transform.position - otherVertex.transform.position)*
                                        (1F/squaredDistance);
                    vertex.Rigidbody.AddForce(repulsion);
                }

                // attraction to center
                vertex.Rigidbody.AddForce(centerOfGraph - vertex.transform.position);
            }

            // attraction
            foreach (var edge in this.Edges)
            {
                if (edge.Left.Rigidbody == null || edge.Right.Rigidbody == null)
                {
                    continue;
                }

                Vector3 attraction = this.hookeAttraction*(edge.Left.transform.position - edge.Right.transform.position);
                edge.Left.Rigidbody.AddForce(-attraction);
                edge.Right.Rigidbody.AddForce(attraction);
            }
        }
    }
}