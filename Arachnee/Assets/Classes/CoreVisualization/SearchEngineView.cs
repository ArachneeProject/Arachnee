using System;
using System.Collections;
using System.Linq;
using Assets.Classes.CoreVisualization.ModelViewManagement;
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

            Logger.LogInfo($"{queue.Count} results for \"{searchQuery}\".");
            _loadingFeedback.SetActive(false);
        }

        public void Dispose()
        {
            _inputField.onEndEdit.RemoveListener(RunSearch);
        }
    }
}