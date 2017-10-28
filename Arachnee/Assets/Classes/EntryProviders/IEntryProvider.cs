using System.Collections.Generic;
using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders
{
    public interface IEntryProvider
    {
        /// <summary>
        /// Runs a search to get a queue of entries corresponding to the given query. First item in the queue is the best result.
        /// </summary>
        /// <param name="searchQuery">The query to run.</param>
        /// <returns>Queue of the results. Best result is on top of the queue.</returns>
        Queue<TEntry> GetSearchResults<TEntry>(string searchQuery) where TEntry : Entry;

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
        /// Gets all entries connected to the given entry id by at least one of the given connection type.
        /// </summary>
        /// <typeparam name="TEntry">Type of the connected entries.</typeparam>
        /// <param name="entryId">Id of the entry.</param>
        /// <param name="connectionType">Types of connection.</param>
        /// <param name="entries">The resulting connected entries.</param>
        /// <returns>Wheter or not the function succeded.</returns>
        bool TryGetConnectedEntries<TEntry>(string entryId, ConnectionType connectionType, out IEnumerable<TEntry> entries) where TEntry : Entry;
    }
}
