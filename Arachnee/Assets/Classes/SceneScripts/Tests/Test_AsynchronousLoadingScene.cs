using System.Collections;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_AsynchronousLoadingScene : MonoBehaviour
    {
        public GameObject loadingObject;
        public ImageTextEntryView imageTextEntryViewPrefab;
        public LineConnectionView lineConnectionViewPrefab;

        void Start()
        {
            StartCoroutine(SceneLoading());
        }
        
        private IEnumerator SceneLoading()
        {
            // set up
            loadingObject.SetActive(true);

            var builder = new ModelViewBuilder();
            builder.SetPrefab<Movie>(imageTextEntryViewPrefab);
            builder.SetPrefab<Artist>(imageTextEntryViewPrefab);
            builder.SetPrefab<Serie>(imageTextEntryViewPrefab);
            builder.SetPrefab(lineConnectionViewPrefab);
            
            var provider = new ModelViewProvider(new OnlineDatabase(), builder);

            yield return new WaitForEndOfFrame();

            // get seed entry
            var awaitableEntryView = provider.GetEntryViewAsync("Movie-218");
            yield return awaitableEntryView.Await();

            var entryView = awaitableEntryView.Result;
            entryView.gameObject.transform.position = Vector3.zero;
            
            // get adjacent entries
            var awaitableList = provider.GetAdjacentEntryViewsAsync(entryView, Connection.AllTypes());
            yield return awaitableList.Await();
            var connectedEntryViews = awaitableList.Result;

            foreach (var connectedEntryView in connectedEntryViews)
            {
                yield return new WaitForEndOfFrame();
                connectedEntryView.transform.position = Random.onUnitSphere * 8;
            }
            
            loadingObject.SetActive(false);
        }
    }
}