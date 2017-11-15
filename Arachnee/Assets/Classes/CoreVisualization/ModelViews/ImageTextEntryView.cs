using System;
using Assets.Classes.CoreVisualization.ModelViewManagement;
using Assets.Classes.Logging;
using Assets.Classes.SceneScripts;

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

            // ensure the renderer is initialized
            textureRenderer.Start();

            // setup entryview
            var imageProvider = new ImageProvider();
            var texture = imageProvider.GetTexture2D(this.Entry);
            textureRenderer.SetTexture(texture);
        }
    }
}