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
using UnityEngine.SceneManagement;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization
{
    public class Explorer : MonoBehaviour
    {
        public SearchEngine searchEngine;
        public ControllerBase controller;
        public GraphEngine graphEngine;
        public LoadingFeedback loadingFeedback;

        private ModelViewProvider _provider;

        private CompactGraph _graph;
        private readonly HashSet<string> _requestedByUser = new HashSet<string>();
        
        public void Start()
        {
            _provider = searchEngine.Provider;
            
            this.searchEngine.OnSearchResultSelected -= OnSearchResultSelected;
            this.searchEngine.OnSearchResultSelected += OnSearchResultSelected;
            _provider.OnEntryViewSelected -= OnEntrySelected;
            _provider.OnEntryViewSelected += OnEntrySelected;
            _provider.Builder.OnConnectionViewBuilt -= OnConnectionBuilt;
            _provider.Builder.OnConnectionViewBuilt += OnConnectionBuilt;
            
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

        public void Reset()
        {
            SceneManager.LoadScene(0); // brutal
        }

        void OnDestroy()
        {
            this.searchEngine.OnSearchResultSelected -= OnSearchResultSelected;
            _provider.OnEntryViewSelected -= OnEntrySelected;
            _provider.Builder.OnConnectionViewBuilt -= OnConnectionBuilt;
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
            if (!_requestedByUser.Contains(entryId))
            {
                _requestedByUser.Add(entryId);
                yield return ConnectToPreviousRequests(entryView);
            }
            
            loadingFeedback.StopLoading();
        }
        
        private IEnumerator ConnectToPreviousRequests(EntryView entryViewToConnect)
        {
            var toConnect = _provider.ActiveEntryViews.Where(e => _requestedByUser.Contains(e.Entry.Id)).ToList();
            toConnect.Remove(entryViewToConnect);

            foreach (var activeEntryView in toConnect)
            {
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
    }
}