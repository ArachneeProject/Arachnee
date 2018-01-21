using System;
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
        private Text _text;
        
        protected override void Start()
        {
            base.Start();

            _button = this.GetComponent<Button>();
            if (_button == null)
            {
                Logger.LogError($"No {nameof(Button)} component found on {nameof(SearchResultView)} GameObject.");
                return;
            }

            _text = this.GetComponentInChildren<Text>();
            {
                if (_text == null)
                {
                    Logger.LogError($"No {nameof(Text)} component found on {nameof(SearchResultView)} GameObject.");
                    return;
                }
            }
            
            if (string.IsNullOrWhiteSpace(this.Result.Date))
            {
                _text.text = Result.Name;
            }
            else
            {
                _text.text = $"{this.Result.Name} ({this.Result.Date.Substring(0, Math.Min(this.Result.Date.Length, 4))})";
            }
            
            _button.onClick.AddListener(OnMouseUpAsButton);
        }
        
        [UsedImplicitly]
        void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnMouseUpAsButton);
            }
        }
    }
}