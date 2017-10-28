using System.Linq;
using Assets.Classes.Core.EntryProviders;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.EntryViewProviders;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.CoreVisualization.PhysicsEngine;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_LoadEngineScene : MonoBehaviour
    {
        public EntryView EntryViewPrefab;
        public ConnectionView ConnectionViewPrefab;
        public GraphEngine graphEngine;

        void Start ()
        {
            // sets up provider
            var sample = new MiniSampleProvider();
            var provider = new EntryViewProvider()
            {
                BiggerProvider = sample
            };
            provider.EntryViewPrefabs.Add(typeof(Movie), EntryViewPrefab);
            provider.EntryViewPrefabs.Add(typeof(Artist), EntryViewPrefab);
            provider.ConnectionViewPrefabs.Add(ConnectionType.Actor, ConnectionViewPrefab);
            provider.ConnectionViewPrefabs.Add(ConnectionType.Director, ConnectionViewPrefab);
            provider.ConnectionViewPrefabs.Add(ConnectionType.Director | ConnectionType.Actor, ConnectionViewPrefab);

            // load graph engine
            foreach (var entry in sample.Entries)
            {
                EntryView v;
                Debug.Assert(provider.TryGetEntryView(entry.Id, out v), "Failed to set up provider.");
                v.transform.position = UnityEngine.Random.onUnitSphere*5;
            
                graphEngine.Add(v);
            
                foreach (var connectionView in provider.GetConnectionViews(v.Entry, ConnectionType.Actor))
                {
                    graphEngine.Add(connectionView);
                }
            }
        }
    }
}
