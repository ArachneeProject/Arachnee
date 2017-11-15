using System.IO;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.ModelViewManagement
{
    public class ImageProvider
    {
        private readonly TmdbProxy _proxy = new TmdbProxy();

        public Texture2D GetTexture2D(Entry entry)
        {
            byte[] bytes = _proxy.GetMainImage(entry);
            
            var texture = new Texture2D(2,2);
            texture.LoadImage(bytes);

            return texture;
        }
    }
}