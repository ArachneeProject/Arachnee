using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public LoadingFeedback loadingFeedback;

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

            this.sidePanel.OnExpandRequested -= Expand;
            this.sidePanel.OnExpandRequested += Expand;

            this.sidePanel.OnFoldUpRequested -= FoldUp;
            this.sidePanel.OnFoldUpRequested += FoldUp;

            this.sidePanel.OnHideRequested -= Hide;
            this.sidePanel.OnHideRequested += Hide;

            this.sidePanel.Start();
            
            string graphPath = Path.Combine(Application.dataPath, "Database", "graph.spdr");
            if (!File.Exists(graphPath))
            {
                Logger.LogError($"Grpah not found at \"{graphPath}\".");
                return;
            }

            if (_graph == null)
            {
                _graph = new CompactGraph();
                StartCoroutine(LoadGraph(graphPath));
            }
        }
        
        void OnDestroy()
        {
            this.searchEngine.OnSearchResultSelected -= OnSearchResultSelected;
            _provider.OnEntryViewSelected -= OnEntrySelected;
            _provider.Builder.OnConnectionViewBuilt -= OnConnectionBuilt;
            
            this.sidePanel.OnExpandRequested -= Expand;
            this.sidePanel.OnFoldUpRequested -= FoldUp;
            this.sidePanel.OnHideRequested -= Hide;
        }

        private IEnumerator LoadGraph(string graphPath)
        {
            Logger.LogInfo("Loading graph...");
            var chrono = Stopwatch.StartNew();
            loadingFeedback.StartLoading();

            var asyncCall = new AsyncCall<CompactGraph, CompactGraph>(
                () => CompactGraph.InitializeFrom(graphPath, Connection.AllTypes()),
                graph => graph);
            yield return asyncCall.Execute();
            _graph = asyncCall.Result;

            loadingFeedback.StopLoading();
            chrono.Stop();
            Logger.LogInfo($"Graph loaded in {chrono.Elapsed.TotalSeconds}s.");
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
            
            sidePanel.OpenPanel(entryView);
            controller.StartCoroutine(FocusOnEntryRoutine(entryView));
        }

        private void OnSearchResultSelected(string entryId)
        {
            controller.StartCoroutine(LoadAndFocusOnEntryRoutine(entryId));
        }

        private IEnumerator LoadAndFocusOnEntryRoutine(string entryId)
        {
            loadingFeedback.StartLoading();

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
            
            // connect to other entries
            yield return ConnectToAll(entryView);

            loadingFeedback.StopLoading();
        }

        // TODO: should connect to previously requested by user only
        private IEnumerator ConnectToAll(EntryView entryViewToConnect)
        {
            foreach (var activeEntryView in _provider.ActiveEntryViews.ToList())
            {
                if (activeEntryView == entryViewToConnect)
                {
                    continue;
                }

                var asyncCall = new AsyncCall<List<string>, List<string>>(
                    () => _graph.GetShortestPath(entryViewToConnect.Entry.Id, activeEntryView.Entry.Id),
                    result => result);

                yield return asyncCall.Execute();

                var path = asyncCall.Result;

                var currentEntryView = entryViewToConnect;

                foreach (string nextId in path)
                {
                    var awaitableEntryView = _provider.GetEntryViewAsync(nextId);
                    yield return awaitableEntryView.Await();

                    var nextEntryView = awaitableEntryView.Result;

                    var awaitableConnections = _provider.GetConnectionViewsAsync(currentEntryView, Connection.AllTypes(), nextEntryView);

                    yield return awaitableConnections.Await(); // here the connections are loaded, so there is nothing else to do

                    currentEntryView = nextEntryView;
                }
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

        #region ExpandFoldUpHide


        private void Hide(EntryView entryView)
        {
            var entryViewRigidbody = (entryView as RigidbodyImageTextEntryView)?.Rigidbody;
            if (entryViewRigidbody != null)
            {
                graphEngine.RemoveRigidbody(entryViewRigidbody);
            }

            _provider.Unload(entryView);

            sidePanel.ClosePanel();
        }

        private void FoldUp(EntryView entryView)
        {
            var connectedIds = entryView.Entry.Connections.Select(c => c.ConnectedId);
            
            
        }

        private void Expand(EntryView entryView)
        {
            StartCoroutine(ExpandRoutine(entryView));
        }

        private IEnumerator ExpandRoutine(EntryView entryView)
        {
            var awaitable = _provider.GetAdjacentEntryViewsAsync(entryView, Connection.AllTypes());
            yield return awaitable.Await();
        }

        #endregion
    }
}