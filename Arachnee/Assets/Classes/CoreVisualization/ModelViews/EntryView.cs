using Assets.Classes.Core.Models;
using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    [RequireComponent(typeof(Rigidbody))]
    public class EntryView : MonoBehaviour
    {
        public delegate void EntryViewClickedDelegate(EntryView v);

        public Entry Entry { get; set; }

        public Rigidbody Rigidbody { get; private set; }

        public event EntryViewClickedDelegate OnClicked;

        void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
            if (Rigidbody == null)
            {
                Logger.LogError($"No {nameof(Rigidbody)} component found on {nameof(EntryView)} GameObject.");
                return;
            }
        }

        void OnMouseUpAsButton()
        {
            OnClicked?.Invoke(this);
        }
    }
}