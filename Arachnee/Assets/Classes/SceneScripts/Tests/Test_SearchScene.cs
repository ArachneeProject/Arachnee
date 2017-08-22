using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.EntryProviders.OnlineDatabase;
using Assets.Classes.EntryProviders.Physical;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_SearchScene : MonoBehaviour
    {
        public InputField input;

        public Vertex vertexPrefab;
        public Edge edgePrefab;
        
        private GameObjectProvider _gameObjectProvider;
        
        void Start ()
        {
            _gameObjectProvider = new GameObjectProvider
            {
                BiggerProvider = new OnlineDatabase()
            };

            _gameObjectProvider.VertexPrefabs.Add(typeof (Movie), vertexPrefab);
            _gameObjectProvider.VertexPrefabs.Add(typeof(Artist), vertexPrefab);

            _gameObjectProvider.EdgePrefabs.Add(ConnectionFlags.Actor, edgePrefab);
            _gameObjectProvider.EdgePrefabs.Add(ConnectionFlags.Director, edgePrefab);

            Debug.Log("Try to type \"the terminator\" for example.");
        }
        
        public void Go()
        {
            Debug.Log("Searching for " + input.text);
            var res = _gameObjectProvider.GetVerticesResults<Entry>(input.text);
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
