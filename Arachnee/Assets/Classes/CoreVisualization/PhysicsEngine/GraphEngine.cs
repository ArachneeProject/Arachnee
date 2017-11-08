using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.PhysicsEngine
{
    public abstract class GraphEngine : MonoBehaviour
    {
        protected readonly HashSet<Rigidbody> Rigidbodies = new HashSet<Rigidbody>();
        protected readonly Dictionary<Rigidbody, HashSet<Rigidbody>> AdjacentRigidbodies = new Dictionary<Rigidbody, HashSet<Rigidbody>>();

        public virtual void AddRigidbody(Rigidbody rigidbodyToAdd)
        {
            Rigidbodies.Add(rigidbodyToAdd);
            if (!AdjacentRigidbodies.ContainsKey(rigidbodyToAdd))
            {
                AdjacentRigidbodies[rigidbodyToAdd] = new HashSet<Rigidbody>();
            }
        }

        public virtual void AddEdge(Rigidbody leftRigidbody, Rigidbody rightRigidbody)
        {
            if (leftRigidbody == rightRigidbody)
            {
                throw new ArgumentException($"{nameof(this.AddEdge)} from {this.GetType().Name}: the given rigidbodies were the same object.");
            }

            // ensure vertices exist
            AddRigidbody(leftRigidbody);
            AddRigidbody(rightRigidbody);

            // add left to right edge
            AdjacentRigidbodies[leftRigidbody].Add(rightRigidbody);

            // add right to left edge
            AdjacentRigidbodies[rightRigidbody].Add(leftRigidbody);
        }

        public virtual void RemoveRigidbody(Rigidbody rigidbodyToRemove)
        {
            // remove vertex from vertices list
            Rigidbodies.Remove(rigidbodyToRemove);

            // remove all edges using this vertex
            foreach (var adjacentRigidbody in AdjacentRigidbodies[rigidbodyToRemove])
            {
                AdjacentRigidbodies[adjacentRigidbody].Remove(rigidbodyToRemove);
            }

            AdjacentRigidbodies.Remove(rigidbodyToRemove);
        }

        public virtual void RemoveEdge(Rigidbody leftRigidbody, Rigidbody rightRigidbody)
        {
            if (AdjacentRigidbodies.ContainsKey(leftRigidbody))
            {
                AdjacentRigidbodies[leftRigidbody].Remove(rightRigidbody);
            }

            if (AdjacentRigidbodies.ContainsKey(rightRigidbody))
            {
                AdjacentRigidbodies[rightRigidbody].Remove(leftRigidbody);
            }
        }
    }
}