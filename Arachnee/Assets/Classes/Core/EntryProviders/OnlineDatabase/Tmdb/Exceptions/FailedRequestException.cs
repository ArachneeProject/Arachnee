using System;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.Exceptions
{
    public class FailedRequestException : Exception
    {
        public FailedRequestException(string message) : base(message)
        {
        }
    }
}