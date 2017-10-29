using System.Linq;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_SearchScene : MonoBehaviour
    {
        public InputField inputField;
        
        public SearchResultView searchResultViewPrefab;

        private ModelViewManager _manager;

        private float _searchCounter = -5; // used to shift each search

        void Start ()
        {
            _manager = new ModelViewManager(new OnlineDatabase());
            _manager.SetPrefab(searchResultViewPrefab);
            
            Debug.Log("Try to type \"the terminator\" for example.");
        }
        
        public void Go()
        {
            Debug.Log("Searching for " + inputField.text);
            var queue = _manager.GetSearchResultViews(inputField.text);
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
