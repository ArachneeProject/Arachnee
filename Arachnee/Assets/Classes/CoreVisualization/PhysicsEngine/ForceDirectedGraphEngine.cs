using System;
using System.Collections.Generic;
using System.Linq;
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

        private List<Rigidbody> _rigidbodies = new List<Rigidbody>();

        public override void AddRigidbody(Rigidbody rigidbodyToAdd)
        {
            base.AddRigidbody(rigidbodyToAdd);
            _rigidbodies = AdjacentRigidbodies.Keys.ToList();
        }

        public override void RemoveRigidbody(Rigidbody rigidbodyToRemove)
        {
            base.RemoveRigidbody(rigidbodyToRemove);
            _rigidbodies = AdjacentRigidbodies.Keys.ToList();
        }

        // TODO: try Barnes-Hut
        void FixedUpdate()
        {
            for (int i = 0; i < _rigidbodies.Count; i++)
            {
                var rigidBody = _rigidbodies[i];

                for (int j = i + 1; j < _rigidbodies.Count; j++)
                {
                    var otherRigidbody = _rigidbodies[j];

                    // repulsion
                    float squaredDistance = MiniMath.GetSquaredDistance(rigidBody.transform.position, otherRigidbody.transform.position);
                    if (squaredDistance > maxSquaredDistanceOfCoulombRepulsion)
                    {
                        // other rigidbody is too far away
                        continue;
                    }

                    if (squaredDistance < 0.001)
                    {
                        // other rigidbody is too close and will produce an extrem repulsive force
                        continue;
                    }

                    Vector3 repulsion = this.coulombRepulsion *
                                        (rigidBody.transform.position - otherRigidbody.transform.position) *
                                        (1F / squaredDistance);
                    rigidBody.AddForce(repulsion);
                    otherRigidbody.AddForce(-repulsion);
                }

                // attraction to center
                rigidBody.AddForce(centerOfGraph - rigidBody.transform.position);

                // attraction to connected rigidbodies
                foreach (var connectedRigidbody in this.AdjacentRigidbodies[rigidBody])
                {
                    Vector3 attraction = this.hookeAttraction * (rigidBody.transform.position - connectedRigidbody.transform.position);
                    rigidBody.AddForce(-attraction);
                }
            }
        }
    }
}