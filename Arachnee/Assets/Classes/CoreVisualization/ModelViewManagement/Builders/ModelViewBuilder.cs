using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViews;

namespace Assets.Classes.CoreVisualization.ModelViewManagement.Builders
{
    public class ModelViewBuilder
    {
        private readonly EntryViewBuilder _entryViewBuilder = new EntryViewBuilder();
        private readonly ConnectionViewBuilder _connectionViewBuilder = new ConnectionViewBuilder();
        private readonly SearchResultViewBuilder _searchResultViewBuilder = new SearchResultViewBuilder();

        public void SetPrefab<TEntry>(EntryView prefab) where TEntry : Entry
        {
            _entryViewBuilder.EntryViewPrefabs[typeof(TEntry)] = prefab;
        }

        public void SetPrefab(ConnectionView prefab)
        {
            _connectionViewBuilder.ConnectionViewPrefab = prefab;
        }

        public void SetPrefab(SearchResultView prefab)
        {
            _searchResultViewBuilder.SearchResultViewPrefab = prefab;
        }

        public EntryView BuildView(Entry entry)
        {
            return _entryViewBuilder.BuildEntryView(entry);
        }

        public ConnectionView BuildView(EntryView leftEntryView, EntryView rightEntryView)
        {
            return _connectionViewBuilder.BuildConnectionView(leftEntryView, rightEntryView);
        }

        public SearchResultView BuildView(SearchResult searchResult)
        {
            return _searchResultViewBuilder.BuildResultView(searchResult);
        }
    }
}