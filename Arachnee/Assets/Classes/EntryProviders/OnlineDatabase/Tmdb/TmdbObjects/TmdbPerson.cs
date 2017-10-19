using System.Collections.Generic;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class TmdbPerson
    {
        public string Birthday { get; set; }
        public string Homepage { get; set; }
        public List<string> AlsoKnownAs { get; set; }
        public bool Adult { get; set; }
        public string Biography { get; set; }
        public object Deathday { get; set; }
        public CombinedCredits CombinedCredits { get; set; }
        public long Gender { get; set; }
        public string ImdbId { get; set; }
        public string PlaceOfBirth { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public double Popularity { get; set; }
        public string ProfilePath { get; set; }
    }
}