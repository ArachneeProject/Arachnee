using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.EntryProviders;
using Assets.Classes.EntryProviders.VisibleEntries;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using UnityEngine;

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

        var expected = sample.GetAvailableEntries<Entry>().Count();
        var actual = provider.GetAvailableEntries<Entry>().Count();
        Debug.Assert(expected == actual, "Expected " + expected + " elements, got " + actual);
    }
}
