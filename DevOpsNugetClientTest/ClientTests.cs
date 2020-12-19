using DevOpsNugetClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevOpsNugetClientTest
{
    /// <summary>
    /// Tests for the Client class
    /// </summary>
    /// <remarks>
    /// For the tests to work you must add the required values in
    /// the User Secrets file for Organisation and PersonalAccessToken (PAT)
    /// see https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page
    /// Since this library uses the package feeds, ensure the PAT has access to
    /// the "Packaging" feeds. If you only wish to read, just select the Read option.
    /// </remarks>
    [TestClass]
    public class ClientTests
    {
        /// <summary>
        /// Lazy client
        /// </summary>
        private readonly Lazy<Client> client = new Lazy<Client>(() =>
        {
            // load configuration from user secrets
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            var org = configuration.GetValue<string>("Organisation");
            if (string.IsNullOrEmpty(org))
                throw new ArgumentNullException("No Organisation value set");

            var token = configuration.GetValue<string>("PersonalAccessToken");
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("No PersonalAccessToken value set");

            return new Client(org, token);
        });

        /// <summary>
        /// Try to read the list of feeds
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetFeedsAsync_Test()
        {
            var result = await client.Value.GetFeedsAsync();

            foreach (var feed in result)
            {
                Console.WriteLine($"{feed.name}: {feed.description}");
            }
            Assert.IsTrue(result.Any(), "No feeds found");
        }

        /// <summary>
        /// Read all packages on a named feed
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetPackagesAsync_Test()
        {
            const string feed = "YourFeedName";

            var result = await client.Value.GetPackagesAsync(feed);

            foreach (var pkg in result)
            {
                Console.WriteLine($"{pkg.name}: {pkg.id}");
            }
            Assert.IsTrue(result.Any(), "No packages found");
        }

        /// <summary>
        /// Read all packages on a feed with the search term
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetPackagesAsync_TestWithNameQuery()
        {
            const string feed = "YourFeedName";
            const string packageNameQuery = "YourPackageName";

            var result = await client.Value.GetPackagesAsync(feed, packageNameQuery);

            foreach (var pkg in result)
            {
                Console.WriteLine($"{pkg.name}: {pkg.id}");
            }
            Assert.IsTrue(result.Any(), "No packages found");
        }

        /// <summary>
        /// Read all versions for a specific package
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetPackageVersionsAsync_Test()
        {
            const string feed = "YourFeedName";
            const string packageId = "YourPackageId";

            var result = await client.Value.GetPackageVersionsAsync(feed, packageId);

            foreach (var ver in result)
            {
                Console.WriteLine($"{ver.version}: {ver.description}  {(ver.isDeleted ? "(deleted)" : "")}");
            }
            Assert.IsTrue(result.Any(), "No package versions found");
        }

        /// <summary>
        /// The GetPackageVersion API only accepts GUIDs
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetPackageVersionsAsync_TestNonGuidIsRejected()
        {
            const string feed = "YourFeedName";

            // packageId should be a guid, not a name
            const string packageId = "YourPackageName";

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                var result = await client.Value.GetPackageVersionsAsync(feed, packageId);
            });
        }

    }
}
