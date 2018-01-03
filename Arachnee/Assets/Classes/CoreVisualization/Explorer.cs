using System.Collections;
using System.IO;
using Assets.Classes.Core.Graph;
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

        private EntryView _lastSelected;

        private ModelViewProvider _provider;

        private CompactGraph _graph;

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
            
            string graphPath = Path.Combine(Application.dataPath, "Database", "graph.spdr");
            if (!File.Exists(graphPath))
            {
                Logger.LogError($"Grpah not found at \"{graphPath}\".");
                return;
            }

            _graph = CompactGraph.InitializeFrom(graphPath, Connection.AllTypes());
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

            _lastSelected = entryView;
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
            
            // focus on entry
            yield return FocusOnEntryRoutine(entryView);
            
            // connect to last selected entry
            if (_lastSelected != null)
            {
                yield return LoadPathTo(entryView);
            }

            _lastSelected = entryView;
        }

        private IEnumerator LoadPathTo(EntryView entryView)
        {
            var steps = _graph.GetShortestPath(entryView.Entry.Id, _lastSelected.Entry.Id);

            if (steps.Count < 2)
            {
                yield break;
            }

            EntryView currentEntryView = entryView;
            for (int i = 1; i < steps.Count; i++)
            {
                var next = steps[i];
                
                var awaitableNextEntryView = _provider.GetEntryViewAsync(next);
                yield return awaitableNextEntryView.Await();

                var nextEntryView = awaitableNextEntryView.Result;
                if (nextEntryView == null)
                {
                    yield break;
                }

                var awaitableConnectionViews = _provider.GetConnectionViewsAsync(currentEntryView, Connection.AllTypes(), nextEntryView);
                yield return awaitableConnectionViews.Await();
                var connectionView = awaitableConnectionViews.Result;
                if (connectionView == null)
                {
                    yield break;
                }

                currentEntryView = nextEntryView;
            }
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