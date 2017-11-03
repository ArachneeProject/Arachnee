using System.Collections.Generic;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class KnownFor
    {
        public bool Adult { get; set; }
        public string BackdropPath { get; set; }
        public List<long> GenreIds { get; set; }
        public ulong Id { get; set; }
        public string MediaType { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public string ReleaseDate { get; set; }
        public string Title { get; set; }
        public bool Video { get; set; }
        public double VoteAverage { get; set; }
        public long VoteCount { get; set; }
    }
}