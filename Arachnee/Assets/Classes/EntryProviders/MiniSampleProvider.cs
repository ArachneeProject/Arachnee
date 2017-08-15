using System.Collections.Generic;
using System.Linq;
using Assets.Classes.GraphElements;
using UnityEngine;

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
                    Id = "218",
                    Title = "The Terminator",
                    Connections = new List<Connection>
                    {
                        // cameron
                        new Connection
                        {
                            Flags = ConnectionFlags.Actor | ConnectionFlags.Director,
                            Id = "218-2710",
                            Left = "218",
                            Right = "2710"
                        },
                        // schwarzenegger
                        new Connection
                        {
                            Flags = ConnectionFlags.Actor,
                            Id = "218-1100",
                            Left = "218",
                            Right = "1100"
                        }
                    }
                },
                new Artist
                {
                    Id = "1100",
                    LastName = "Schwarzenegger",
                    Connections = new List<Connection>
                    {
                        new Connection
                        {
                            Flags = ConnectionFlags.Actor,
                            Id = "218-1100",
                            Left = "218",
                            Right = "1100"
                        }
                    }
                },
                new Artist
                {
                    Id = "2710",
                    LastName = "Cameron",
                    Connections = new List<Connection>
                    {
                        new Connection
                        {
                            Flags = ConnectionFlags.Actor | ConnectionFlags.Director,
                            Id = "218-2710",
                            Left = "218",
                            Right = "2710"
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Returns a stack with only one random item on top of it.
        /// </summary>
        public override Stack<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            var items = Entries.OfType<TEntry>().ToList();
            var randomItem = items.ElementAt(Random.Range(0, items.Count - 1));
            return new Stack<TEntry>(new [] { randomItem });
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            entry = Entries.FirstOrDefault(e => e.Id == entryId);
            return entry != null;
        }
    }
}