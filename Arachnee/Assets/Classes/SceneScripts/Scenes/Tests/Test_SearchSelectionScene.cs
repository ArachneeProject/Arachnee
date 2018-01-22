using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.SceneScripts.Scenes.Tests
{
    public class Test_SearchSelectionScene : MonoBehaviour
    {
        public EntryView movieViewPrefab;
        public EntryView artistViewPrefab;
        public EntryView serieViewPrefab;
        public SearchResultView searchResultViewPrefab;

        public InputField input;
        public Text clickedEntryViewLabel;
        public Button validateButton;
        
        private ModelViewProvider _provider;

        private SearchResultView _selectedResultView;
        private readonly List<SearchResultView> _searchResults = new List<SearchResultView>();

        void Start () 
        {
            var builder = new ModelViewBuilder();
            builder.SetPrefab<Movie>(movieViewPrefab);
            builder.SetPrefab<Artist>(artistViewPrefab);
            builder.SetPrefab<TvSeries>(serieViewPrefab);
            builder.SetPrefab(searchResultViewPrefab);

            _provider = new ModelViewProvider(new OnlineDatabase(), builder);
            
            Clear();

            Debug.Log("Try to write \"Jackie Chan\". " +
                      "Then click on one of the results, and validate. " +
                      "The corresponding prefab should then be displayed: " +
                      "cube for a movie, capsule for a serie) or sphere for an artist.");
        }
        
        public void RunSearch()
        {
            Clear();

            // run search
            var results = _provider.GetSearchResultViews(input.text);
            Debug.Log(results.Count + " results for " + input.text);
            
            // set up search results
            while (results.Any())
            {
                var result = results.Dequeue();
                result.OnClicked += UpdateSelectedResultView;
                result.transform.position = Random.onUnitSphere * 2;
                _searchResults.Add(result);
            }
        }

        private void Clear()
        {
            clickedEntryViewLabel.gameObject.SetActive(false);
            validateButton.gameObject.SetActive(false);
            _selectedResultView = null;

            foreach (var searchResult in _searchResults)
            {
                searchResult.OnClicked -= UpdateSelectedResultView;
                DestroyImmediate(searchResult.gameObject); // I know what I'm doing here
            }

            _searchResults.Clear();
        }

        private void UpdateSelectedResultView(SearchResultView resultView)
        {
            _selectedResultView = resultView;

            clickedEntryViewLabel.gameObject.SetActive(true);
            clickedEntryViewLabel.text = resultView.Result.Name;

            validateButton.gameObject.SetActive(true);
        }
    
        public void Validate()
        {
            var entry = _provider.GetEntryView(_selectedResultView.Result.EntryId);
            if (entry != null)
            {
                entry.transform.position = Random.onUnitSphere * 2;
            }

            Clear();
        }
    }
}
