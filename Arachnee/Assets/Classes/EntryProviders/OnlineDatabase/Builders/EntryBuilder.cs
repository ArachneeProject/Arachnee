using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Builders
{
    public abstract class EntryBuilder : TmdbClient
    {
        public abstract bool TryToBuild(string id, out Entry entry);
    }
}