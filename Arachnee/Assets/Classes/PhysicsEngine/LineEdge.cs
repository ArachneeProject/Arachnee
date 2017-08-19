using System;
using UnityEngine;

namespace Assets.Classes.PhysicsEngine
{
    public class LineEdge : Edge
    {
        private LineRenderer _lineRenderer;

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
            _lineRenderer.SetPosition(0, Left.transform.position);
            _lineRenderer.SetPosition(1, Right.transform.position);
        }
    }
}