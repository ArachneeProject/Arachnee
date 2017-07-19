using Arachnee.GraphElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnee.EntryProvider
{
    public class OnlineDatabase : EntryProvider
    {
        private Dictionary<Type, string> queries;

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            Type type;
            if (!Entry.TryGetEntryType(entryId, out type))
            {
            }

            // switch 


            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs an online search to get a list of entry ids corresponding to the given query.
        /// </summary>
        /// <param name="searchQuery">The query to run.</param>
        /// <param name="entryIds">Entry ids corresponding to the query.</param>
        /// <returns>Wheter or not the function succedeed.</returns>
        public bool TryOnlineSearch(string searchQuery, out List<string> entryIds)
        {
            throw new NotImplementedException();
        }
    }
}
