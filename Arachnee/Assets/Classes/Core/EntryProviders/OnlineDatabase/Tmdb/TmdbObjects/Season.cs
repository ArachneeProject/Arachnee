using System;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class Season
    {
        public DateTime AirDate { get; set; }
        public long EpisodeCount { get; set; }
        public long Id { get; set; }
        public string PosterPath { get; set; }
        public long SeasonNumber { get; set; }
    }
}