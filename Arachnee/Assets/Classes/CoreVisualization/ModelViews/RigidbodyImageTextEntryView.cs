using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class RigidbodyImageTextEntryView : ImageTextEntryView
    {
        public Rigidbody Rigidbody { get; set; }

        public override void Start()
        {
            base.Start();

            Rigidbody = this.gameObject.GetComponent<Rigidbody>();
            if (Rigidbody == null)
            {
                Logger.LogError($"No {nameof(Rigidbody)} component found on {nameof(RigidbodyImageTextEntryView)} gameobject.");
            }
        }
    }
}