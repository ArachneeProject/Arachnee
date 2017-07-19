using Arachnee.GraphElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnee.EntryProvider
{
    public class LocalDatabase : EntryProvider
    {
        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            // try load from file
            throw new NotImplementedException();

            return BiggerProvider != null && BiggerProvider.TryGetEntry(entryId, out entry);
        }
    }
}
