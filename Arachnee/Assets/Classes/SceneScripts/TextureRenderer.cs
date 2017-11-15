using System.IO;
using UnityEditor;
using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.SceneScripts
{
    public class TextureRenderer : MonoBehaviour
    {
        private Renderer _renderer;

        public void Start()
        {
            _renderer = this.GetComponent<Renderer>();
            if (_renderer == null)
            {
                Logger.LogError($"No {nameof(Renderer)} component found on {nameof(TextureRenderer)} gameobject.");
                return;
            }
        }

        public void SetTexture(Texture texture)
        {
            if (texture == null)
            {
                Logger.LogError("Texture to set was null.");
                return;
            }

            _renderer.material.mainTexture = texture;
        }
    }
}