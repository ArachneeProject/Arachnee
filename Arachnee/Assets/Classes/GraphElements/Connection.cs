using System;

namespace Assets.Classes.GraphElements
{
    public class Connection
    {
        public string ConnectedId { get; set; }

        public ConnectionFlags Flags { get; set; }

        public static string GetIdentifier(string fromEntryId, string toEntryId, ConnectionFlags flags)
        {
            string flagsId = Convert.ToString((int) flags, 2);
            return string.Compare(fromEntryId, toEntryId, StringComparison.OrdinalIgnoreCase) < 0 
                ? $"{fromEntryId}::{flagsId}::{toEntryId}" 
                : $"{toEntryId}::{flagsId}::{fromEntryId}";
        }
    }
}
