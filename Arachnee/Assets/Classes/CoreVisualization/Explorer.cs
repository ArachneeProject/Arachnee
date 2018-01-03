using System.Collections;
using System.Collections.Generic;
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
                _graph = CompactGraph.InitializeFrom(graphPath, Connection.AllTypes());
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
            
            // connect to nearest entry
            yield return ConnectToNearest(entryView);
        }

        private IEnumerator ConnectToNearest(EntryView entryViewToConnect)
        {
            var connectedIds = entryViewToConnect.Entry.Connections.Select(c => c.ConnectedId).ToList();
            var otherActiveIds = new List<string>();
            bool alreadyLinked = false;

            // tries to connect the EntryView to its adjacent EntryViews if they are already here
            foreach (var activeEntryView in _provider.ActiveEntryViews.ToList())
            {
                if (connectedIds.Contains(activeEntryView.Entry.Id))
                {
                    yield return  _provider.GetConnectionViewsAsync(entryViewToConnect, Connection.AllTypes(), activeEntryView).Await();
                    alreadyLinked = true;
                }
                else
                {
                    otherActiveIds.Add(activeEntryView.Entry.Id);
                }
            }

            if (alreadyLinked)
            {
                yield break;
            }

            // tries to connect the EntryView to the nearest available EntryView
            var allSteps = _graph.GetShortestPaths(entryViewToConnect.Entry.Id, otherActiveIds);
            if (allSteps.Count == 0)
            {
                yield break;
            }

            var shortestSteps = allSteps.Aggregate((currentBest, next) => currentBest.Count < next.Count ? currentBest : next);
            
            if (shortestSteps.Count < 2)
            {
                yield break;
            }

            EntryView currentEntryView = entryViewToConnect;
            for (int i = 1; i < shortestSteps.Count; i++)
            {
                var next = shortestSteps[i];
                
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