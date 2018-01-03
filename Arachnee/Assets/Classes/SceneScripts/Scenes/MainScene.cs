using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.CoreVisualization.PhysicsEngine;
using Assets.Classes.SceneScripts.Controllers;
using UnityEngine;

namespace Assets.Classes.SceneScripts.Scenes
{
    public class MainScene : MonoBehaviour
    {
        public ControllerBase controller;

        public SearchEngine searchEngine;
        public SearchResultView searchResultViewPrefab;

        public EntryView moviePrefab;
        public EntryView artistPrefab;
        public ConnectionView connectionPrefab;
        
        public GameObject loadingFeedback;

        public Explorer explorer;
        public SidePanel sidePanel;

        public GraphEngine graphEngine;
        
        private ModelViewBuilder _builder;
        private ModelViewProvider _provider;
        
        
        void Start()
        {
            _builder = new ModelViewBuilder();
            _builder.SetPrefab(searchResultViewPrefab);
            _builder.SetPrefab<Movie>(moviePrefab);
            _builder.SetPrefab<Artist>(artistPrefab);
            _builder.SetPrefab(connectionPrefab);

            _provider = new ModelViewProvider(new OnlineDatabase(), _builder);

            searchEngine.Provider = _provider;
            searchEngine.Start();
            explorer.Start();
        }
    }
}
