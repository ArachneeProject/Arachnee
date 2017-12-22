using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using Assets.Classes.CoreVisualization;
using Assets.Classes.CoreVisualization.Layouts;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViews;
using Assets.Classes.CoreVisualization.PhysicsEngine;
using Assets.Classes.SceneScripts.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.SceneScripts.Scenes
{
    public class MainScene : MonoBehaviour
    {
        public ControllerBase controller;

        public EntryView moviePrefab;
        public EntryView artistPrefab;
        public ConnectionView connectionPrefab;

        public SearchResultView searchResultViewPrefab;
        
        public InputField mainInputField;
        public GameObject loadingFeedback;
        public VerticalLayout verticalLayout;

        public GraphEngine graphEngine;

        private ModelViewBuilder _builder;
        private ModelViewProvider _provider;

        private SearchEngine _searchEngine;
        private Explorer _explorer;
        
        void Start()
        {
            _builder = new ModelViewBuilder();
            _builder.SetPrefab(searchResultViewPrefab);
            _builder.SetPrefab<Movie>(moviePrefab);
            _builder.SetPrefab<Artist>(artistPrefab);
            _builder.SetPrefab(connectionPrefab);

            _provider = new ModelViewProvider(new OnlineDatabase(), _builder);

            _searchEngine = new SearchEngine(mainInputField, _provider, loadingFeedback, verticalLayout);
            _explorer = new Explorer(_provider, _searchEngine, controller, graphEngine);
        }
    }
}
