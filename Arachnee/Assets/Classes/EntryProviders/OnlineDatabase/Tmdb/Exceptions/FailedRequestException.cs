using System;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.Exceptions
{
    public class FailedRequestException : Exception
    {
        public FailedRequestException(string message) : base(message)
        {
        }
    }
}