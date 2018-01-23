using System.Linq;
using Assets.Classes.SceneScripts;
using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class LineConnectionViewWithFloatingText : LineConnectionView
    {
        public Vector3 offset = Vector3.up * 10;

        private FloatingText _floatingText;

        protected override void Start()
        {
            base.Start();

            _floatingText = this.GetComponentInChildren<FloatingText>();
            if (_floatingText == null)
            {
                Logger.LogError($"{nameof(FloatingText)} not found in children of {nameof(LineConnectionViewWithFloatingText)} game object.");
                return;
            }

            var label = Left.Entry.Connections.FirstOrDefault(c => c.ConnectedId == Right.Entry.Id)?.Label;
            if (string.IsNullOrEmpty(label))
            {
                var rightToLeftConnection = Right.Entry.Connections.FirstOrDefault(c => c.ConnectedId == Left.Entry.Id);
                if (rightToLeftConnection == null)
                {
                    Logger.LogError($"Connection between {this.Left.Entry} and {this.Right.Entry} doesn't exist.");
                    return;
                }

                label = string.IsNullOrEmpty(rightToLeftConnection.Label) 
                    ? string.Empty 
                    : rightToLeftConnection.Label;
            }
            
            _floatingText.SetText(label);
        }

        protected override void Update()
        {
            base.Update();

            _floatingText.transform.position = (Left.transform.position + Right.transform.position) / 2f;
            _floatingText.transform.Translate(offset);
        }
    }
}