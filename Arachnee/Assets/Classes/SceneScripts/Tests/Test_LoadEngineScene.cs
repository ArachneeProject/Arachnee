using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.EntryProviders;
using Assets.Classes.EntryProviders.Physical;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using UnityEngine;

public class Test_LoadEngineScene : MonoBehaviour
{
    public Vertex vertexPrefab;
    public Edge edgePrefab;
    public GraphEngine graphEngine;

    void Start ()
    {
        // sets up provider
        var sample = new MiniSampleProvider();
        var provider = new GameObjectProvider()
        {
            BiggerProvider = sample
        };
        provider.VertexPrefabs.Add(typeof(Movie), vertexPrefab);
        provider.VertexPrefabs.Add(typeof(Artist), vertexPrefab);
        provider.EdgePrefabs.Add(ConnectionFlags.Actor, edgePrefab);
        provider.EdgePrefabs.Add(ConnectionFlags.Director, edgePrefab);
        provider.EdgePrefabs.Add(ConnectionFlags.Director | ConnectionFlags.Actor, edgePrefab);

        // load graph engine
        foreach (var entry in sample.Entries)
        {
            Vertex v;
            Debug.Assert(provider.TryGetVertex(entry.Id, out v), "Failed to set up provider.");
            v.transform.position = UnityEngine.Random.onUnitSphere*5;
            
            graphEngine.Add(v);
            
            var edges = provider.GetEdges(v.Entry, ConnectionFlags.All);
            foreach (var edge in edges)
            {
                graphEngine.Add(edge);
            }
        }

        var expected = sample.GetAvailableEntries<Entry>().Count();
        var actual = provider.GetAvailableEntries<Entry>().Count();
        Debug.Assert(expected == actual, "Expected " + expected + " elements, got " + actual);
    }
}
