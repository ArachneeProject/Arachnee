using System.Linq;
using Assets.Classes.EntryProviders;
using Assets.Classes.EntryProviders.Physical;
using Assets.Classes.GraphElements;
using UnityEngine;

namespace Assets.Classes.Scripts.Tests
{
    public class GraphTestScene : MonoBehaviour
    {
        public GameObject moviePrefab;
        public GameObject artistPrefab;

        public GameObject actorPrefab;
        public GameObject directorPrefab;
        public GameObject actorDirectorPrefab;
        
        // Use this for initialization
        void Start()
        {
            var testProvider = new TestSampleProvider();
            var graph = new PhysicalGraph
            {
                BiggerProvider = testProvider
            };
            graph.EntryPrefabs.Add(typeof (Movie), moviePrefab);
            graph.EntryPrefabs.Add(typeof (Artist), artistPrefab);
            
            graph.ConnectionPrefabs.Add(ConnectionFlags.Actor, actorPrefab);
            graph.ConnectionPrefabs.Add(ConnectionFlags.Director, directorPrefab);
            graph.ConnectionPrefabs.Add(ConnectionFlags.Actor | ConnectionFlags.Director, actorDirectorPrefab);

            for (int i = 0; i < 10; i++)
            {
                foreach (var entry in testProvider.Entries)
                {
                    PhysicalEntry p;
                    if (!graph.TryGetPhysicalEntry(entry.Id, out p))
                    {
                        Debug.LogError("Failed.");
                    }

                    p.GameObject.transform.position = Random.onUnitSphere * 3;
                }

                foreach (var pCon in graph.GetAvailablePhysicalConnections(ConnectionFlags.All)
                                          .Where(p => testProvider.Entries.FirstOrDefault(e => p.Contains(e.Id)) != null))
                {
                    pCon.GameObject.transform.position = Random.onUnitSphere * 3;
                }
            }

            // checks
            Debug.Log("You should ONLY see two sphere (artists), one cube (movie), one long parallelepiped (actor)," +
                      " and one long ellipsoid forming a cross with a parallelepiped (director-actor).");

            var count = graph.GetAvailablePhysicalEntries<Movie>().Count();
            Debug.Assert(count == 1, "Count was " + count);

            count = graph.GetAvailablePhysicalEntries<Artist>().Count();
            Debug.Assert(count == 2, "Count was " + count);

            count = graph.GetAvailablePhysicalEntries<Entry>().Count();
            Debug.Assert(count == 3, "Count was " + count);

            count = graph.GetAvailablePhysicalConnections(ConnectionFlags.All).Count();
            Debug.Assert(count == 2, "Count was " + count);
        }
    }
}
