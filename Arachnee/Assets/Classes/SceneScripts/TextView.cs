using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.SceneScripts
{
    [RequireComponent(typeof(TextMesh))]
    public class TextView : MonoBehaviour
    {
        private TextMesh _textMesh;
        
        void Start()
        {
            _textMesh = this.GetComponent<TextMesh>();
            if (_textMesh == null)
            {
                Logger.LogError($"No {nameof(TextMesh)} component found on {nameof(TextView)} gameobject.");
                return;
            }
        }
        
        public void SetText(string text)
        {
            if (text == null)
            {
                Logger.LogError("Text to set was null.");
                return;
            }

            _textMesh.text = text;
        }
    }
}