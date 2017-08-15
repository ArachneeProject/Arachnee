using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using UnityEngine;

namespace Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients
{
    public class MultiSearchClient : TmdbClient
    {
        public IEnumerable<SearchResult> RunSearch(string searchQuery)
        {
            var request = new RestRequest("search/multi", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddParameter("query", searchQuery);
            request.AddParameter("api_key", Constant.ApiKey);

            string response;
            if (!TryExecuteRequest(request, out response))
            {
                return new List<SearchResult>(); 
            }
            
            var obj = JObject.Parse(response).Value<JArray>("results");
            return JsonConvert.DeserializeObject<List<SearchResult>>(obj.ToString());
        }
    }
}