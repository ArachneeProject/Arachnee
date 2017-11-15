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
        protected override void Start()
        {
            base.Start();

            var textureRenderer = this.gameObject.GetComponentInChildren<TextureRenderer>();
            if (textureRenderer == null)
            {
                Logger.LogError($"No {nameof(TextureRenderer)} component found in children of {nameof(ImageTextEntryView)} gameobject.");
                return;
            }
            
            textureRenderer.Start();
            
            var asyncCall = new AsyncCall<byte[]>();

            this.StartCoroutine(asyncCall.Execute(() =>
            {
                var imageProvider = new ImageProvider();
                var bytes = imageProvider.GetTextureBytes(this.Entry);
                return bytes;
            }, 
            bytes =>
            {
                var texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                textureRenderer.SetTexture(texture);
            }));
        }


    }
}