using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineConnectionView : ConnectionView
    {
        private LineRenderer _lineRenderer;
        
        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (_lineRenderer == null)
            {
                Logger.LogError($"No {nameof(LineRenderer)} component found on {nameof(LineConnectionView)} GameObject.");
                return;
            }
        }
        
        void Update()
        {
            _lineRenderer.SetPosition(0, Left.transform.position);
            _lineRenderer.SetPosition(1, Right.transform.position);
        }
    }
}