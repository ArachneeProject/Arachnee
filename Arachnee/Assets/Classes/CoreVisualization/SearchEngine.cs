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
    public class SearchEngine : MonoBehaviour
    {
        public InputField inputField;
        public LoadingFeedback loadingFeedback;
        public LayoutBase layout;

        private readonly List<SearchResultView> _lastSearch = new List<SearchResultView>();
        
        private ModelViewProvider _provider;

        public ModelViewProvider Provider
        {
            get { return _provider; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                if (_provider != null)
                {
                    throw new ArgumentException("Provider already set.");
                }

                _provider = value;
            }
        }

        public event Action<string> OnSearchResultSelected;

        public void Start()
        {
            if (_provider == null)
            {
                Logger.LogError("Provider is not set.");
                return;
            }

            inputField.onEndEdit.RemoveListener(RunSearch);
            inputField.onEndEdit.AddListener(RunSearch);

            this.loadingFeedback.Start();
            this.layout.Start();
            this.layout.gameObject.SetActive(false);
        }
        
        private void RunSearch(string searchQuery)
        {
            inputField.StartCoroutine(RunSearchRoutine(searchQuery));
        }
        
        private void ClearSearch()
        {
            foreach (var searchResultView in _lastSearch)
            {
                searchResultView.OnClicked -= OnSelectedResultView;
                Provider.Unload(searchResultView);
            }

            layout.gameObject.SetActive(false);
            _lastSearch.Clear();
        }

        private IEnumerator RunSearchRoutine(string searchQuery)
        {
            ClearSearch();

            Logger.LogInfo($"Searching for \"{searchQuery}\"...");
            loadingFeedback.StartLoading();

            var awaitableQueue = Provider.GetSearchResultViewsAsync(searchQuery);
            yield return awaitableQueue.Await();
            var queue = awaitableQueue.Result;

            if (!queue.Any())
            {
                Logger.LogInfo($"No result for \"{searchQuery}\".");
                loadingFeedback.StopLoading();
                yield break;
            }
            
            _lastSearch.AddRange(queue);
            Logger.LogInfo($"{queue.Count} results for \"{searchQuery}\".");
            
            while (queue.Any())
            {
                var searchResultView = queue.Dequeue();
                searchResultView.OnClicked += OnSelectedResultView;

                layout.Add(searchResultView.transform);
            }

            layout.gameObject.SetActive(true);
            loadingFeedback.StopLoading();
        }

        private void OnSelectedResultView(SearchResultView searchresultview)
        {
            var selectedEntry = searchresultview.Result.EntryId;
            Logger.LogInfo($"Selected \"{searchresultview.Result.Name}\".");
            ClearSearch();
            OnSearchResultSelected?.Invoke(selectedEntry);
        }
        
        void OnDestroy()
        {
            if (inputField == null)
            {
                return;
            }

            inputField.onEndEdit.RemoveListener(RunSearch);
        }
    }
}