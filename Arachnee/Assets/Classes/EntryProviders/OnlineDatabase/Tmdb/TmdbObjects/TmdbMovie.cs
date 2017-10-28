using System.Collections.Generic;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class TmdbMovie
    {
        public bool Adult { get; set; }
        public string BackdropPath { get; set; }
        public BelongsToCollection BelongsToCollection { get; set; }
        public long Budget { get; set; }
        public Credits Credits { get; set; }
        public List<IdentifiedName> Genres { get; set; }
        public string Homepage { get; set; }
        public long Id { get; set; }
        public string ImdbId { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public List<IdentifiedName> ProductionCompanies { get; set; }
        public List<ProductionCountry> ProductionCountries { get; set; }
        public string ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public long Runtime { get; set; }
        public List<SpokenLanguage> SpokenLanguages { get; set; }
        public string Status { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        public bool Video { get; set; }
        public double VoteAverage { get; set; }
        public long VoteCount { get; set; }
    }
}