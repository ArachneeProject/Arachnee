using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;
using UnityEngine.UI;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization
{
    public class SearchEngineView : IDisposable
    {
        private readonly InputField _inputField;
        private readonly ModelViewProvider _provider;
        private readonly GameObject _loadingFeedback;
        
        private readonly List<SearchResultView> _lastSearch = new List<SearchResultView>();

        public event EventHandler<string> OnSelectedEntry;

        public SearchEngineView(InputField inputField, ModelViewProvider provider, GameObject loadingFeedback)
        {
            _inputField = inputField;
            _provider = provider;
            _loadingFeedback = loadingFeedback;

            inputField.onEndEdit.AddListener(RunSearch);
            _loadingFeedback.SetActive(false);
        }

        private void RunSearch(string searchQuery)
        {
            _inputField.StartCoroutine(RunSearchRoutine(searchQuery));
        }

        private IEnumerator RunSearchRoutine(string searchQuery)
        {
            ClearSearch();

            Logger.LogInfo($"Searching for \"{searchQuery}\"...");
            _loadingFeedback.SetActive(true);

            var awaitableQueue = _provider.GetSearchResultViewsAsync(searchQuery);
            yield return awaitableQueue.Await();
            var queue = awaitableQueue.Result;

            if (!queue.Any())
            {
                Logger.LogInfo($"No result for \"{searchQuery}\".");
                _loadingFeedback.SetActive(false);
                yield break;
            }
            
            _lastSearch.AddRange(queue);
            Logger.LogInfo($"{queue.Count} results for \"{searchQuery}\".");

            int i = 0;
            while (queue.Any())
            {
                var searchResultView = queue.Dequeue();
                searchResultView.OnClicked += OnSelectedResultView;
            }

            _loadingFeedback.SetActive(false);
        }

        private void OnSelectedResultView(SearchResultView searchresultview)
        {
            var selectedEntry = searchresultview.Result.EntryId;
            ClearSearch();
            OnSelectedEntry?.Invoke(this, selectedEntry);
        }

        private void ClearSearch()
        {
            foreach (var searchResultView in _lastSearch)
            {
                searchResultView.OnClicked -= OnSelectedResultView;
                _provider.Unload(searchResultView);
            }

            _lastSearch.Clear();
        }

        public void Dispose()
        {
            ClearSearch();
            _inputField.onEndEdit.RemoveListener(RunSearch);
        }
    }
}