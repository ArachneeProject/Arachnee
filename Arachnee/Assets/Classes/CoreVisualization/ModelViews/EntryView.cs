using System;
using Assets.Classes.Core.Models;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class EntryView : MonoBehaviour
    {
        public Entry Entry { get; set; }
        
        public event Action<EntryView> OnClicked;
        
        protected void OnMouseUpAsButton()
        {
            OnClicked?.Invoke(this);
        }

        public virtual void Start()
        {
            
        }
    }
}