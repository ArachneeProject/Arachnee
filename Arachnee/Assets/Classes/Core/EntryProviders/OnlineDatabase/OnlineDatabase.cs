using System;
using System.Collections.Generic;
using Assets.Classes.Core.Models;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase
{
    public class OnlineDatabase : EntryProvider
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
