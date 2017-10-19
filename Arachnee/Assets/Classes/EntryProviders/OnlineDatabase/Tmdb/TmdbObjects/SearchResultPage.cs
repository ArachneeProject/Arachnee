using System.Collections.Generic;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects
{
    public class SearchResultPage
    {
        public long Page { get; set; }
        public List<CombinedResult> Results { get; set; }
        public long TotalPages { get; set; }
        public long TotalResults { get; set; }
    }
}