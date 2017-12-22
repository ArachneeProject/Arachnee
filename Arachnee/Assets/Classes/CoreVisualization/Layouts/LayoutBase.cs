using UnityEngine;

namespace Assets.Classes.CoreVisualization.Layouts
{
    public abstract class LayoutBase : MonoBehaviour
    {
        /// <summary>
        /// Apply layout on the given transform.
        /// </summary>
        public abstract void Add(Transform transformToAdd);
    }
}