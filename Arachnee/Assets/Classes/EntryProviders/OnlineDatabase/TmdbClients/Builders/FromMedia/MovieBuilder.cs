using Assets.Classes.GraphElements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders.FromMedia
{
    public class MovieBuilder : MediaBuilder
    {
        protected override string ResourceAddress => "movie/{id}";

        protected override bool TryDeserialize(string jsonString, out Entry entry)
        {
            entry = JsonConvert.DeserializeObject<Movie>(jsonString, JsonSettings.Tmdb);

            if (string.IsNullOrEmpty(entry?.Id))
            {
                entry = DefaultEntry.Instance;
                return false;
            }

            entry.Id = typeof(Movie).Name + IdSeparator + entry.Id;
            return true;
        }
    }
}