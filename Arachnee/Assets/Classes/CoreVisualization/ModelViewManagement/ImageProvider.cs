using System.IO;
using Assets.Classes.Core.EntryProviders.OnlineDatabase;
using Assets.Classes.Core.Models;
using UnityEngine;

namespace Assets.Classes.CoreVisualization.ModelViewManagement
{
    public class ImageProvider
    {
        private readonly TmdbProxy _proxy = new TmdbProxy();

        public byte[] GetTextureBytes(Entry entry)
        {
            byte[] bytes = _proxy.GetMainImage(entry);
            return bytes;
        }
    }
}