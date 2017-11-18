using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Core.EntryProviders;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViews;
using JetBrains.Annotations;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.ModelViewManagement
{
    /// <summary>
    /// Class in charge of responding to queries for Views.
    /// </summary>
    public class ModelViewProvider
    {
        private readonly IEntryProvider _provider;
        private readonly ModelViewBuilder _builder;

        private readonly Dictionary<string, EntryView> _cachedEntryViews = new Dictionary<string, EntryView>();

        private readonly Dictionary<string, ConnectionView> _cachedConnectionViews = new Dictionary<string, ConnectionView>();
        
        public ModelViewProvider(IEntryProvider provider, ModelViewBuilder builder)
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

            return BuildEntryView(entry);
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
            var connectionsToGet = validConnections.Select(c => new { c.ConnectedId, Identifier = ConnectionView.GetIdentifier(entryView.Entry.Id, c.ConnectedId) });

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
        
        /// /// <summary>
        /// Returns a collection of EntryViews linked to the given EntryView, 
        /// restricted to those linked by at least one of the given connection types.
        /// </summary>
        /// <param name="entryView">The EntryView to get the linked EntryViews from.</param>
        /// <param name="connectionTypes">The list of restricting connection types.</param>
        public List<EntryView> GetAdjacentEntryViews(EntryView entryView, List<ConnectionType> connectionTypes)
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

        #region UnityAsync

        private object _routineResult;
        
        /// <summary>
        /// Same as <see cref="GetEntryView"/>, but awaitable.
        /// </summary>
        public AwaitableCall<EntryView> GetEntryViewAsync(string entryId)
        {
            return new AwaitableCall<EntryView>(() => GetEntryViewAsyncRoutine(entryId), () => (EntryView) _routineResult);
        }

        private IEnumerator GetEntryViewAsyncRoutine(string entryId)
        {
            AsyncCall<Entry, EntryView> asyncCall;

            if (_cachedEntryViews.ContainsKey(entryId))
            {
                asyncCall = new AsyncCall<Entry, EntryView>(() => null, x => _cachedEntryViews[entryId]);
                yield return asyncCall.Execute();
                _routineResult = asyncCall.Result;
                yield break;
            }

            asyncCall = new AsyncCall<Entry, EntryView>(() =>
            {
                Entry entry;
                return _provider.TryGetEntry(entryId, out entry)
                    ? entry
                    : null;
            },
                entry =>
                {
                    if (entry == null || entry == DefaultEntry.Instance)
                    {
                        Logger.LogError($"Unable to get {nameof(Entry)} with id \"{entryId}\".");
                        return null;
                    }

                    return BuildEntryView(entry);
                });

            yield return asyncCall.Execute();
            _routineResult = asyncCall.Result;
        }
        
        /// <summary>
        /// Same as <see cref="GetConnectionViews"/>, but awaitable.
        /// </summary>
        public AwaitableCall<List<ConnectionView>> GetConnectionViewsAsync(EntryView entryView, List<ConnectionType> connectionTypes)
        {
            return new AwaitableCall<List<ConnectionView>>(() => GetConnectionViewsAsyncRoutine(entryView, connectionTypes), () => (List<ConnectionView>) _routineResult);
        }

        private IEnumerator GetConnectionViewsAsyncRoutine(EntryView entryView, List<ConnectionType> connectionTypes)
        {
            var result = new List<ConnectionView>();

            var validConnections = entryView.Entry.Connections.Where(c => connectionTypes.Contains(c.Type));
            var connectionsToGet = validConnections.Select(c => new
            {
                c.ConnectedId,
                Identifier = ConnectionView.GetIdentifier(entryView.Entry.Id, c.ConnectedId)
            });

            foreach (var connection in connectionsToGet)
            {
                if (_cachedConnectionViews.ContainsKey(connection.Identifier))
                {
                    result.Add(_cachedConnectionViews[connection.Identifier]);
                    continue;
                }

                // build the connection
                var awaitableCall = GetEntryViewAsync(connection.ConnectedId);
                yield return awaitableCall.Await();

                var oppositeEntryView = awaitableCall.Result;
                if (oppositeEntryView == null)
                {
                    Logger.LogError($"Unable to get opposite {nameof(EntryView)} for connection id \"{connection.Identifier}\".");
                    continue;
                }

                var connectionView = _builder.BuildView(entryView, oppositeEntryView);
                if (connectionView == null)
                {
                    Logger.LogError(
                        $"Unable to build {nameof(ConnectionView)} for connection id \"{connection.Identifier}\".");
                    continue;
                }

                _cachedConnectionViews[connectionView.Id] = connectionView;
                result.Add(connectionView);
            }

            _routineResult = result;
        }
        
        /// <summary>
        /// Same as <see cref="GetAdjacentEntryViews"/>, but awaitable.
        /// </summary>
        public AwaitableCall<List<EntryView>> GetAdjacentEntryViewsAsync(EntryView entryView, List<ConnectionType> connectionTypes)
        {
            return new AwaitableCall<List<EntryView>>(() => GetAdjacentEntryViewsAsyncRoutine(entryView, connectionTypes), () => (List<EntryView>) _routineResult);
        }

        private IEnumerator GetAdjacentEntryViewsAsyncRoutine(EntryView entryView, List<ConnectionType> connectionTypes)
        {
            var awaitableCall = GetConnectionViewsAsync(entryView, connectionTypes);
            yield return awaitableCall.Await();
            _routineResult = awaitableCall.Result.Select(c => c.Left == entryView ? c.Right : c.Left).ToList();
        }

        #endregion UnityAsync

        private EntryView BuildEntryView(Entry entry)
        {
            var entryView = _builder.BuildView(entry);
            if (entryView == null)
            {
                Logger.LogError($"Unable to build {nameof(EntryView)} for id \"{entry.Id}\".");
                return null;
            }

            _cachedEntryViews.Add(entry.Id, entryView);
            return entryView;
        }
    }
}