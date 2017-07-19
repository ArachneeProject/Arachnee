using Arachnee.GraphElements;
using System.Collections.Generic;

namespace Arachnee.EntryProvider
{
    public interface IEntryProvider
    {
        /// <summary>
        /// Gets the entry corresponding to the given id.
        /// </summary>
        /// <param name="entryId">Id of the entry.</param>
        /// <param name="entry">The resulting entry.</param>
        /// <returns>Wheter or not the function succeded.</returns>
        bool TryGetEntry(string entryId, out Entry entry);

        /// <summary>
        /// Gets all available entries in this <see cref="IEntryProvider"/>.
        /// </summary>
        /// <typeparam name="TEntry">Type of the entries.</typeparam>
        /// <returns>Entries available.</returns>
        IEnumerable<TEntry> GetAvailableEntries<TEntry>() where TEntry : Entry;

        /// <summary>
        /// Gets all entries connected to the given entry id by at least one of the given connection flags.
        /// </summary>
        /// <typeparam name="TEntry">Type of the connected entries.</typeparam>
        /// <param name="entryId">Id of the entry.</param>
        /// <param name="connectionFlags">Types of connection.</param>
        /// <param name="entries">The resulting connected entries.</param>
        /// <returns>Wheter or not the function succeded.</returns>
        bool TryGetConnectedEntries<TEntry>(string entryId, ConnectionFlags connectionFlags, out IEnumerable<TEntry> entries) where TEntry : Entry;
    }
}
