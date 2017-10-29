using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Core.EntryProviders;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.Logging;
using JetBrains.Annotations;

namespace Assets.Classes.CoreVisualization.ModelViewManagement
{
    public class ModelViewManager
    {
        private readonly IEntryProvider _provider;
        private readonly ModelViewBuilder _builder = new ModelViewBuilder();

        private readonly Dictionary<string, EntryView> _cachedEntryViews = new Dictionary<string, EntryView>();

        private readonly Dictionary<string, ConnectionView> _cachedConnectionViews = new Dictionary<string, ConnectionView>();

        public ModelViewManager(IEntryProvider provider)
        {
            _provider = provider;
        }

        public void SetPrefab<TEntry>(EntryView prefab) where TEntry : Entry
        {
            _builder.EntryViewPrefabs[typeof(TEntry)] = prefab;
        }

        public void SetPrefab(ConnectionView prefab)
        {
            _builder.ConnectionViewPrefab = prefab;
        }

        [CanBeNull]
        public EntryView GetEntryView(string entryId)
        {
            if (_cachedEntryViews.ContainsKey(entryId))
            {
                return _cachedEntryViews[entryId];
            }

            Entry entry;
            if (!_provider.TryGetEntry(entryId, out entry))
            {
                Logger.LogError($"Unable to get {nameof(Entry)} with id \"{entryId}\".");
                return null;
            }

            var entryView = _builder.BuildEntryView(entry);
            if (entryView == null)
            {
                Logger.LogError($"Unable to build {nameof(EntryView)} for id \"{entryId}\".");
                return null;
            }

            _cachedEntryViews.Add(entry.Id, entryView);
            return entryView;
        }

        public List<ConnectionView> GetConnectionViews(EntryView entryView)
        {
            return GetConnectionViews(entryView, Enum.GetValues(typeof(ConnectionType)).Cast<ConnectionType>().ToList());
        }

        public List<ConnectionView> GetConnectionViews(EntryView entryView, List<ConnectionType> connectionTypes)
        {
            var validConnections = entryView.Entry.Connections.Where(c => connectionTypes.Contains(c.Type));
            var connectionsToGet = validConnections.Select(c => new { c.ConnectedId, Identifier = ConnectionView.GetIdentifier(entryView.Entry.Id, c.ConnectedId)});

            var result = new List<ConnectionView>();
            foreach (var connection in connectionsToGet)
            {
                if (_cachedConnectionViews.ContainsKey(connection.Identifier))
                {
                    result.Add(_cachedConnectionViews[connection.Identifier]);
                    continue;
                }

                // build the connection
                var oppositeEntryView = GetEntryView(connection.ConnectedId);
                if (oppositeEntryView == null)
                {
                    Logger.LogError($"Unable to get opposite {nameof(EntryView)} for connection id \"{connection.Identifier}\".");
                    continue;
                }

                var connectionView = _builder.BuildConnectionView(entryView, oppositeEntryView);
                if (connectionView == null)
                {
                    Logger.LogError($"Unable to build {nameof(ConnectionView)} for connection id \"{connection.Identifier}\".");
                    continue;
                }

                _cachedConnectionViews[connectionView.Id] = connectionView;
                result.Add(connectionView);
            }

            return result;
        }
        
        public Queue<SearchResultView> GetSearchResultViews(string searchQuery)
        {
            throw new NotImplementedException();
        }
    }
}