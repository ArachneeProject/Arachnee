using System.Collections.Generic;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class Cast
    {
        public bool? Adult { get; set; }
        public string BackdropPath { get; set; }
        public long CastId { get; set; }
        public string Character { get; set; }
        public string CreditId { get; set; }
        public string Department { get; set; }
        public long? EpisodeCount { get; set; }
        public string FirstAirDate { get; set; }
        public long Gender { get; set; }
        public List<long> GenreIds { get; set; }
        public long Id { get; set; }
        public string Job { get; set; }
        public string MediaType { get; set; }
        public string Name { get; set; }
        public long Order { get; set; }
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
        public double VoteAverage { get; set; }
        public long VoteCount { get; set; }
    }
}