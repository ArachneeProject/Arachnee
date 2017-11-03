using System.Collections.Generic;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class Images
    {
        public List<string> BackdropSizes { get; set; }
        public string BaseUrl { get; set; }
        public List<string> LogoSizes { get; set; }
        public List<string> PosterSizes { get; set; }
        public List<string> ProfileSizes { get; set; }
        public string SecureBaseUrl { get; set; }
        public List<string> StillSizes { get; set; }
    }
}