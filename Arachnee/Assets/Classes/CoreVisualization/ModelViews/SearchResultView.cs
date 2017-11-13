using Assets.Classes.Core.Models;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class SearchResultView : MonoBehaviour
    {
        public delegate void SearchResultViewClickedDelegate(SearchResultView searchResultView);

        public event SearchResultViewClickedDelegate OnClicked;

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