using Assets.Classes.Logging;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class SearchResultButton : SearchResultView
    {
        private Button _button;
        private Text _text;
        
        [UsedImplicitly]
        void Start()
        {
            _button = this.GetComponent<Button>();
            if (_button == null)
            {
                Logger.LogError($"No {nameof(Button)} component found on {nameof(SearchResultView)} gameobject.");
                return;
            }

            _text = this.GetComponentInChildren<Text>();
            if (_text == null)
            {
                Logger.LogError($"No {nameof(Text)} component found in children of {nameof(SearchResultView)} gameobject.");
                return;
            }

            _button.onClick.AddListener(OnMouseUpAsButton);
        }
        
        [UsedImplicitly]
        void OnDestroy()
        {
            _button.onClick.RemoveListener(OnMouseUpAsButton);
        }
    }
}