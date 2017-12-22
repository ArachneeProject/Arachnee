using System;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViews;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders
{
    /// <summary>
    /// Class in charge of creating the Views corresponding to the given Models.
    /// </summary>
    public class ModelViewBuilder
    {
        private readonly EntryViewBuilder _entryViewBuilder = new EntryViewBuilder();
        private readonly ConnectionViewBuilder _connectionViewBuilder = new ConnectionViewBuilder();
        private readonly SearchResultViewBuilder _searchResultViewBuilder = new SearchResultViewBuilder();
        
        public event Action<EntryView> OnBuiltEntryView;
        public event Action<ConnectionView> OnBuiltConnectionView;
        public event Action<SearchResultView> OnBuiltSearchResult;

        public ModelViewBuilder()
        {
            _entryViewBuilder.OnBuilt += (arg) => OnBuiltEntryView?.Invoke(arg);
            _connectionViewBuilder.OnBuilt += (arg) => OnBuiltConnectionView?.Invoke(arg);
            _searchResultViewBuilder.OnBuilt += (arg) => OnBuiltSearchResult?.Invoke(arg);
        }
        
        /// <summary>
        /// Sets the prefab that should be used to build an Entry.
        /// </summary>
        /// <typeparam name="TEntry">Type of Entry that should be represented by the given prefab.</typeparam>
        /// <param name="prefab">The prefab to set.</param>
        public void SetPrefab<TEntry>(EntryView prefab) where TEntry : Entry
        {
            _entryViewBuilder.EntryViewPrefabs[typeof(TEntry)] = prefab;
        }

        /// <summary>
        /// Sets the prefab that should be used to build a ConnectionView.
        /// </summary>
        /// <param name="prefab"></param>
        public void SetPrefab(ConnectionView prefab)
        {
            _connectionViewBuilder.ConnectionViewPrefab = prefab;
        }

        /// <summary>
        /// Sets the prefab that should be used to build a SearchResult.
        /// </summary>
        /// <param name="prefab"></param>
        public void SetPrefab(SearchResultView prefab)
        {
            _searchResultViewBuilder.SearchResultViewPrefab = prefab;
        }
        
        /// <summary>
        /// Build the EntryView corresponding to the given Entry. 
        /// Uses the prefab set by the SetPrefab method.
        /// </summary>
        /// <param name="entry">The Entry that should be represented by an EntryView.</param>
        /// <returns>The EntryView.</returns>
        public EntryView BuildView(Entry entry)
        {
            var view = _entryViewBuilder.BuildEntryView(entry);
            return view;
        }

        /// <summary>
        /// Build the ConnectionView corresponding to the connections existing between the given two EntryViews.
        /// Uses the prefab set by the SetPrefab method.
        /// </summary>
        /// <param name="leftEntryView">One of the two EntryView of the connection.</param>
        /// <param name="rightEntryView">The other EntryView of the connection.</param>
        /// <returns>The ConnectionView.</returns>
        public ConnectionView BuildView(EntryView leftEntryView, EntryView rightEntryView)
        {
            var view = _connectionViewBuilder.BuildConnectionView(leftEntryView, rightEntryView);
            return view;
        }

        /// <summary>
        /// Build the SearchResultView corresponding to the given SearchResult. 
        /// Uses the prefab set by the SetPrefab method.
        /// </summary>
        /// <param name="searchResult">The SearchResult that should be represented by a SearchResultView.</param>
        /// <returns>The SearchResultView.</returns>
        public SearchResultView BuildView(SearchResult searchResult)
        {
            var view = _searchResultViewBuilder.BuildResultView(searchResult);
            return view;
        }
    }
}