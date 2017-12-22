using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.SceneScripts.Controllers;
using UnityEngine;

namespace Assets.Classes.CoreVisualization
{
    public class Explorer
    {
        private readonly ModelViewProvider _provider;
        private readonly SearchEngine _searchEngine;
        private readonly ControllerBase _controller;
        
        public Explorer(ModelViewProvider provider, SearchEngine searchEngine, ControllerBase controller)
        {
            _provider = provider;
            _searchEngine = searchEngine;
            _controller = controller;

            searchEngine.OnSelectedEntry += OnSelectedSearch;
        }

        private void OnSelectedSearch(object sender, string e)
        {
            FocusOnEntry(e);
        }

        private void FocusOnEntry(string entryId)
        {
            var entryView = _provider.GetEntryView(entryId);
            if (entryView == null)
            {
                return;
            }

            entryView.transform.position = Random.onUnitSphere * 5;
            _controller.transform.LookAt(entryView.transform.position);
        }
    }
}