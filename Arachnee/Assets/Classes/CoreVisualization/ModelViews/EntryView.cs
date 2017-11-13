using Assets.Classes.Core.Models;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class EntryView : MonoBehaviour
    {
        public delegate void EntryViewClickedDelegate(EntryView v);

        public Entry Entry { get; set; }
        
        public event EntryViewClickedDelegate OnClicked;
        
        protected void OnMouseUpAsButton()
        {
            OnClicked?.Invoke(this);
        }

        protected virtual void Start()
        {
            
        }
    }
}