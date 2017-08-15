using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients;
using Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders;
using Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders.FromMedia;
using Assets.Classes.GraphElements;
using UnityEngine;

namespace Assets.Classes.EntryProviders.OnlineDatabase
{
    public class OnlineDatabase : EntryProvider
    {
        private readonly MultiSearchClient _multiSearchClient = new MultiSearchClient();
        private readonly Dictionary<string, EntryBuilder> _builders = new Dictionary<string, EntryBuilder>
        {
            {"movie", new MovieBuilder()}
        };

        public override Stack<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            var mediaResults = _multiSearchClient.RunSearch(searchQuery);

            var results = new Stack<TEntry>();
            foreach (var result in mediaResults)
            {
                Entry e;
                if (TryGetEntry(result, out e) && e is TEntry)
                {
                    results.Push((TEntry) e); // best result will be the last in the stack
                }
            }

            return new Stack<TEntry>(results); // invert the stack
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            var split = entryId.Split(new [] {TmdbClient.IdSeparator}, StringSplitOptions.None);
            if (split.Length != 2)
            {
                throw new ArgumentException(entryId + " is not a valid id", "entryId");
            }

            var mediaType = split[0];
            var id = split[1];
            EntryBuilder builder;
            if (_builders.TryGetValue(mediaType, out builder))
            {
                return builder.TryToBuild(id, out entry);
            }

            Debug.LogWarning("Unable to find builder for unhandled type " + mediaType);
            entry = DefaultEntry.Instance;
            return false;
        }
    }
}
