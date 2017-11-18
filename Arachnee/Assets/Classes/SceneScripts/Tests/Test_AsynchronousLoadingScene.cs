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
            
            var prov = new ModelViewProvider(new OnlineDatabase(), builder);

            yield return new WaitForEndOfFrame();

            // get seed entry
            var asyncCall = prov.GetEntryViewAsync("Movie-218");
            yield return asyncCall.Execute();

            asyncCall.Result.gameObject.transform.position = Vector3.zero;
            
            /*
            // get adjacent entries
            var connectedEntryViews = prov.GetConnectedEntryViews(entryView.Result);
            
            foreach (var connectedEntryView in connectedEntryViews)
            {
                yield return new WaitForEndOfFrame();
                connectedEntryView.transform.position = Random.onUnitSphere * 8;
            }
            */

            loadingObject.SetActive(false);
        }
    }
}