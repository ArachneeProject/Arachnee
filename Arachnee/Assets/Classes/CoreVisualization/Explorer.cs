using System.Collections;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.CoreVisualization.PhysicsEngine;
using Assets.Classes.SceneScripts.Controllers;
using UnityEngine;

namespace Assets.Classes.CoreVisualization
{
    public class Explorer
    {
        private readonly ModelViewProvider _provider;
        private readonly SearchEngine _searchEngine;
        private readonly ControllerBase _controller;
        private readonly GraphEngine _graphEngine;

        public Explorer(ModelViewProvider provider, SearchEngine searchEngine, ControllerBase controller, GraphEngine graphEngine)
        {
            _provider = provider;
            _searchEngine = searchEngine;
            _controller = controller;
            _graphEngine = graphEngine;

            _searchEngine.OnSelectedEntry += OnSelectedSearch;
        }

        private void OnSelectedSearch(object sender, string e)
        {
            _controller.StartCoroutine(FocusOnEntry(e));
        }

        private IEnumerator FocusOnEntry(string entryId)
        {
            var awaitableEntryView = _provider.GetEntryViewAsync(entryId);
            yield return awaitableEntryView.Await();
            var entryView = awaitableEntryView.Result as RigidbodyImageTextEntryView;
            if (entryView == null)
            {
                yield break;
            }
            
            _graphEngine.AddRigidbody(entryView.Rigidbody);

            Quaternion fromRotation = _controller.transform.rotation;
            Quaternion toRotation = Quaternion.LookRotation(entryView.transform.position - _controller.transform.position);

            float time = 0;
            while (time < 1)
            {
                time += Time.deltaTime;

                _controller.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, time);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}