using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders.FromCollection
{
    public abstract class FromCollectionBuilder : EntryBuilder
    {
        public override bool TryToBuild(string id, out Entry entry)
        {
            throw new System.NotImplementedException();
        }
    }
}