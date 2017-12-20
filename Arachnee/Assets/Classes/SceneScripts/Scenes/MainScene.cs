using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.CoreVisualization;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.CoreVisualization.ModelViewManagement.Builders;
using Assets.Classes.CoreVisualization.ModelViews;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes.SceneScripts.Scenes
{
    public class MainScene : MonoBehaviour
    {
        public SearchResultView searchResultViewPrefab;
        public InputField mainInputField;
        public GameObject loadingFeedback;

        private ModelViewBuilder _builder;
        private ModelViewProvider _provider;

        private SearchEngineView _searchEngineView;

        void Start()
        {
            _builder = new ModelViewBuilder();
            _builder.SetPrefab(searchResultViewPrefab);

            _provider = new ModelViewProvider(new OnlineDatabase(), _builder);
            _searchEngineView = new SearchEngineView(mainInputField, _provider, loadingFeedback);
        }

        void OnDestroy()
        {
            _searchEngineView.Dispose();
        }
    }
}
