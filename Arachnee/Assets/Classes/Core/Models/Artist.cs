using System.Collections.Generic;

namespace Assets.Classes.Core.Models
{
    public class Artist : Entry
    {
        public string Biography { get; set; }

        public string Birthday { get; set; }

        public string Deathday { get; set; }
        
        public string Homepage { get; set; }

        public string ImdbId { get; set; }

        public string Name { get; set; }

        public List<string> NickNames { get; set; }

        public string PlaceOfBirth { get; set; }

        public float Popularity { get; set; }
        
        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
