using Newtonsoft.Json;

namespace Assets.Classes.EntryProviders.OnlineDatabase
{
    public class SearchResult
    {
        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        public string Id { get; set; }
    }
}