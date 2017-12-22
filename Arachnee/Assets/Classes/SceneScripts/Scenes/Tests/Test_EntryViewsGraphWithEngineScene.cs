using System.Collections;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.CoreVisualization.PhysicsEngine;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Scenes.Tests
{
    public class Test_EntryViewsGraphWithEngineScene : MonoBehaviour
    {
        public RigidbodyImageTextEntryView rigidbodyImageTextEntryViewPrefab;
        public LineConnectionView lineConnectionViewPrefab;

        public ForceDirectedGraphEngine forceDirectedGraphEngine;

        void Start()
        {
            StartCoroutine(SceneLoading());
        }

        private IEnumerator SceneLoading()
        {
            var builder = new ModelViewBuilder();
            builder.SetPrefab<Movie>(rigidbodyImageTextEntryViewPrefab);
            builder.SetPrefab<Artist>(rigidbodyImageTextEntryViewPrefab);
            builder.SetPrefab<Serie>(rigidbodyImageTextEntryViewPrefab);
            builder.SetPrefab(lineConnectionViewPrefab);

            builder.OnBuiltEntryView += AddEntryViewToEngine;
            builder.OnBuiltConnectionView += AddConnectionToEngine;

            var provider = new ModelViewProvider(new OnlineDatabase(), builder);

            yield return new WaitForEndOfFrame();

            // get seed entry
            var awaitableEntryView = provider.GetEntryViewAsync("Movie-218");
            yield return awaitableEntryView.Await();

            var entryView = awaitableEntryView.Result;
            entryView.gameObject.transform.position = Vector3.zero;

            // get adjacent entries
            var awaitableList = provider.GetAdjacentEntryViewsAsync(entryView, Connection.AllTypes());
            yield return awaitableList.Await();
            
            // get next adjacent entries
            /* TODO: improve engine perfs
            foreach (var view in awaitableList.Result)
            {
                var awaitableNextList = provider.GetAdjacentEntryViewsAsync(view, Connection.AllTypes());
                yield return awaitableNextList.Await();
            }
            */
        }

        private void AddConnectionToEngine(ConnectionView connectionView)
        {
            if (connectionView == null)
            {
                return;
            }

            var left = connectionView.Left as RigidbodyImageTextEntryView;
            if (left?.Rigidbody == null)
            {
                return;
            }

            var right = connectionView.Right as RigidbodyImageTextEntryView;
            if (right?.Rigidbody == null)
            {
                return;
            }

            forceDirectedGraphEngine.AddEdge(left.Rigidbody, right.Rigidbody);
        }

        private void AddEntryViewToEngine(EntryView entryView)
        {
            var rigidbodyEntryView = entryView as RigidbodyImageTextEntryView;
            if (rigidbodyEntryView?.Rigidbody == null)
            {
                return;
            }
            
            rigidbodyEntryView.transform.position = Random.onUnitSphere * 2;
            forceDirectedGraphEngine.AddRigidbody(rigidbodyEntryView.Rigidbody);
        }
    }
}