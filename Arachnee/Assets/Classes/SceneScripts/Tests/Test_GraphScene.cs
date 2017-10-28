using System.Linq;
using Assets.Classes.EntryProviders;
using Assets.Classes.EntryProviders.VisibleEntries;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_GraphScene : MonoBehaviour
    {
        public EntryView moviePrefab;
        public EntryView artistPrefab;

        public ConnectionView actorPrefab;
        public ConnectionView directorPrefab;
        public ConnectionView actorDirectorPrefab;
        
        void Start()
        {
            var testProvider = new MiniSampleProvider();
            var gameObjectProvider = new EntryViewProvider()
            {
                BiggerProvider = testProvider
            };

            gameObjectProvider.EntryViewPrefabs.Add(typeof (Movie), moviePrefab);
            gameObjectProvider.EntryViewPrefabs.Add(typeof(Artist), artistPrefab);
            
            gameObjectProvider.ConnectionViewPrefabs.Add(ConnectionType.Actor, actorPrefab);
            gameObjectProvider.ConnectionViewPrefabs.Add(ConnectionType.Director, directorPrefab);
            gameObjectProvider.ConnectionViewPrefabs.Add(ConnectionType.Actor | ConnectionType.Director, actorDirectorPrefab);

            for (int i = 0; i < 30; i++)
            {
                foreach (var entry in testProvider.Entries)
                {
                    EntryView v;
                    Debug.Assert(gameObjectProvider.TryGetEntryView(entry.Id, out v), entry.Id + " not found.");
                    v.transform.position = Random.onUnitSphere * 3;

                    foreach (var connectionView in gameObjectProvider.GetConnectionViews(entry, ConnectionType.Actor))
                    {
                        connectionView.transform.position = Random.onUnitSphere * 3;
                    }
                }
            }

            // checks
            Debug.Log("You should see two sphere (artists), one cube (movie), one long parallelepiped (actor)," +
                      " and one long ellipsoid forming a cross with a parallelepiped (director-actor). Nothing more.");

            var count = gameObjectProvider.GetAvailableEntryViews<Movie>().Count();
            Debug.Assert(count == 1, "Count was " + count);

            count = gameObjectProvider.GetAvailableEntryViews<Artist>().Count();
            Debug.Assert(count == 2, "Count was " + count);

            count = gameObjectProvider.GetAvailableEntryViews<Entry>().Count();
            Debug.Assert(count == 3, "Count was " + count);

            count = testProvider.Entries.SelectMany(e => gameObjectProvider.GetConnectionViews(e, ConnectionType.Actor)).Distinct().Count();
            Debug.Assert(count == 2, "Count was " + count);
        }
    }
}
