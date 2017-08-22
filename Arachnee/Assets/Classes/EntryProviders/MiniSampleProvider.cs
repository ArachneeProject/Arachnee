using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders
{
    public class MiniSampleProvider : EntryProvider
    {
        public readonly List<Entry> Entries;

        public MiniSampleProvider()
        {
            Entries = new List<Entry>
            {
                new Movie
                {
                    Id = "Movie-218",
                    Title = "The Terminator",
                    Connections = new List<Connection>
                    {
                        // cameron
                        new Connection
                        {
                            Flags = ConnectionFlags.Actor | ConnectionFlags.Director,
                            ConnectedId = "Artist-2710"
                        },
                        // schwarzenegger
                        new Connection
                        {
                            Flags = ConnectionFlags.Actor,
                            ConnectedId = "Artist-1100"
                        }
                    }
                },
                new Artist
                {
                    Id = "Artist-1100",
                    LastName = "Schwarzenegger",
                    Connections = new List<Connection>
                    {
                        new Connection
                        {
                            Flags = ConnectionFlags.Actor,
                            ConnectedId = "Movie-218",
                        }
                    }
                },
                new Artist
                {
                    Id = "Artist-2710",
                    FirstName = "James",
                    LastName = "Cameron",
                    Connections = new List<Connection>
                    {
                        new Connection
                        {
                            Flags = ConnectionFlags.Actor | ConnectionFlags.Director,
                            ConnectedId = "Movie-218"
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Returns a queue containing the item corresponding to the query.
        /// </summary>
        public override Queue<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            var queue = new Queue<TEntry>();

            var items = Entries.OfType<TEntry>().ToList();
            var best = items.FirstOrDefault(i => i.ToString().Equals(searchQuery, StringComparison.OrdinalIgnoreCase));
            if (best != null)
            {
                queue.Enqueue(best);
            }

            return queue;
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            entry = Entries.FirstOrDefault(e => e.Id == entryId);
            return entry != null;
        }
    }
}