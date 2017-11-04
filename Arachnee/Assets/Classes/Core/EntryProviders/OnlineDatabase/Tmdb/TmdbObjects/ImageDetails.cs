using Newtonsoft.Json;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class ImageDetails
    {
        public double AspectRatio { get; set; }
        public string FilePath { get; set; }
        public long Height { get; set; }
        [JsonProperty("iso_639_1")]
        public string LanguageIsoCode { get; set; }
        public double VoteAverage { get; set; }
        public long VoteCount { get; set; }
        public long Width { get; set; }
    }
}