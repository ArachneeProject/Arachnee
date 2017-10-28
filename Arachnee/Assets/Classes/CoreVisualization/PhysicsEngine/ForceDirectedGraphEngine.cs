using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.Utils;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.PhysicsEngine
{
    public class ForceDirectedGraphEngine : GraphEngine
    {
        public float maxSquaredDistanceOfCoulombRepulsion = 10000;
        public float coulombRepulsion = 150;
        public float hookeAttraction = 1;
        public Vector3 centerOfGraph = Vector3.zero;

        private readonly HashSet<ConnectionView> _incompleteEdges = new HashSet<ConnectionView>();

        public override void Add(EntryView entryView)
        {
            this.Vertices.Add(entryView);

            var completeEdges = _incompleteEdges.Where(e => 
                this.Vertices.Contains(e.Left) && this.Vertices.Contains(e.Right)).ToList();
            foreach (var completeEdge in completeEdges)
            {
                this.Edges.Add(completeEdge);
                _incompleteEdges.Remove(completeEdge);
            }
        }

        public override void Add(ConnectionView connectionView)
        {
            if (this.Vertices.Contains(connectionView.Left) && this.Vertices.Contains(connectionView.Right))
            {
                this.Edges.Add(connectionView);
            }
            else
            {
                _incompleteEdges.Add(connectionView);
            }
        }

        public override void Remove(EntryView entryView)
        {
            this.Vertices.Remove(entryView);

            var incompleteEdges = this.Edges.Where(e => 
                !this.Vertices.Contains(e.Left) || !this.Vertices.Contains(e.Right)).ToList();
            foreach (var incompleteEdge in incompleteEdges)
            {
                this.Edges.Remove(incompleteEdge);
                _incompleteEdges.Add(incompleteEdge);
            }
        }

        public override void Remove(ConnectionView connectionView)
        {
            _incompleteEdges.Remove(connectionView);
            this.Edges.Remove(connectionView);
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