using System;
using System.Collections.Generic;
using Assets.Classes.Core.Models;
using Assets.Classes.Logging;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase
{
    public class OnlineDatabase : EntryProvider
    {
        private readonly TmdbProxy _proxy = new TmdbProxy();

        public override Queue<SearchResult> GetSearchResults(string searchQuery)
        {
            var queue = new Queue<SearchResult>();
            if (string.IsNullOrEmpty(searchQuery))
            {
                return queue;
            }

            var results = _proxy.GetSearchResults(searchQuery);
            foreach (var searchResult in results)
            {
                queue.Enqueue(searchResult);
            }

            return queue;
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            try
            {
                entry = _proxy.GetEntry(entryId);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                entry = DefaultEntry.Instance;
                return false;
            }
        }
    }
}
