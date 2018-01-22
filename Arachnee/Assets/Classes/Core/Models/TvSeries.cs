using System;
using System.Collections.Generic;

namespace Assets.Classes.Core.Models
{
    public class TvSeries : Entry
    {
        public string BackdropPath { get; set; }
        public List<long> EpisodeRunTime { get; set; }
        public DateTime FirstAirDate { get; set; }
        public bool InProduction { get; set; }
        public DateTime LastAirDate { get; set; }
        public string Name { get; set; }
        public long NumberOfEpisodes { get; set; }
        public long NumberOfSeasons { get; set; }
        public List<string> OriginCountry { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalName { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public string Status { get; set; }
        public double VoteAverage { get; set; }
        public long VoteCount { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}