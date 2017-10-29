using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Core.EntryProviders;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_GraphScene : MonoBehaviour
    {
        public EntryView movieViewPrefab;
        public EntryView artistViewPrefab;

        public ConnectionView connectionViewPrefab;
        
        void Start()
        {
            var testProvider = new MiniSampleProvider();
            var manager = new ModelViewManager(testProvider);

            manager.SetPrefab<Movie>(movieViewPrefab);
            manager.SetPrefab<Artist>(artistViewPrefab);
            manager.SetPrefab(connectionViewPrefab);

            var entryViewSet = new HashSet<EntryView>();
            
            for (int i = 0; i < 100; i++)
            {
                foreach (var entryId in testProvider.Entries.Select(e => e.Id))
                {
                    var entryView = manager.GetEntryView(entryId);
                    entryView.transform.position = Random.onUnitSphere * 5;

                    entryViewSet.Add(entryView);
                }
            }

            var connectionViewSet = new HashSet<ConnectionView>();

            for (int i = 0; i < 100; i++)
            {
                foreach (var entryView in entryViewSet)
                {
                    var connectionViews = manager.GetConnectionViews(entryView);

                    foreach (var connectionView in connectionViews)
                    {
                        connectionView.transform.position = Random.onUnitSphere * 5;
                        connectionViewSet.Add(connectionView);
                    }
                }
            }

            // checks
            Debug.Log("You should see one cube (movie), two sphere (artist), and two ellipsoid (artist-movie connections).");
            Debug.Assert(entryViewSet.Count == 3, $"Count was {entryViewSet.Count}");
            Debug.Assert(connectionViewSet.Count == 2, $"Count was {connectionViewSet.Count}");
        }
    }
}
