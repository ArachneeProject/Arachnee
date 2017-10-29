using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_SearchScene : MonoBehaviour
    {
        public InputField input;

        public EntryView EntryViewPrefab;
        public ConnectionView ConnectionViewPrefab;
        
        private ModelViewManager _manager;
        
        void Start ()
        {
            _manager = new ModelViewManager(new OnlineDatabase());

            _manager.SetPrefab<Movie>(EntryViewPrefab);
            _manager.SetPrefab<Artist>(EntryViewPrefab);

            _manager.SetPrefab(ConnectionViewPrefab);
            
            Debug.Log("Try to type \"the terminator\" for example.");
        }
        
        public void Go()
        {
            Debug.Log("Searching for " + input.text);
            var res = _manager.GetSearchResultViews(input.text);
            if (!res.Any())
            {
                Debug.Log("No result.");
                return;
            }
            
            var bestRes = res.Dequeue();
            Debug.Log("Best is " + bestRes + " (among " + res.Count + " other results)");

            var best = _manager.GetEntryView(bestRes.Result.EntryId);
            best.transform.position = UnityEngine.Random.onUnitSphere * 3;
            Camera.main.transform.LookAt(best.transform.position);
        }
    }
}
