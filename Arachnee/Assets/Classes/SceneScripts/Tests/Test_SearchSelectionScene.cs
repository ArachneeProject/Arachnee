﻿using System.Collections.Generic;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_SearchSelectionScene : MonoBehaviour
    {
        public EntryView entryViewPrefab;

        public InputField input;
        public Text clickedEntryViewName;
        public Button validateButton;

        private ModelViewManager _manager;

        private EntryView _selectedEntryView;
        private readonly List<EntryView> _searchResults = new List<EntryView>();

        void Start () 
        {
            _manager = new ModelViewManager(new OnlineDatabase());

            _manager.SetPrefab<Movie>(entryViewPrefab);
            _manager.SetPrefab<Artist>(entryViewPrefab);
            
            clickedEntryViewName.gameObject.SetActive(false);
            validateButton.gameObject.SetActive(false);
        }
    
        public void RunSearch()
        {
            // clear previous search
            clickedEntryViewName.gameObject.SetActive(false);
            validateButton.gameObject.SetActive(false);

            _selectedEntryView = null;
            foreach (var searchResult in _searchResults)
            {
                searchResult.OnClicked -= UpdateClickedEntryView;
                DestroyImmediate(searchResult.gameObject); // I know what I'm doing here
            }
            _searchResults.Clear();

            // run search
            var results = _manager.GetSearchResultViews(input.text);
            Debug.Log(results.Count + " results for " + input.text);



            // update search results
            /*
            while (results.Count > 0)
            {
                var result = results.Dequeue();
                result.OnClicked += UpdateClickedEntryView;
                result.transform.position = Random.onUnitSphere*2;
                _searchResults.Add(result);
            }
            */
        }

        private void UpdateClickedEntryView(EntryView entryView)
        {
            _selectedEntryView = entryView;

            clickedEntryViewName.gameObject.SetActive(true);
            clickedEntryViewName.text = entryView.ToString();

            validateButton.gameObject.SetActive(true);
        }
    
        public void Validate()
        {
            var particle = _selectedEntryView.gameObject.GetComponent<ParticleSystem>();
            particle.Emit(10);

            validateButton.gameObject.SetActive(false);
            clickedEntryViewName.gameObject.SetActive(false);
        }
    }
}
