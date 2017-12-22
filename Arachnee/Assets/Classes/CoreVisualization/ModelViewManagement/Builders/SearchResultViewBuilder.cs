using System;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViews;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders
{
    /// <summary>
    /// Class in charge of creating a SearchResultView.
    /// </summary>
    public class SearchResultViewBuilder
    {
        public SearchResultView SearchResultViewPrefab { get; set; }

        /// <summary>
        /// Fired when an <see cref="SearchResult"/> is built.
        /// </summary>
        public event Action<SearchResultView> OnBuilt;

        [CanBeNull]
        public SearchResultView BuildResultView(SearchResult searchResult)
        {
            if (searchResult == null)
            {
                throw new ArgumentNullException(nameof(searchResult));
            }

            var searchResultView = Object.Instantiate(this.SearchResultViewPrefab);
            searchResultView.Result = searchResult;
            searchResultView.gameObject.name = $"{nameof(SearchResult)}: {searchResult.Name} ({searchResult.EntryId})"; ;
            OnBuilt?.Invoke(searchResultView);

            return searchResultView;
        }
    }
}