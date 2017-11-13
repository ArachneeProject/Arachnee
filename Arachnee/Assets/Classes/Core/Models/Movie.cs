using System.Collections.Generic;

namespace Assets.Classes.Core.Models
{
    public class Movie : Entry
    {
        public string BackdropPath { get; set; }

        public int Budget { get; set; }

        public List<string> Tags { get; set; }

        public string Homepage { get; set; }

        public string ImdbId { get; set; }

        public string OriginalLanguage { get; set; }

        public string OriginalTitle { get; set; }

        public string Overview { get; set; }

        public float Popularity { get; set; }
        
        public string ReleaseDate { get; set; }

        public int Revenue { get; set; }

        public int Runtime { get; set; }

        public string Status { get; set; }

        public string Tagline { get; set; }

        public string Title { get; set; }

        public float VoteAverage { get; set; }

        public int VoteCount { get; set; }

        public override string ToString()
        {
            return $"{Title} ({Id})";
        }
    }
}
