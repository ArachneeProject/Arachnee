using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.Exceptions;
using Assets.Classes.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects;
using Newtonsoft.Json;
using RestSharp;

namespace Assets.Classes.EntryProviders.OnlineDatabase.Tmdb
{
    public class TmdbClient
    {
        private readonly RestClient _tmdbClient = new RestClient("https://api.themoviedb.org/3/");

        public TmdbClient()
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallback;
        }

        public List<CombinedResult> GetCombinedSearchResults(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return new List<CombinedResult>();
            }

            var request = new RestRequest("search/multi", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddQueryParameter("query", searchQuery);
            request.AddQueryParameter("api_key", Constant.ApiKey);

            var responseContent = ExecuteRequest(request);
            var firstPage = JsonConvert.DeserializeObject<SearchResultPage>(responseContent, TmdbJsonSettings.Instance);

            return firstPage.Results;
        }

        public TmdbMovie GetMovie(string tmdbMovieId)
        {
            long id;
            if (!long.TryParse(tmdbMovieId, out id))
            {
                throw new InvalidTmdbRequestException($"\"{tmdbMovieId}\" is not a valid movie id.");
            }

            var request = new RestRequest("movie/{id}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddUrlSegment("id", tmdbMovieId);
            request.AddQueryParameter("append_to_response", "credits");
            request.AddQueryParameter("api_key", Constant.ApiKey);

            var response = ExecuteRequest(request);

            var movie = JsonConvert.DeserializeObject<TmdbMovie>(response, TmdbJsonSettings.Instance);
            if (movie.Id == default(long))
            {
                throw new InvalidTmdbRequestException($"\"{tmdbMovieId}\" didn't return any result.");
            }

            return movie;
        }
        
        public TmdbPerson GetPerson(string tmdbPersonId)
        {
            long id;
            if (!long.TryParse(tmdbPersonId, out id))
            {
                throw new InvalidTmdbRequestException($"\"{tmdbPersonId}\" is not a valid person id.");
            }

            var request = new RestRequest("person/{id}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddUrlSegment("id", tmdbPersonId);
            request.AddQueryParameter("append_to_response", "combined_credits");
            request.AddQueryParameter("api_key", Constant.ApiKey);

            var response = ExecuteRequest(request);

            var person = JsonConvert.DeserializeObject<TmdbPerson>(response, TmdbJsonSettings.Instance);
            if (person.Id == default(long))
            {
                throw new InvalidTmdbRequestException($"\"{tmdbPersonId}\" didn't return any result.");
            }

            return person;
        }

        public List<string> GetAllJobs()
        {
            var request = new RestRequest("job/list", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };
            
            request.AddQueryParameter("api_key", Constant.ApiKey);

            var responseContent = ExecuteRequest(request);
            var officialList = JsonConvert.DeserializeObject<OfficialJobList>(responseContent, TmdbJsonSettings.Instance);

            return officialList.Jobs.SelectMany(j => j.JobList).ToList();
        }

        private string ExecuteRequest(IRestRequest request)
        {
            var response = _tmdbClient.Execute(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new FailedRequestException(response.ErrorMessage);
            }
            if (response.ErrorException != null)
            {
                throw new TmdbRequestFailedException(response.ErrorException.Message);
            }

            return response.Content;
        }

        // TODO: better understand what this code do
        private static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (var i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        var chainIsValid = chain.Build((X509Certificate2) certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }
    }
}