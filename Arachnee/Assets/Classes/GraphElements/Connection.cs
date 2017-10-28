using System;

namespace Assets.Classes.GraphElements
{
    public class Connection
    {
        public string ConnectedId { get; set; }

        public ConnectionType Type { get; set; }

        public string Label { get; set; }

        // TODO: should be moved to ConnectionView class
        public static string GetIdentifier(string fromEntryId, string toEntryId, ConnectionType type)
        {
            return string.Compare(fromEntryId, toEntryId, StringComparison.OrdinalIgnoreCase) < 0
                ? $"{fromEntryId}::{type}::{toEntryId}"
                : $"{toEntryId}::{type}::{fromEntryId}";
        }
    }
}
