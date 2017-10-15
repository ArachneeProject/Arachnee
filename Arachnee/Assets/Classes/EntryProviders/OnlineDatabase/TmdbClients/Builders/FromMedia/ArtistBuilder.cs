using System;
using Assets.Classes.GraphElements;
using Newtonsoft.Json;

namespace Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders.FromMedia
{
    public class ArtistBuilder : MediaBuilder
    {
        protected override string ResourceAddress => "person/{id}";

        protected override bool TryDeserialize(string jsonString, out Entry entry)
        {
            entry = JsonConvert.DeserializeObject<Artist>(jsonString, JsonSettings.Tmdb);

            if (string.IsNullOrEmpty(entry?.Id))
            {
                entry = DefaultEntry.Instance;
                return false;
            }

            entry.Id = typeof(Artist).Name + IdSeparator + entry.Id;
            return true;
        }
    }
}