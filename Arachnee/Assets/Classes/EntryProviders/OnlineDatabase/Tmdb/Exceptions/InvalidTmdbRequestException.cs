using System;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.Exceptions
{
    public class InvalidTmdbRequestException : Exception
    {
        public InvalidTmdbRequestException(string message) : base(message)
        {
        }
    }
}