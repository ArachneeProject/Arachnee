using System.Linq;
using Assets.Classes.EntryProviders;
using Assets.Classes.EntryProviders.Physical;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_GraphScene : MonoBehaviour
    {
        public Vertex moviePrefab;
        public Vertex artistPrefab;

        public Edge actorPrefab;
        public Edge directorPrefab;
        public Edge actorDirectorPrefab;
        
        void Start()
        {
            var testProvider = new MiniSampleProvider();
            var gameObjectProvider = new GameObjectProvider()
            {
                BiggerProvider = testProvider
            };

            gameObjectProvider.VertexPrefabs.Add(typeof (Movie), moviePrefab);
            gameObjectProvider.VertexPrefabs.Add(typeof(Artist), artistPrefab);
            
            gameObjectProvider.EdgePrefabs.Add(ConnectionFlags.Actor, actorPrefab);
            gameObjectProvider.EdgePrefabs.Add(ConnectionFlags.Director, directorPrefab);
            gameObjectProvider.EdgePrefabs.Add(ConnectionFlags.Actor | ConnectionFlags.Director, actorDirectorPrefab);

            for (int i = 0; i < 30; i++)
            {
                foreach (var entry in testProvider.Entries)
                {
                    Vertex v;
                    Debug.Assert(gameObjectProvider.TryGetVertex(entry.Id, out v), entry.Id + " not found.");
                    v.transform.position = Random.onUnitSphere * 3;

                    foreach (var edge in gameObjectProvider.GetEdges(entry, ConnectionFlags.All))
                    {
                        edge.transform.position = Random.onUnitSphere * 3;
                    }
                }
            }

            // checks
            Debug.Log("You should see two sphere (artists), one cube (movie), one long parallelepiped (actor)," +
                      " and one long ellipsoid forming a cross with a parallelepiped (director-actor). Nothing more.");

            var count = gameObjectProvider.GetAvailableVertices<Movie>().Count();
            Debug.Assert(count == 1, "Count was " + count);

            count = gameObjectProvider.GetAvailableVertices<Artist>().Count();
            Debug.Assert(count == 2, "Count was " + count);

            count = gameObjectProvider.GetAvailableVertices<Entry>().Count();
            Debug.Assert(count == 3, "Count was " + count);

            count = testProvider.Entries.SelectMany(e => gameObjectProvider.GetEdges(e, ConnectionFlags.All)).Distinct().Count();
            Debug.Assert(count == 2, "Count was " + count);
        }
    }
}
