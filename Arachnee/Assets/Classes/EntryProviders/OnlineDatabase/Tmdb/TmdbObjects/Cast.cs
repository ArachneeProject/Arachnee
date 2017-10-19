using System.Collections.Generic;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class Cast
    {
        public List<long> GenreIds { get; set; }
        public string OriginalTitle { get; set; }
        public string CreditId { get; set; }
        public string BackdropPath { get; set; }
        public bool? Adult { get; set; }
        public string Character { get; set; }
        public long? EpisodeCount { get; set; }
        public string Department { get; set; }
        public string FirstAirDate { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public long Id { get; set; }
        public string MediaType { get; set; }
        public string OriginalLanguage { get; set; }
        public List<string> OriginCountry { get; set; }
        public string OriginalName { get; set; }
        public string ReleaseDate { get; set; }
        public double Popularity { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public bool? Video { get; set; }
        public string Title { get; set; }
        public double VoteAverage { get; set; }
        public long VoteCount { get; set; }
    }
}