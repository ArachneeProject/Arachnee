using System;
using System.Collections.Generic;
using System.Reflection;

namespace Arachnee.GraphElements
{
    public abstract class Entry
    {
        public string Id { get; set; }

        public List<Connection> Connections { get; set; }

        public static bool TryGetEntryType(string entryId, out Type type)
        {
            throw new NotImplementedException();
        }
    }
}