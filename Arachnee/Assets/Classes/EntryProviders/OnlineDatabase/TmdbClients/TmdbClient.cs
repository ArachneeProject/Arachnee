using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using RestSharp;
using UnityEngine;

namespace Assets.Classes.EntryProviders.OnlineDatabase.TmdbClients
{
    public class TmdbClient
    {
        public const string IdSeparator = "-";
        private readonly RestClient _tmdbClient = new RestClient("https://api.themoviedb.org/3/");

        public TmdbClient()
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallback;
        }

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

        // TODO: better understand what this code do
        private static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
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