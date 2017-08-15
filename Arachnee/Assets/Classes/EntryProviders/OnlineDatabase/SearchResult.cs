using Newtonsoft.Json;

namespace Assets.Classes.EntryProviders.OnlineDatabase
{
    public class SearchResult
    {
        private string _mediaType;

        [JsonProperty("media_type")]
        public string MediaType
        {
            get { return _mediaType; }
            set { _mediaType = char.ToUpper(value[0]) + value.Substring(1).ToLower(); }
        }

        public string Id { get; set; }
    }
}