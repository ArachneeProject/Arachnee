using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.CoreVisualization.Layouts;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;
using UnityEngine.UI;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization
{
    public class SearchEngine : IDisposable
    {
        private readonly InputField _inputField;
        private readonly ModelViewProvider _provider;
        private readonly GameObject _loadingFeedback;
        private readonly LayoutBase _layout;

        private readonly List<SearchResultView> _lastSearch = new List<SearchResultView>();
        
        public event Action<string> OnSearchResultSelected;

        public SearchEngine(InputField inputField, ModelViewProvider provider, GameObject loadingFeedback, LayoutBase layout)
        {
            _inputField = inputField;
            _provider = provider;
            _loadingFeedback = loadingFeedback;
            _layout = layout;
            
            inputField.onEndEdit.AddListener(RunSearch);
            _loadingFeedback.SetActive(false);
            _layout.Start();
            _layout.gameObject.SetActive(false);
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
            
            while (queue.Any())
            {
                var searchResultView = queue.Dequeue();
                searchResultView.OnClicked += OnSelectedResultView;

                _layout.Add(searchResultView.transform);
            }

            _layout.gameObject.SetActive(true);
            _loadingFeedback.SetActive(false);
        }

        private void OnSelectedResultView(SearchResultView searchresultview)
        {
            var selectedEntry = searchresultview.Result.EntryId;
            Logger.LogInfo($"Selected \"{searchresultview.Result.Name}\".");
            ClearSearch();
            OnSearchResultSelected?.Invoke(selectedEntry);
        }

        public void ClearSearch()
        {
            foreach (var searchResultView in _lastSearch)
            {
                searchResultView.OnClicked -= OnSelectedResultView;
                _provider.Unload(searchResultView);
            }

            _layout.gameObject.SetActive(false);
            _lastSearch.Clear();
        }

        public void Dispose()
        {
            if (_inputField == null)
            {
                return;
            }

            _inputField.onEndEdit.RemoveListener(RunSearch);
        }
    }
}