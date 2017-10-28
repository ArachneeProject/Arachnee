using UnityEngine;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class ConnectionView : MonoBehaviour
    {
        public EntryView Left { get; set; }

        public EntryView Right { get; set; }
    }
}