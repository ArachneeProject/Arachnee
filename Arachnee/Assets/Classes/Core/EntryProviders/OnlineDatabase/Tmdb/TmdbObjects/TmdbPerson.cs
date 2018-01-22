using System.Collections.Generic;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class TmdbPerson
    {
        public bool Adult { get; set; }
        public List<string> AlsoKnownAs { get; set; }
        public string Biography { get; set; }
        public string Birthday { get; set; }
        public CombinedCredits CombinedCredits { get; set; }
        public string Deathday { get; set; }
        public long? Gender { get; set; }
        public string Homepage { get; set; }
        public ulong Id { get; set; }
        public Images Images { get; set; }
        public string ImdbId { get; set; }
        public string Name { get; set; }
        public string PlaceOfBirth { get; set; }
        public double Popularity { get; set; }
        public string ProfilePath { get; set; }
    }
}