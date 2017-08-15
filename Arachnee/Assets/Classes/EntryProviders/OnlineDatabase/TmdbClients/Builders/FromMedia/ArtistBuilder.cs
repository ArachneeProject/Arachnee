using System;
using Assets.Classes.GraphElements;

namespace Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders.FromMedia
{
    public class ArtistBuilder : MediaBuilder
    {
        protected override string ResourceAddress
        {
            get { throw new NotImplementedException(); }
        }
        
        protected override bool TryDeserialize(string jsonString, out Entry entry)
        {
            throw new NotImplementedException();
        }
    }
}