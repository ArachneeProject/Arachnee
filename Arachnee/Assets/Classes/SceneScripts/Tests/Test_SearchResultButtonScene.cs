using System.Linq;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Tests
{
    public class Test_SearchResultButtonScene : MonoBehaviour
    {
        public EntryView entryViewPrefab;
        public SearchResultButton searchResultButtonPrefab;

        private SearchResultButton _button;
        private ModelViewManager _manager;
        
        private void Start()
        {
            _manager = new ModelViewManager(new OnlineDatabase());
            _manager.SetPrefab(searchResultButtonPrefab);
            _manager.SetPrefab<Movie>(entryViewPrefab);

            // set up searchResultButton
            var queue = _manager.GetSearchResultViews("Terminator Judgment Day");

            _button = queue.Dequeue() as SearchResultButton;
            if (_button == null)
            {
                Debug.LogError($"{nameof(SearchResultButton)} is null.");
                return;
            }

            _button.OnClicked += LookAtEntryView;

            // destroy other results that we don't care of
            while (queue.Any())
            {
                Destroy(queue.Dequeue().gameObject);
            }

            Debug.Log("Clicking on the button should create a cube once, then clicking it again should only move the same cube (without creating another one).");
        }

        private void LookAtEntryView(SearchResultView searchresultview)
        {
            var view = _manager.GetEntryView(searchresultview.Result.EntryId);
            if (view == null)
            {
                return;
            }

            view.transform.position = Random.onUnitSphere * 3;
            Camera.main.transform.LookAt(view.transform.position);
        }
    }
}