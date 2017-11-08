using System;
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

        // TODO: improve computational complexity
        void FixedUpdate()
        {
            foreach (var rigidBody in this.Rigidbodies)
            {
                foreach (var otherRigidbody in this.Rigidbodies)
                {
                    if (otherRigidbody == rigidBody)
                    {
                        continue;
                    }

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

                    Vector3 repulsion = this.coulombRepulsion*
                                        (rigidBody.transform.position - otherRigidbody.transform.position)*
                                        (1F/squaredDistance);
                    rigidBody.AddForce(repulsion);
                }

                // attraction to center
                rigidBody.AddForce(centerOfGraph - rigidBody.transform.position);
            }

            // attraction
            foreach (var rigidBody in this.AdjacentRigidbodies.Keys)
            {
                foreach (var connectedRigidbody in this.AdjacentRigidbodies[rigidBody])
                {
                    Vector3 attraction = this.hookeAttraction * (rigidBody.transform.position - connectedRigidbody.transform.position);
                    rigidBody.AddForce(-attraction);
                    connectedRigidbody.AddForce(attraction);
                }
            }
        }
    }
}