using System.Collections.Generic;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class Configuration
    {
        public List<string> ChangeKeys { get; set; }
        public ImageConfiguration ImageConfiguration { get; set; }
    }
}