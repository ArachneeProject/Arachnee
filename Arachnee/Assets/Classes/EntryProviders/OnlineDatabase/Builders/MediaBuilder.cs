using Assets.Classes.GraphElements;
using Newtonsoft.Json;
using RestSharp;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Builders
{
    public abstract class MediaBuilder<TEntry> : EntryBuilder where TEntry : Entry
    {
        protected abstract string ResourceAddress { get; }

        public override bool TryToBuild(string id, out Entry entry)
        {
            var request = new RestRequest(ResourceAddress, Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("id", id);
            request.AddQueryParameter("api_key", Constant.ApiKey);

            string result;
            if (!base.TryExecuteRequest(request, out result))
            {
                entry = DefaultEntry.Instance;
                return false;
            }

            entry = JsonConvert.DeserializeObject<TEntry>(result);
            return true;
        }
    }
}