using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders
{
    public abstract class EntryBuilder : TmdbClient
    {
        protected abstract string ResourceAddress { get; }

        public abstract bool TryToBuild(string id, out Entry entry);
    }
}