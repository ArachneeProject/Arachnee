using UnityEngine;

namespace Assets.Classes.CoreVisualization.Layouts
{
    public abstract class LayoutBase : MonoBehaviour
    {
        /// <summary>
        /// Apply layout on the given transform.
        /// </summary>
        public abstract bool Add(RectTransform transformToAdd);

        public abstract void Clear();

        public abstract void Start();
    }
}