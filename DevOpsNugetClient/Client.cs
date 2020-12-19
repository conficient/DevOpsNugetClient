using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.SystemTextJson;
using System;
using System.Text.Json;

namespace DevOpsNugetClient
{
    public class Client
    {

        readonly IRestClient _feedClient;
        readonly IRestClient _pkgsClient;

        public Client(string organisation, string personalAccessToken)
        {
            if (string.IsNullOrWhiteSpace(organisation))
                throw new ArgumentNullException(nameof(organisation), "Organisation is required");
            if (string.IsNullOrWhiteSpace(personalAccessToken))
                throw new ArgumentNullException(nameof(personalAccessToken), "PAT is required");

            var opts = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // set Feed URL
            _feedClient = new RestClient($"https://feeds.dev.azure.com/{organisation}")
                .UseSystemTextJson(opts);
            // set Pkgs URL
            _pkgsClient = new RestClient($"https://pkgs.dev.azure.com/{organisation}")
                .UseSystemTextJson(opts);

            // set access using PAT
            var auth = new HttpBasicAuthenticator("", personalAccessToken);
            _feedClient.Authenticator = auth;
            _pkgsClient.Authenticator = auth;
        }


        const string versionParam = "api-version";
        const string version = "6.0-preview.1";

        /// <summary>
        /// Make a GET query returning type T to the feed API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        protected T FeedGetAsync<T>(RestRequest req) 
        {
            // add the api version
            req.AddQueryParameter(versionParam, version);

            return _feedClient.ExecuteGetAsync<ResultSet<T>>(req);

        }
    }
}
