using Assets.Classes.GraphElements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders.FromMedia
{
    public class MovieBuilder : MediaBuilder
    {
        protected override string ResourceAddress
        {
            get { return "movie/{id}"; }
        }

        protected override bool TryDeserialize(string jsonString, out Entry entry)
        {
            entry = JsonConvert.DeserializeObject<Movie>(jsonString);
            entry.Id = typeof (Movie).Name + IdSeparator + entry.Id;

            return true;
        }
    }
}