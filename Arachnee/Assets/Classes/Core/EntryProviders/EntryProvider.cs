using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Core.Models;

namespace Assets.Classes.Core.EntryProviders
{
    public abstract class EntryProvider : IEntryProvider
    {
        protected readonly Dictionary<string, Entry> CachedEntries = new Dictionary<string, Entry>();

        public IEntryProvider BiggerProvider { get; set; }

        public abstract Queue<SearchResult> GetSearchResults(string searchQuery);
        
        public bool TryGetEntry(string entryId, out Entry entry)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException("Unable to provide an entry because the given id was empty", "entryId");
            }

            if (CachedEntries.TryGetValue(entryId, out entry))
            {
                return true;
            }
            
            if (!TryLoadEntry(entryId, out entry))
            {
                return false;
            }
            
            CachedEntries.Add(entryId, entry);
            return true;
        }
        
        public bool TryGetConnectedEntries<TEntry>(string entryId, List<ConnectionType> connectionTypes, out IEnumerable<TEntry> entries) where TEntry : Entry
        {
            Entry entry;
            if (!TryGetEntry(entryId, out entry))
            {
                entries = Enumerable.Empty<TEntry>();
                return false;
            }
            
            var oppositeEntries = new List<Entry>();
            foreach (var connection in entry.Connections.Where(c => connectionTypes.Contains(c.Type)))
            {
                Entry oppositeEntry;
                if (this.TryGetEntry(connection.ConnectedId, out oppositeEntry))
                {
                    oppositeEntries.Add(oppositeEntry);
                }
            }

            entries = oppositeEntries.OfType<TEntry>();
            return true;
        }

        protected abstract bool TryLoadEntry(string entryId, out Entry entry);
    }
}
