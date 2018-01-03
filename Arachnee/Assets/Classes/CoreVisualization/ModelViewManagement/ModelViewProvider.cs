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
using Object = UnityEngine.Object;

namespace Assets.Classes.CoreVisualization.ModelViewManagement
{
    /// <summary>
    /// Class in charge of responding to queries for Views.
    /// </summary>
    public class ModelViewProvider
    {
        private readonly IEntryProvider _provider;
        
        private readonly Dictionary<string, EntryView> _cachedEntryViews = new Dictionary<string, EntryView>();
        private readonly Dictionary<string, ConnectionView> _cachedConnectionViews = new Dictionary<string, ConnectionView>();
        private readonly Dictionary<string, SearchResultView> _cachedSearchResultViews = new Dictionary<string, SearchResultView>();

        public IEnumerable<EntryView> ActiveEntryViews => _cachedEntryViews.Values.Where(e => e.isActiveAndEnabled);

        public ModelViewBuilder Builder { get; }

        public event Action<EntryView> OnEntryViewSelected;

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
            Builder = builder;
            Builder.OnEntryViewBuilt += AddHandler;
        }

        private void AddHandler(EntryView e)
        {
            e.OnClicked += FireCLickedEvent;
        }

        private void FireCLickedEvent(EntryView clicked)
        {
            this.OnEntryViewSelected?.Invoke(clicked);
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
        /// <param name="adjacentEntryView">Optionnal adjacent EntryView to get only its related ConnectionViews.</param>
        public List<ConnectionView> GetConnectionViews(EntryView entryView, ICollection<ConnectionType> connectionTypes, EntryView adjacentEntryView = null)
        {
            if (entryView == null)
            {
                return new List<ConnectionView>();
            }

            var validConnections = adjacentEntryView?.Entry == null 
                ? entryView.Entry.Connections.Where(c => connectionTypes.Contains(c.Type)) 
                : entryView.Entry.Connections.Where(c => c.ConnectedId == adjacentEntryView.Entry.Id && connectionTypes.Contains(c.Type));
                
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

                var connectionView = Builder.BuildView(entryView, oppositeEntryView);
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
        public List<EntryView> GetAdjacentEntryViews(EntryView entryView, ICollection<ConnectionType> connectionTypes)
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
                var resultView = BuildOrReuseSearchResultView(result);
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
                asyncCall.Result?.gameObject.SetActive(true);
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
        public AwaitableCall<List<ConnectionView>> GetConnectionViewsAsync(EntryView entryView, ICollection<ConnectionType> connectionTypes, EntryView adjacentEntryView = null)
        {
            return new AwaitableCall<List<ConnectionView>>(() => GetConnectionViewsAsyncRoutine(entryView, connectionTypes, adjacentEntryView), () => (List<ConnectionView>) _routineResult);
        }

        private IEnumerator GetConnectionViewsAsyncRoutine(EntryView entryView, ICollection<ConnectionType> connectionTypes, EntryView adjacentEntryView)
        {
            var result = new List<ConnectionView>();

            var validConnections = adjacentEntryView?.Entry == null
                ? entryView.Entry.Connections.Where(c => connectionTypes.Contains(c.Type))
                : entryView.Entry.Connections.Where(c => c.ConnectedId == adjacentEntryView.Entry.Id && connectionTypes.Contains(c.Type));

            var connectionsToGet = validConnections.Select(c => new
            {
                c.ConnectedId,
                Identifier = ConnectionView.GetIdentifier(entryView.Entry.Id, c.ConnectedId)
            });

            foreach (var connection in connectionsToGet)
            {
                if (_cachedConnectionViews.ContainsKey(connection.Identifier))
                {
                    var existingConnection = _cachedConnectionViews[connection.Identifier];
                    existingConnection.gameObject.SetActive(true);
                    result.Add(existingConnection);
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

                var connectionView = Builder.BuildView(entryView, oppositeEntryView);
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
        public AwaitableCall<List<EntryView>> GetAdjacentEntryViewsAsync(EntryView entryView, ICollection<ConnectionType> connectionTypes)
        {
            return new AwaitableCall<List<EntryView>>(() => GetAdjacentEntryViewsAsyncRoutine(entryView, connectionTypes), () => (List<EntryView>) _routineResult);
        }

        private IEnumerator GetAdjacentEntryViewsAsyncRoutine(EntryView entryView, ICollection<ConnectionType> connectionTypes)
        {
            var awaitableCall = GetConnectionViewsAsync(entryView, connectionTypes);
            yield return awaitableCall.Await();
            _routineResult = awaitableCall.Result.Select(c => c.Left == entryView ? c.Right : c.Left).ToList();
        }

        /// <summary>
        /// Same as <see cref="GetSearchResultViews", but awaitable./>
        /// </summary>
        public AwaitableCall<Queue<SearchResultView>> GetSearchResultViewsAsync(string searchQuery)
        {
            return new AwaitableCall<Queue<SearchResultView>>(() => GetSearchResultViewsAsyncRoutine(searchQuery), () => (Queue<SearchResultView>) _routineResult);
        }

        private IEnumerator GetSearchResultViewsAsyncRoutine(string searchQuery)
        {
            var asyncCall = new AsyncCall<Queue<SearchResult>, Queue<SearchResultView>>(() =>
                {
                    if (string.IsNullOrWhiteSpace(searchQuery))
                    {
                        return new Queue<SearchResult>();
                    }

                    return _provider.GetSearchResults(searchQuery);
                },
                searchResults =>
                {
                    var searchResultViews = new Queue<SearchResultView>();
                    while (searchResults.Any())
                    {
                        var searchResult = searchResults.Dequeue();
                        var searchResultView = BuildOrReuseSearchResultView(searchResult);
                        searchResultViews.Enqueue(searchResultView);
                    }

                    return searchResultViews;
                });

            yield return asyncCall.Execute();
            _routineResult = asyncCall.Result;
        }

        #endregion UnityAsync

        private SearchResultView BuildOrReuseSearchResultView(SearchResult searchResult)
        {
            SearchResultView searchResultView;
            if (_cachedSearchResultViews.ContainsKey(searchResult.EntryId))
            {
                searchResultView = _cachedSearchResultViews[searchResult.EntryId];
            }
            else
            {
                searchResultView = Builder.BuildView(searchResult);
                if (searchResultView == null)
                {
                    Logger.LogError($"Unable to build {nameof(SearchResultView)} for \"{searchResult}\".");
                    return null;
                }

                _cachedSearchResultViews.Add(searchResult.EntryId, searchResultView);
            }

            searchResultView.gameObject.SetActive(true);
            return searchResultView;
        }

        private EntryView BuildEntryView(Entry entry)
        {
            var entryView = Builder.BuildView(entry);
            if (entryView == null)
            {
                Logger.LogError($"Unable to build {nameof(EntryView)} for id \"{entry.Id}\".");
                return null;
            }

            _cachedEntryViews.Add(entry.Id, entryView);
            return entryView;
        }

        public void Unload(EntryView entryView)
        {
            //entryView.OnClicked -= FireCLickedEvent;
            entryView.gameObject.SetActive(false);

            foreach (var connection in entryView.Entry.Connections)
            {
                var identifier = ConnectionView.GetIdentifier(entryView.Entry.Id, connection.ConnectedId);
                if (_cachedConnectionViews.ContainsKey(identifier))
                {
                    Unload(_cachedConnectionViews[identifier]);
                }
            }
        }

        private void Unload(ConnectionView connectionView)
        {
            connectionView.gameObject.SetActive(false);
        }

        public void Unload(SearchResultView searchResultView)
        {
            if (_cachedSearchResultViews.ContainsKey(searchResultView.Result.EntryId))
            {
                searchResultView.gameObject.SetActive(false);
            }
            else
            {
                Object.DestroyImmediate(searchResultView.gameObject);
            }
        }
    }
}