using System;
using System.Collections.Generic;
using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders.OnlineDatabase
{
    public class OnlineDatabase : EntryProvider
    {
        public override Stack<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            throw new NotImplementedException();
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            throw new NotImplementedException();
        }
    }
}
