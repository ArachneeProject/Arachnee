using System;
using System.Collections.Generic;
using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders
{
    public class LocalDatabase : EntryProvider
    {
        public override Queue<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            throw new NotImplementedException();
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            throw new NotImplementedException();
        }
    }
}
