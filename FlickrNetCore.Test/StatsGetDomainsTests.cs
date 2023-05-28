using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class StatsGetDomainsTests : BaseTest
    {
        private string collectionId = "78188-72157600072356354";
        private string photoId = "5890800";
        private string photosetId = "1493109";

        [Test]
        public async Task StatsGetCollectionDomainsBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var domains = await f.StatsGetCollectionDomainsAsync(DateTime.Today.AddDays(-2), cancellationToken);

            Assert.IsNotNull(domains, "StatDomains should not be null.");
            Assert.AreEqual(domains.Total, domains.Count, "StatDomains.Count should be the same as StatDomains.Total");

            // Overloads
            domains = await f.StatsGetCollectionDomainsAsync(DateTime.Today.AddDays(-2), collectionId, cancellationToken);
            Assert.IsNotNull(domains);

            domains = await f.StatsGetCollectionDomainsAsync(DateTime.Today.AddDays(-2), 1, 10, cancellationToken);
            Assert.IsNotNull(domains);

            domains = await f.StatsGetCollectionDomainsAsync(DateTime.Today.AddDays(-2), collectionId, 1, 10, cancellationToken);
            Assert.IsNotNull(domains);
        }

        [Test]
        public async Task StatsGetCollectionStatsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            Stats stats = await f.StatsGetCollectionStatsAsync(DateTime.Today.AddDays(-2), collectionId, cancellationToken);

            Assert.IsNotNull(stats, "Stats should not be null.");
        }

        [Test]
        public async Task StatsGetPhotoDomainsTests(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var domains = await f.StatsGetPhotoDomainsAsync(DateTime.Today.AddDays(-2), cancellationToken);
            Assert.IsNotNull(domains, "StatDomains should not be null.");
            Assert.AreNotEqual(0, domains.Count, "StatDomains.Count should not be zero.");

            foreach (StatDomain domain in domains)
            {
                Assert.IsNotNull(domain.Name, "StatDomain.Name should not be null.");
                Assert.AreNotEqual(0, domain.Views, "StatDomain.Views should not be zero.");
            }

            // Overloads
            domains = await f.StatsGetPhotoDomainsAsync(DateTime.Today.AddDays(-2), photoId, cancellationToken);
            Assert.IsNotNull(domains, "StatDomains should not be null.");

            domains = await f.StatsGetPhotoDomainsAsync(DateTime.Today.AddDays(-2), photoId, 1, 10, cancellationToken);
            Assert.IsNotNull(domains, "StatDomains should not be null.");

            domains = await f.StatsGetPhotoDomainsAsync(DateTime.Today.AddDays(-2), 1, 10, cancellationToken);
            Assert.IsNotNull(domains, "StatDomains should not be null.");
        }

        [Test]
        public async Task StatsGetPhotoStatsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var stats = await f.StatsGetPhotoStatsAsync(DateTime.Today.AddDays(-5), photoId, cancellationToken);

            Assert.IsNotNull(stats);
        }

        [Test]
        public async Task StatsGetPhotosetDomainsBasic(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var domains = await f.StatsGetPhotosetDomainsAsync(DateTime.Today.AddDays(-2), cancellationToken);
            Assert.IsNotNull(domains, "StatDomains should not be null.");

            foreach (StatDomain domain in domains)
            {
                Assert.IsNotNull(domain.Name, "StatDomain.Name should not be null.");
                Assert.AreNotEqual(0, domain.Views, "StatDomain.Views should not be zero.");
            }

            // Overloads
            domains = await f.StatsGetPhotosetDomainsAsync(DateTime.Today.AddDays(-2), 1, 10, cancellationToken);
            Assert.IsNotNull(domains, "StatDomains should not be null.");

            domains = await f.StatsGetPhotosetDomainsAsync(DateTime.Today.AddDays(-2), photosetId, cancellationToken);
            Assert.IsNotNull(domains, "StatDomains should not be null.");

            domains = await f.StatsGetPhotosetDomainsAsync(DateTime.Today.AddDays(-2), photosetId, 1, 10, cancellationToken);
            Assert.IsNotNull(domains, "StatDomains should not be null.");
        }

        [Test]
        public async Task StatsGetPhotosetStatsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var stats = await f.StatsGetPhotosetStatsAsync(DateTime.Today.AddDays(-5), photosetId, cancellationToken);

            Assert.IsNotNull(stats);
        }

        [Test]
        public async Task StatsGetPhotostreamDomainsBasic(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var domains = await f.StatsGetPhotostreamDomainsAsync(DateTime.Today.AddDays(-2), cancellationToken);
            Assert.IsNotNull(domains, "StatDomains should not be null.");

            foreach (StatDomain domain in domains)
            {
                Assert.IsNotNull(domain.Name, "StatDomain.Name should not be null.");
                Assert.AreNotEqual(0, domain.Views, "StatDomain.Views should not be zero.");
            }

            // Overload
            domains = await f.StatsGetPhotostreamDomainsAsync(DateTime.Today.AddDays(-2), 1, 10, cancellationToken);
            Assert.IsNotNull(domains, "StatDomains should not be null.");
        }

        [Test]
        public async Task StatsGetPhotostreamStatsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var stats = await f.StatsGetPhotostreamStatsAsync(DateTime.Today.AddDays(-5), cancellationToken);

            Assert.IsNotNull(stats);
        }
    }
}