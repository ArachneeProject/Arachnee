using System.Collections;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.CoreVisualization.PhysicsEngine;
using Assets.Classes.SceneScripts.Controllers;
using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization
{
    public class Explorer
    {
        private readonly ModelViewProvider _provider;
        private readonly SearchEngine _searchEngine;
        private readonly ControllerBase _controller;
        private readonly GraphEngine _graphEngine;
        private readonly SidePanel _sidePanel;

        public Explorer(ModelViewProvider provider, SearchEngine searchEngine, ControllerBase controller,
            GraphEngine graphEngine, SidePanel sidePanel)
        {
            _provider = provider;
            _searchEngine = searchEngine;
            _controller = controller;
            _graphEngine = graphEngine;
            _sidePanel = sidePanel;

            _searchEngine.OnSearchResultSelected += OnSearchResultSelected;
            _provider.OnEntryViewSelected += OnEntrySelected;
            _provider.Builder.OnConnectionViewBuilt += OnConnectionBuilt;
            _sidePanel.Start();
        }

        private void OnConnectionBuilt(ConnectionView connectionView)
        {
            var left = connectionView.Left as RigidbodyImageTextEntryView;
            var right = connectionView.Right as RigidbodyImageTextEntryView;
            if (left == null || right == null)
            {
                Logger.LogWarning("Part of connection was null.");
                return;
            }

            _graphEngine.AddEdge(left.Rigidbody, right.Rigidbody);
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
            _sidePanel.OpenPanel(entryView.Entry);

            //yield return _provider.GetAdjacentEntryViewsAsync(entryView, Connection.AllTypes()).Await();
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