using System.Collections.Generic;
using FSharpx.Collections;

namespace Assets.Classes.GraphElements
{
    public class Artist : Entry
    {
        public string Biography { get; set; }

        public string Birthday { get; set; }

        public string Deathday { get; set; }

        public int Gender { get; set; }

        public string Homepage { get; set; }

        public string ImdbId { get; set; }

        public string Name { get; set; }

        public List<string> NickNames { get; set; }

        public string PlaceOfBirth { get; set; }

        public float Popularity { get; set; }

        public string ProfilePath { get; set; }
        
        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
