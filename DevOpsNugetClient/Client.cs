using DevOpsClient;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevOpsNugetClient
{
    public class Client
    {
        #region Ctor

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

        #endregion

        #region Helper methods


        const string versionParam = "api-version";
        const string version = "6.0-preview.1";

        /// <summary>
        /// Make a GET query to the feed API returning type T
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="req">request</param>
        /// <returns></returns>
        protected async Task<T> FeedGetAsync<T>(RestRequest req) 
        {
            // add the api version as a query parameter
            req.AddQueryParameter(versionParam, version);

            var result = await _feedClient.ExecuteGetAsync<ResultSet<T>>(req);

            if (!result.IsSuccessful)
                throw new RestException(result);

            return result.Data.Value;
        }
        
        #endregion

        /// <summary>
        /// Get a list of feeds for the Organisation
        /// </summary>
        /// <returns></returns>
        public async Task<List<Feed>> GetFeedsAsync()
        {
            // source: https://docs.microsoft.com/en-us/rest/api/azure/devops/artifacts/feed%20%20management/get%20feeds?view=azure-devops-rest-6.0
            const string url = "/_apis/packaging/feeds";
            var request = new RestRequest(url);
            return await FeedGetAsync<List<Feed>>(request);
        }

        /// <summary>
        /// Get a list of packages for the feed
        /// </summary>
        /// <param name="feedId">Feed Id or Name</param>
        /// <param name="packageNameQuery">optional - filter for package name</param>
        /// <returns>a list of Packages found</returns>
        public async Task<List<Package>> GetPackagesAsync(string feedId, string packageNameQuery = null)
        {
            // source: https://docs.microsoft.com/en-us/rest/api/azure/devops/artifacts/artifact%20%20details/get%20packages?view=azure-devops-rest-6.0
            string url = $"/_apis/packaging/feeds/{feedId}/packages";
            var request = new RestRequest(url);
            // add package name filter
            if (!string.IsNullOrEmpty(packageNameQuery))
                request.AddQueryParameter(nameof(packageNameQuery), packageNameQuery);
            return await FeedGetAsync<List<Package>>(request);
        }

        /// <summary>
        /// Get the versions for a packageId
        /// </summary>
        /// <param name="feedId">feed name or id</param>
        /// <param name="packageId">pacakge GUID (_NOT_ NAME)</param>
        /// <returns></returns>
        public async Task<List<PackageVersion>> GetPackageVersionsAsync(string feedId, string packageId)
        {
            if (!Guid.TryParse(packageId, out Guid guid))
                throw new ArgumentException(nameof(packageId), "PackageId must be a package Id (GUID) not a name");
            // source: https://docs.microsoft.com/en-us/rest/api/azure/devops/artifacts/artifact%20%20details/get%20package%20versions?view=azure-devops-rest-6.0
            string url = $"/_apis/packaging/feeds/{feedId}/packages/{packageId}/versions";
            var request = new RestRequest(url);
            return await FeedGetAsync<List<PackageVersion>>(request);
        }

        /// <summary>
        /// Delete a single version of a NUGET package 
        /// </summary>
        /// <param name="feedId">The FeedId</param>
        /// <param name="packageName">Use the package NAME not the GUID/ID</param>
        /// <param name="semVer">Use the semver, e.g. 1.2.3.4</param>
        /// <returns></returns>
        public async Task<bool> DeletePackageVersionAsync(string feedId, string packageName, string semVer)
        {
            // Source: https://docs.microsoft.com/en-us/rest/api/azure/devops/artifactspackagetypes/nuget/delete%20package%20version?view=azure-devops-rest-6.0
            string url = $"/_apis/packaging/feeds/{feedId}/nuget/packages/{packageName}/versions/{semVer}";
            var request = new RestRequest(url)
            {
                Method = Method.DELETE
            };
            // add version
            request.AddQueryParameter(versionParam, version);
            var result = await _pkgsClient.ExecuteAsync(request);
            return result.IsSuccessful;
        }

    }
}
