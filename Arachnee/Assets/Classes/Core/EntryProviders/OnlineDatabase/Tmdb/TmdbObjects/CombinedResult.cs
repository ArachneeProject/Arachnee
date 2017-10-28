using System.Collections.Generic;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class CombinedResult
    {
        public bool? Adult { get; set; }
        public string BackdropPath { get; set; }
        public string FirstAirDate { get; set; }
        public List<long> GenreIds { get; set; }
        public long Id { get; set; }
        public List<KnownFor> KnownFor { get; set; }
        public string MediaType { get; set; }
        public string Name { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalName { get; set; }
        public string OriginalTitle { get; set; }
        public List<string> OriginCountry { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public string ProfilePath { get; set; }
        public string ReleaseDate { get; set; }
        public string Title { get; set; }
        public bool? Video { get; set; }
        public double? VoteAverage { get; set; }
        public long? VoteCount { get; set; }
    }
}