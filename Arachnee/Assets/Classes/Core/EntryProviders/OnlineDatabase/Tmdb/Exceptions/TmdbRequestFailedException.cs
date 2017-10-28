namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.Exceptions
{
    public class TmdbRequestFailedException : FailedRequestException
    {
        public TmdbRequestFailedException(string message) : base(message)
        {
        }
    }
}