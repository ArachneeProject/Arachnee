using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.Exceptions;
using Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb.TmdbObjects;
using Newtonsoft.Json;
using RestSharp;

namespace Assets.Classes.Core.EntryProviders.OnlineDatabase.Tmdb
{
    public class TmdbClient
    {
        private readonly RestClient _tmdbClient = new RestClient("https://api.themoviedb.org/3/");
        private readonly RestClient _imageClient = new RestClient("https://image.tmdb.org/t/p/");
        
        public TmdbClient()
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallback;
        }

        /// <summary>
        /// Search multiple models in a single request (currently supports searching for movies, tv shows and people).
        /// </summary>
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

        /// <summary>
        /// Get the primary informations about a movie.
        /// </summary>
        public TmdbMovie GetMovie(ulong tmdbMovieId)
        {
            var request = new RestRequest("movie/{id}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddUrlSegment("id", tmdbMovieId.ToString());
            request.AddQueryParameter("append_to_response", "credits,images");
            request.AddQueryParameter("api_key", Constant.ApiKey);

            var response = ExecuteRequest(request);

            var movie = JsonConvert.DeserializeObject<TmdbMovie>(response, TmdbJsonSettings.Instance);
            if (movie.Id == default(ulong))
            {
                throw new InvalidTmdbRequestException($"\"{tmdbMovieId}\" didn't return any result.");
            }

            return movie;
        }

        /// <summary>
        /// Get the primary person details.
        /// </summary>
        public TmdbPerson GetPerson(ulong tmdbPersonId)
        {
            var request = new RestRequest("person/{id}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddUrlSegment("id", tmdbPersonId.ToString());
            request.AddQueryParameter("append_to_response", "combined_credits,images");
            request.AddQueryParameter("api_key", Constant.ApiKey);

            var response = ExecuteRequest(request);

            var person = JsonConvert.DeserializeObject<TmdbPerson>(response, TmdbJsonSettings.Instance);
            if (person.Id == default(ulong))
            {
                throw new InvalidTmdbRequestException($"\"{tmdbPersonId}\" didn't return any result.");
            }

            return person;
        }

        /// <summary>
        /// The list of official jobs that are used on TMDb.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get the system wide configuration information.
        /// </summary>
        /// <returns></returns>
        public Configuration GetConfiguration()
        {
            var request = new RestRequest("configuration", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddQueryParameter("api_key", Constant.ApiKey);

            var responseContent = ExecuteRequest(request);
            var config = JsonConvert.DeserializeObject<Configuration>(responseContent, TmdbJsonSettings.Instance);

            return config;
        }

        public byte[] GetImage(string fileSize, string filePath)
        {
            if (string.IsNullOrWhiteSpace(fileSize))
            {
                throw new ArgumentNullException(nameof(fileSize), "File size cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path cannot be empty.");
            }
            
            var request = new RestRequest($"{fileSize}{filePath}", Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            var response = _imageClient.Execute(request);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new FailedRequestException(response.ErrorMessage);
            }

            if (response.ErrorException != null)
            {
                throw new TmdbRequestFailedException(response.ErrorException.Message);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new TmdbRequestFailedException(response.Content);
            }

            return response.RawBytes;
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