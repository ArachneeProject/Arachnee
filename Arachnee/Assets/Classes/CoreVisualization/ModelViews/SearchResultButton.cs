using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    [RequireComponent(typeof(Button))]
    public class SearchResultButton : SearchResultView
    {
        private Button _button;
        
        [UsedImplicitly]
        void Start()
        {
            _button = this.GetComponent<Button>();
            if (_button == null)
            {
                Logger.LogError($"No {nameof(Button)} component found on {nameof(SearchResultView)} GameObject.");
                return;
            }
            
            var canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Logger.LogError($"Canvas was not found for {nameof(SearchResultView)}.");
                return;
            }

            this.transform.SetParent(canvas.transform);

            _button.onClick.AddListener(OnMouseUpAsButton);
        }
        
        [UsedImplicitly]
        void OnDestroy()
        {
            _button.onClick.RemoveListener(OnMouseUpAsButton);
        }
    }
}