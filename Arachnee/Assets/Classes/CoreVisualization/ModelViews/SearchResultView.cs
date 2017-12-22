using System;
using Assets.Classes.Core.Models;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class SearchResultView : MonoBehaviour
    {
        public event Action<SearchResultView> OnClicked;

        public SearchResult Result { get; set; }

        protected void OnMouseUpAsButton()
        {
            OnClicked?.Invoke(this);
        }

        protected virtual void Start()
        {
            
        }
    }
}