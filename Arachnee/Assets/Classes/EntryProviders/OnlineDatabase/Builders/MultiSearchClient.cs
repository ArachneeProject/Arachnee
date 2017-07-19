using System.Collections.Generic;
using System.Linq;
using Assets.Classes.GraphElements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Builders
{
    public class MultiSearchClient : TmdbClient
    {
        public IEnumerable<MediaResult> RunSearch(string searchQuery)
        {
            var request = new RestRequest("search/multi", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddQueryParameter("query", searchQuery);
            request.AddQueryParameter("api_key", Constant.ApiKey);

            string response;
            if (!TryExecuteRequest(request, out response))
            {
                return new List<MediaResult>(); 
            }
            
            var obj = JObject.Parse(response).Value<JArray>("results");
            return JsonConvert.DeserializeObject<List<MediaResult>>(obj.ToString());
        }
    }
}