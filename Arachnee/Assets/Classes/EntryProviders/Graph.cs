using System.Collections.Generic;
using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders
{
    public class Graph : EntryProvider
    {
        public override Stack<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            throw new System.NotImplementedException();
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            if (BiggerProvider != null)
            {
                return BiggerProvider.TryGetEntry(entryId, out entry);
            }

            entry = DefaultEntry.Instance;
            return false;
        }
    }
}
