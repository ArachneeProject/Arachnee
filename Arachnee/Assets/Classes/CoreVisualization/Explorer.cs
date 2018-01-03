using System.Collections;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.CoreVisualization.PhysicsEngine;
using Assets.Classes.SceneScripts.Controllers;
using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization
{
    public class Explorer : MonoBehaviour
    {
        public SearchEngine searchEngine;
        public ControllerBase controller;
        public GraphEngine graphEngine;
        public SidePanel sidePanel;

        private Entry _lastSelected;

        private ModelViewProvider _provider;
        
        public void Start()
        {
            _provider = searchEngine.Provider;

            this.searchEngine.OnSearchResultSelected -= OnSearchResultSelected;
            this.searchEngine.OnSearchResultSelected += OnSearchResultSelected;
            _provider.OnEntryViewSelected -= OnEntrySelected;
            _provider.OnEntryViewSelected += OnEntrySelected;
            _provider.Builder.OnConnectionViewBuilt -= OnConnectionBuilt;
            _provider.Builder.OnConnectionViewBuilt += OnConnectionBuilt;

            this.sidePanel.Start();
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

            graphEngine.AddEdge(left.Rigidbody, right.Rigidbody);
        }

        private void OnEntrySelected(EntryView entryView)
        {
            if (entryView?.Entry == null || entryView.Entry == DefaultEntry.Instance)
            {
                return;
            }

            _lastSelected = entryView.Entry;
            controller.StartCoroutine(FocusOnEntryRoutine(entryView));
        }

        private void OnSearchResultSelected(string entryId)
        {
            controller.StartCoroutine(LoadAndFocusOnEntryRoutine(entryId));
        }

        private IEnumerator LoadAndFocusOnEntryRoutine(string entryId)
        {
            // load entry
            var awaitableEntryView = _provider.GetEntryViewAsync(entryId);
            yield return awaitableEntryView.Await();
            var entryView = awaitableEntryView.Result;
            if (entryView == null)
            {
                yield break;
            }

            // connect to last selected entry
            if (_lastSelected != null)
            {
                
            }

            _lastSelected = entryView.Entry;

            // focus on entry
            yield return FocusOnEntryRoutine(entryView);
            sidePanel.OpenPanel(entryView.Entry);
        }

        private IEnumerator FocusOnEntryRoutine(EntryView e)
        {
            var entryView = e as RigidbodyImageTextEntryView;
            if (entryView == null)
            {
                yield break;
            }

            graphEngine.AddRigidbody(entryView.Rigidbody);

            Quaternion fromRotation = controller.transform.rotation;
            
            float time = 0;
            while (time < 1)
            {
                time += Time.deltaTime;
                Quaternion toRotation = Quaternion.LookRotation(entryView.transform.position - controller.transform.position);
                controller.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, time);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}