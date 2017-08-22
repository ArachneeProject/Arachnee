using Assets.Classes.EntryProviders.OnlineProvider;
using Assets.Classes.GraphElements;
using RestSharp;

namespace Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients.Builders.FromMedia
{
    public abstract class MediaBuilder : EntryBuilder
    {
        public override bool TryToBuild(string id, out Entry entry)
        {
            var request = new RestRequest(ResourceAddress, Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddUrlSegment("id", id);
            request.AddParameter("api_key", Constant.ApiKey);

            string result;
            if (!base.TryExecuteRequest(request, out result))
            {
                entry = DefaultEntry.Instance;
                return false;
            }

            return TryDeserialize(result, out entry);
        }

        protected abstract bool TryDeserialize(string jsonString, out Entry entry);
    }
}