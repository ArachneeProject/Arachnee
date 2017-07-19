using Arachnee.GraphElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnee.EntryProvider
{
    public class Graph : EntryProvider
    {
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
