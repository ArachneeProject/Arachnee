using System;
using System.Collections;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.SceneScripts;
using UnityEngine;
using Logger = Assets.Classes.Logging.Logger;

namespace Assets.Classes.CoreVisualization.ModelViews
{
    public class ImageTextEntryView : TextEntryView
    {
        private TextureRenderer _textureRenderer;

        public override void Start()
        {
            base.Start();

            _textureRenderer = this.gameObject.GetComponentInChildren<TextureRenderer>();
            if (_textureRenderer == null)
            {
                Logger.LogError($"No {nameof(TextureRenderer)} component found in children of {nameof(ImageTextEntryView)} gameobject.");
                return;
            }

            _textureRenderer.Start();

            StartCoroutine(SetTextureAsync());
        }

        IEnumerator SetTextureAsync()
        {
            var asyncCall = new AsyncCall<byte[], Texture2D>(() =>
            {
                var imageProvider = new ImageProvider();
                var bytes = imageProvider.GetTextureBytes(this.Entry);
                return bytes;
            },
            bytes =>
            {
                var texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                return texture;
            });

            yield return asyncCall.Execute();
            _textureRenderer.SetTexture(asyncCall.Result);
        }
    }
}