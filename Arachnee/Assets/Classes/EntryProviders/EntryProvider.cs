using Arachnee.GraphElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnee.EntryProvider
{
    public abstract class EntryProvider : IEntryProvider
    {
        private readonly List<Entry> _cachedEntries = new List<Entry>();

        public IEntryProvider BiggerProvider { get; set; }

        public bool TryGetEntry(string entryId, out Entry entry)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException("Unable to provide an entry because the given id was empty", "entryId");
            }

            entry = _cachedEntries.FirstOrDefault(e => e.Id == entryId);
            if (entry != null)
            {
                return true;
            }

            if (!TryLoadEntry(entryId, out entry))
            {
                return false;
            }

            _cachedEntries.Add(entry);
            return true;
        }

        public IEnumerable<TEntry> GetAvailableEntries<TEntry>() where TEntry : Entry
        {
            return _cachedEntries.OfType<TEntry>();
        }
        
        public bool TryGetConnectedEntries<TEntry>(string entryId, ConnectionFlags connectionFlags, out IEnumerable<TEntry> entries) where TEntry : Entry
        {
            Entry entry;
            if (!TryGetEntry(entryId, out entry))
            {
                entries = Enumerable.Empty<TEntry>();
                return false;
            }

            var validConnections = entry.Connections.Where(c => c.Contains(entryId) && ((c.Flags & connectionFlags) != 0x0));
            var oppositeEntries = new List<Entry>();
            foreach (var validConnection in validConnections)
            {
                Entry oppositeEntry;
                if (this.TryGetEntry(validConnection.GetOppositeOf(entryId), out oppositeEntry))
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
