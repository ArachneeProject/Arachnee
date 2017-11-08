using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Core.EntryProviders;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.Logging;
using JetBrains.Annotations;

namespace Assets.Classes.CoreVisualization.ModelViewManagement
{
    /// <summary>
    /// Class in charge of responding to queries for Views.
    /// </summary>
    public class ModelViewManager
    {
        private readonly IEntryProvider _provider;
        private readonly ModelViewBuilder _builder;

        private readonly Dictionary<string, EntryView> _cachedEntryViews = new Dictionary<string, EntryView>();

        private readonly Dictionary<string, ConnectionView> _cachedConnectionViews = new Dictionary<string, ConnectionView>();

        public ModelViewManager(IEntryProvider provider, ModelViewBuilder builder)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            _provider = provider;
            _builder = builder;
        }
        
        /// <summary>
        /// Returns the EntryView corresponding to the given entry id.
        /// </summary>
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

            var entryView = _builder.BuildView(entry);
            if (entryView == null)
            {
                Logger.LogError($"Unable to build {nameof(EntryView)} for id \"{entryId}\".");
                return null;
            }

            _cachedEntryViews.Add(entry.Id, entryView);
            return entryView;
        }

        /// <summary>
        /// Returns a collection of all ConnectionViews linked to the given EntryView.
        /// </summary>
        /// <param name="entryView">The EntryView to get the ConnectionViews from.</param>
        public List<ConnectionView> GetConnectionViews(EntryView entryView)
        {
            return GetConnectionViews(entryView, Connection.AllTypes());
        }

        /// <summary>
        /// Returns a collection of ConnectionViews linked to the given EntryView, 
        /// restricted to those having at least one of the given connection types.
        /// </summary>
        /// <param name="entryView">The EntryView to get the ConnectionViews from.</param>
        /// <param name="connectionTypes">The list of restricting connection types.</param>
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

                var connectionView = _builder.BuildView(entryView, oppositeEntryView);
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
        
        /// <summary>
        /// Returns a collection of EntryViews linked to the given EntryView.
        /// </summary>
        /// <param name="entryView">The EntryView to get the connected EntryViews from.</param>
        public List<EntryView> GetConnectedEntryViews(EntryView entryView)
        {
            return GetConnectedEntryViews(entryView, Connection.AllTypes());
        }

        /// /// <summary>
        /// Returns a collection of EntryViews linked to the given EntryView, 
        /// restricted to those linked by at least one of the given connection types.
        /// </summary>
        /// <param name="entryView">The EntryView to get the linked EntryViews from.</param>
        /// <param name="connectionTypes">The list of restricting connection types.</param>
        public List<EntryView> GetConnectedEntryViews(EntryView entryView, List<ConnectionType> connectionTypes)
        {
            var connectionViews = GetConnectionViews(entryView, connectionTypes);
            return connectionViews.Select(c => c.Left == entryView ? c.Right : c.Left).ToList();
        }

        /// <summary>
        /// Returns an ordered collection of SearchResultView corresponding to the given query.
        /// First element is the best result.
        /// </summary>
        /// <param name="searchQuery">The query to search for.</param>
        public Queue<SearchResultView> GetSearchResultViews(string searchQuery)
        {
            var resultsQueue = _provider.GetSearchResults(searchQuery);
            var resultViewsQueue = new Queue<SearchResultView>();

            while (resultsQueue.Any())
            {
                var result = resultsQueue.Dequeue();
                var resultView = _builder.BuildView(result);
                resultViewsQueue.Enqueue(resultView);
            }

            return resultViewsQueue;
        }
    }
}