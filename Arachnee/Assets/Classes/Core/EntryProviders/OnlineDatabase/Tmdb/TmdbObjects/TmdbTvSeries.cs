using System;
using System.Collections.Generic;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class TmdbTvSeries
    {
        public string BackdropPath { get; set; }
        public List<CreatedBy> CreatedBy { get; set; }
        public List<long> EpisodeRunTime { get; set; }
        public DateTime FirstAirDate { get; set; }
        public List<IdentifiedName> Genres { get; set; }
        public string Homepage { get; set; }
        public ulong Id { get; set; }
        public bool InProduction { get; set; }
        public List<string> Languages { get; set; }
        public DateTime LastAirDate { get; set; }
        public string Name { get; set; }
        public List<IdentifiedName> Networks { get; set; }
        public long NumberOfEpisodes { get; set; }
        public long NumberOfSeasons { get; set; }
        public List<string> OriginCountry { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalName { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public List<object> ProductionCompanies { get; set; }
        public List<Season> Seasons { get; set; }
        public string Status { get; set; }
        public string PurpleType { get; set; }
        public double VoteAverage { get; set; }
        public long VoteCount { get; set; }
        public Credits Credits { get; set; }
    }
}