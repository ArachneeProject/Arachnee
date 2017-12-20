using System.Linq;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.SceneScripts.Scenes.Tests
{
    public class Test_SearchScene : MonoBehaviour
    {
        public InputField inputField;
        
        public SearchResultView searchResultViewPrefab;

        private ModelViewProvider _provider;

        private float _searchCounter = -5; // used to shift each search

        void Start ()
        {
            var builder = new ModelViewBuilder();
            builder.SetPrefab(searchResultViewPrefab);

            _provider = new ModelViewProvider(new OnlineDatabase(), builder);
            
            Debug.Log("Try to type \"the terminator\" for example.");
        }
        
        public void Go()
        {
            Debug.Log("Searching for " + inputField.text);
            var queue = _provider.GetSearchResultViews(inputField.text);
            if (!queue.Any())
            {
                Debug.Log("No result.");
                return;
            }

            _searchCounter += 5;
            
            var bestRresultView = queue.Dequeue();
            Debug.Log("Best is " + bestRresultView + " (among " + queue.Count + " other results)");

            bestRresultView.transform.position = new Vector3(_searchCounter, 0, 0);
            Camera.main.transform.LookAt(bestRresultView.transform.position);

            float i = -2; // used to shift each search result
            while (queue.Any())
            {
                queue.Dequeue().transform.position = new Vector3(_searchCounter, i, 0);
                i -= 2;
            }
        }
    }
}
