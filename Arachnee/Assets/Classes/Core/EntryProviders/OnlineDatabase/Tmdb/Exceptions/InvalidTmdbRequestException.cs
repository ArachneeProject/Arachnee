using System;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.Exceptions
{
    public class InvalidTmdbRequestException : Exception
    {
        public InvalidTmdbRequestException(string message) : base(message)
        {
        }
    }
}