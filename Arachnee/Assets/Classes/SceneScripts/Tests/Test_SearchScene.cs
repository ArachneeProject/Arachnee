using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.EntryProviders.OnlineDatabase;
using Assets.Classes.EntryProviders.VisibleEntries;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_SearchScene : MonoBehaviour
    {
        public InputField input;

        public EntryView EntryViewPrefab;
        public ConnectionView ConnectionViewPrefab;
        
        private EntryViewProvider _entryViewProvider;
        
        void Start ()
        {
            _entryViewProvider = new EntryViewProvider
            {
                BiggerProvider = new OnlineDatabase()
            };

            _entryViewProvider.EntryViewPrefabs.Add(typeof (Movie), EntryViewPrefab);
            _entryViewProvider.EntryViewPrefabs.Add(typeof(Artist), EntryViewPrefab);

            _entryViewProvider.ConnectionViewPrefabs.Add(ConnectionType.Actor, ConnectionViewPrefab);
            _entryViewProvider.ConnectionViewPrefabs.Add(ConnectionType.Director, ConnectionViewPrefab);

            Debug.Log("Try to type \"the terminator\" for example.");
        }
        
        public void Go()
        {
            Debug.Log("Searching for " + input.text);
            var res = _entryViewProvider.GetEntryViewResults<Entry>(input.text);
            if (res.Any())
            {
                var best = res.Dequeue();
                Debug.Log("Best is " + best + " (among " + res.Count + " other results)");
                
                best.transform.position = UnityEngine.Random.onUnitSphere*3;
                Camera.main.transform.LookAt(best.transform.position);
                
                return;
            }

            Debug.Log("No result.");
        }
    }
}
