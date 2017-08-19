using System;
using System.Linq;
using UnityEngine;

namespace Assets.Classes.PhysicsEngine
{
    public class Edge : MonoBehaviour
    {
        public Vertex Left { get; set; }

        public Vertex Right { get; set; }
    }
}