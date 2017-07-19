using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.EntryProviders.OnlineDatabase.Builders;
using Assets.Classes.GraphElements;
using UnityEngine;

namespace Assets.Classes.EntryProviders.OnlineDatabase
{
    public class OnlineDatabase : EntryProvider
    {
        private readonly MultiSearchClient _multiSearchClient = new MultiSearchClient();
        private readonly Dictionary<string, EntryBuilder> _builders = new Dictionary<string, EntryBuilder>
        {
            {"movie", new MovieBuilder()},
            {"person", new ArtistBuilder()}
        };

        public override Stack<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            var mediaResults = _multiSearchClient.RunSearch(searchQuery);

            var results = new List<Entry>();
            foreach (var mediaResult in mediaResults)
            {
                Entry e;
                if (TryGetEntry(mediaResult.MediaType + "-" + mediaResult.Id, out e))
                {
                    results.Add(e);
                }
            }

            return new Stack<TEntry>(results.OfType<TEntry>());
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            var split = entryId.Split('-');
            if (split.Length != 2)
            {
                throw new ArgumentException(entryId + " is not a valid id", "entryId");
            }

            var mediaType = split[0];
            var id = split[1];
            EntryBuilder builder;
            if (!_builders.TryGetValue(mediaType, out builder))
            {
                Debug.LogError("Unable to find builder for unhandled type " + mediaType);
                entry = DefaultEntry.Instance;
                return false;
            }
            
            return builder.TryToBuild(id, out entry);
        }
    }
}
