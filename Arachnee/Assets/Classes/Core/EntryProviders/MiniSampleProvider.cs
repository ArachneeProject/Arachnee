using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes.Core.Models;

namespace Assets.Classes.Core.EntryProviders
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
                    Id = "Movie-280",
                    Title = "Terminator 2: Judgment Day",
                    Connections = new List<Connection>
                    {
                        // cameron
                        new Connection
                        {
                            ConnectedId = "Artist-2710",
                            Type = ConnectionType.Actor,
                            Label = "Some guy at the bar"
                        },
                        new Connection
                        {
                            ConnectedId = "Artist-2710",
                            Type = ConnectionType.Director
                        },

                        // schwarzenegger
                        new Connection
                        {
                            ConnectedId = "Artist-1100",
                            Type = ConnectionType.Actor
                        }
                    }
                },
                new Artist
                {
                    Id = "Artist-1100",
                    Name = "Arnold Schwarzenegger",
                    Connections = new List<Connection>
                    {
                        new Connection
                        {
                            ConnectedId = "Movie-280",
                            Type = ConnectionType.Actor,
                            Label = "T-800"
                        }
                    }
                },
                new Artist
                {
                    Id = "Artist-2710",
                    Name = "James Cameron",
                    Connections = new List<Connection>
                    {
                        new Connection
                        {
                            ConnectedId = "Movie-280",
                            Type = ConnectionType.Actor,
                            Label = "Some guy at the bar"
                        },
                        new Connection
                        {
                            ConnectedId = "Movie-280",
                            Type = ConnectionType.Director
                        },
                    }
                }
            };
        }

        /// <summary>
        /// Returns a queue containing the item corresponding to the query.
        /// </summary>
        public override Queue<SearchResult> GetSearchResults(string searchQuery)
        {
            throw new NotImplementedException();
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            entry = Entries.FirstOrDefault(e => e.Id == entryId);
            return entry != null;
        }
    }
}