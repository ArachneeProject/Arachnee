using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Builders
{
    public class MovieBuilder : MediaBuilder<Movie>
    {
        protected override string ResourceAddress
        {
            get { return "movie/{id}"; }
        }
    }
}