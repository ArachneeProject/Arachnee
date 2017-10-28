using System.Collections.Generic;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class Credits
    {
        public List<Cast> Cast { get; set; }
        public List<Crew> Crew { get; set; }
    }
}