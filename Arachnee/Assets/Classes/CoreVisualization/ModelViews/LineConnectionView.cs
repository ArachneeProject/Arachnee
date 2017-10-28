using System;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class LineConnectionView : ConnectionView
    {
        private LineRenderer _lineRenderer;

        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (_lineRenderer == null)
            {
                throw new Exception("No LineRenderer component found on ConnectionView GameObject.");
            }
        }

        void Update()
        {
            _lineRenderer.SetPosition(0, Left.transform.position);
            _lineRenderer.SetPosition(1, Right.transform.position);
        }
    }
}