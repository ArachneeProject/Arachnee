using System.Collections;
using System.Collections.Generic;
using Assets.Classes.EntryProviders;
using Assets.Classes.EntryProviders.Physical;
using Assets.Classes.GraphElements;
using Assets.Classes.PhysicsEngine;
using UnityEngine;

public class Test_LoadEngineScene : MonoBehaviour
{
    public GraphEngine graphEngine;

    void Start ()
    {
        // sets up provider
        var sample = new MiniSampleProvider();
        var provider = new PhysicalProvider
        {
            BiggerProvider = sample
        };

        // load graph engine
        foreach (var entry in sample.Entries)
        {
            PhysicalEntry e;
            Debug.Assert(provider.TryGetPhysicalEntry(entry.Id, out e), "Failed to set up provider.");


        }

        
        
    }
}
