using RestSharp;
using UnityEngine;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Builders
{
    public abstract class TmdbClient
    {
        private readonly RestClient _tmdbClient = new RestClient("https://api.themoviedb.org/3/");

        protected bool TryExecuteRequest(IRestRequest request, out string result)
        {
            var response = _tmdbClient.Execute(request);
            if (response.ErrorException != null)
            {
                Debug.LogError("Error retrieving response: " + response.ErrorMessage);
                result = string.Empty;
                return false;
            }

            result = response.Content;
            return true;
        }
    }
}