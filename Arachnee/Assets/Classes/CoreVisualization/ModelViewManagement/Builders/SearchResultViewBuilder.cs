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

            return searchResultView;
        }
    }
}