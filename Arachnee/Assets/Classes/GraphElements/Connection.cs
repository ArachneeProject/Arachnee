using System;

namespace Assets.Classes.GraphElements
{
    public class Connection
    {
        private const string Separator = ":";

        public string ConnectedId { get; set; }

        public ConnectionFlags Flags { get; set; }

        public static string GetIdentifier(string fromEntryId, string toEntryId, ConnectionFlags flags)
        {
            string flagsId = Convert.ToString((int) flags, 2);
            if (string.Compare(fromEntryId, toEntryId, StringComparison.OrdinalIgnoreCase) < 0)
            {
                return fromEntryId + Separator + flagsId + Separator + toEntryId;
            }

            return toEntryId + Separator + flagsId + Separator + fromEntryId;
        }
    }
}
