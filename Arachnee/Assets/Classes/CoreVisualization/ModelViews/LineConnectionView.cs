using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineConnectionView : ConnectionView
    {
        private LineRenderer _lineRenderer;
        
        protected override void Start()
        {
            base.Start();

            _lineRenderer = GetComponent<LineRenderer>();
            if (_lineRenderer == null)
            {
                Logger.LogError($"No {nameof(LineRenderer)} component found on {nameof(LineConnectionView)} GameObject.");
                return;
            }
        }
        
        protected override void Update()
        {
            base.Update();

            _lineRenderer.SetPosition(0, Left.transform.position);
            _lineRenderer.SetPosition(1, Right.transform.position);
        }
    }
}