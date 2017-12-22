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

        public Explorer(ModelViewProvider provider, SearchEngine searchEngine, ControllerBase controller,
            GraphEngine graphEngine)
        {
            _provider = provider;
            _searchEngine = searchEngine;
            _controller = controller;
            _graphEngine = graphEngine;

            _searchEngine.OnSearchResultSelected += OnSearchResultSelected;
            _provider.OnEntryViewSelected += OnEntrySelected;
        }

        private void OnEntrySelected(EntryView entryView)
        {
            _controller.StartCoroutine(FocusOnEntryRoutine(entryView));
        }

        private void OnSearchResultSelected(string entryId)
        {
            _controller.StartCoroutine(LoadAndFocusOnEntryRoutine(entryId));
        }

        private IEnumerator LoadAndFocusOnEntryRoutine(string entryId)
        {
            var awaitableEntryView = _provider.GetEntryViewAsync(entryId);
            yield return awaitableEntryView.Await();
            var entryView = awaitableEntryView.Result;
            if (entryView == null)
            {
                yield break;
            }

            yield return FocusOnEntryRoutine(entryView);
        }

        private IEnumerator FocusOnEntryRoutine(EntryView e)
        {
            var entryView = e as RigidbodyImageTextEntryView;
            if (entryView == null)
            {
                yield break;
            }

            _graphEngine.AddRigidbody(entryView.Rigidbody);

            Quaternion fromRotation = _controller.transform.rotation;
            
            float time = 0;
            while (time < 1)
            {
                time += Time.deltaTime;
                Quaternion toRotation = Quaternion.LookRotation(entryView.transform.position - _controller.transform.position);
                _controller.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, time);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}