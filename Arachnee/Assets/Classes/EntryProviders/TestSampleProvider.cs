using System.Collections.Generic;
using System.Linq;
using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders
{
    public class TestSampleProvider : EntryProvider
    {
        public readonly List<Entry> Entries;

        public TestSampleProvider()
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

        public override Stack<TEntry> GetSearchResults<TEntry>(string searchQuery)
        {
            return new Stack<TEntry>(Entries.OfType<TEntry>());
        }

        protected override bool TryLoadEntry(string entryId, out Entry entry)
        {
            entry = Entries.First(e => e.Id == entryId);
            return true;
        }
    }
}