using Assets.Classes.EntryProviders;
using Assets.Classes.EntryProviders.OnlineDatabase;
using Assets.Classes.GraphElements;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_LocalDbScene : MonoBehaviour
    {
        void Start()
        {
            var onlineDb = new OnlineDatabase();
            var localDb = new LocalDatabase {BiggerProvider = onlineDb};

            Entry e;
            Debug.Assert(localDb.TryGetEntry("Movie-218", out e), "Failed to find the entry.");

            var movie = e as Movie;
            if (movie == null)
            {
                Debug.LogError("Failed to cast entry as movie.");
                return;
            }

            Debug.Assert(movie.Title.Equals("The Terminator"), "Title is incorrect.");

            Debug.Assert(localDb.TrySave(movie), "Failed to save movie");

            localDb.BiggerProvider = null;
            Debug.Assert(localDb.TryGetEntry(movie.Id, out e), "Movie was not saved in the db.");
            movie = e as Movie;
            if (movie == null)
            {
                Debug.LogError("Failed to cast entry as movie.");
                return;
            }

            Debug.Log("If there is no error message, you're good to go.");
        }
    }
}