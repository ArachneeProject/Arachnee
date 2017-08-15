using System;
using System.Linq;
using UnityEngine;

namespace Assets.Classes.PhysicsEngine
{
    public class Edge : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        public Transform Left { get; set; }

        public Transform Right { get; set; }

        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (_lineRenderer == null)
            {
                throw new Exception("No LineRenderer component found on Edge GameObject.");
            }
        }

        void Update()
        {
            _lineRenderer.SetPosition(0, Left.position);
            _lineRenderer.SetPosition(1, Right.position);
        }
    }
}