using System.Collections.Generic;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class CombinedCredits
    {
        public List<Cast> Cast { get; set; }
        public List<Cast> Crew { get; set; }
    }
}