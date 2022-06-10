using FlickrNet;
using FlickrNet.CollectionModels;
using FlickrNet.Models;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for StatsGetReferrerTests
    /// </summary>
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class StatsGetReferrerTests : BaseTest
    {
        private string collectionId = "78188-72157600072356354";
        private string photoId = "5890800";
        private string photosetId = "1493109";
        private readonly DateTime lastWeek = DateTime.Today.AddDays(-7);

        [Test]
        public async Task StatsGetPhotoReferrersBasicTest(CancellationToken cancellationToken = default)
        {
            string domain = "flickr.com";

            Flickr f = AuthInstance;

            StatReferrerCollection referrers = await f.StatsGetPhotoReferrersAsync(lastWeek, domain, 1, 10, cancellationToken);

            Assert.IsNotNull(referrers, "StatReferrers should not be null.");

            Assert.AreNotEqual(0, referrers.Total, "StatReferrers.Total should not be zero.");

            Assert.AreEqual(referrers.Count, Math.Min(referrers.Total, referrers.PerPage), "Count should either be equal to Total or PerPage.");

            Assert.AreEqual(domain, referrers.DomainName, "StatReferrers.Domain should be the same as the searched for domain.");

            foreach (StatReferrer referrer in referrers)
            {
                Assert.IsNotNull(referrer.Url, "StatReferrer.Url should not be null.");
                Assert.AreNotEqual(0, referrer.Views, "StatReferrer.Views should be greater than zero.");
            }

            // Overloads
            referrers = await f.StatsGetPhotoReferrersAsync(lastWeek, domain, cancellationToken);
            Assert.IsNotNull(referrers);

            referrers = await f.StatsGetPhotoReferrersAsync(lastWeek, domain, photoId, cancellationToken);
            Assert.IsNotNull(referrers);

            referrers = await f.StatsGetPhotoReferrersAsync(lastWeek, domain, photoId, 1, 10, cancellationToken);
            Assert.IsNotNull(referrers);
        }

        [Test]
        public async Task StatsGetPhotosetsReferrersBasicTest(CancellationToken cancellationToken = default)
        {
            string domain = "flickr.com";

            Flickr f = AuthInstance;

            StatReferrerCollection referrers = await f.StatsGetPhotosetReferrersAsync(lastWeek, domain, 1, 10, cancellationToken);

            Assert.IsNotNull(referrers, "StatReferrers should not be null.");

            // I often get 0 referrers for a particular given date. As this method only works for the previous 28 days I cannot pick a fixed date.
            // Therefore we cannot confirm that regerrers.Total is always greater than zero.

            Assert.AreEqual(referrers.Count, Math.Min(referrers.Total, referrers.PerPage), "Count should either be equal to Total or PerPage.");

            if (referrers.Total == 0) return;

            Assert.AreEqual(domain, referrers.DomainName, "StatReferrers.Domain should be the same as the searched for domain.");

            foreach (StatReferrer referrer in referrers)
            {
                Assert.IsNotNull(referrer.Url, "StatReferrer.Url should not be null.");
                Assert.AreNotEqual(0, referrer.Views, "StatReferrer.Views should be greater than zero.");
            }

            // Overloads
            referrers = await f.StatsGetPhotosetReferrersAsync(lastWeek, domain, cancellationToken);
            Assert.IsNotNull(referrers);

            referrers = await f.StatsGetPhotosetReferrersAsync(lastWeek, domain, photosetId, cancellationToken);
            Assert.IsNotNull(referrers);

            referrers = await f.StatsGetPhotosetReferrersAsync(lastWeek, domain, photosetId, 1, 10, cancellationToken);
            Assert.IsNotNull(referrers);
        }

        [Test]
        public async Task StatsGetPhotostreamReferrersBasicTest(CancellationToken cancellationToken = default)
        {
            string domain = "flickr.com";

            Flickr f = AuthInstance;

            StatReferrerCollection referrers = await f.StatsGetPhotostreamReferrersAsync(lastWeek, domain, 1, 10, cancellationToken);

            Assert.IsNotNull(referrers, "StatReferrers should not be null.");

            // I often get 0 referrers for a particular given date. As this method only works for the previous 28 days I cannot pick a fixed date.
            // Therefore we cannot confirm that regerrers.Total is always greater than zero.

            Assert.AreEqual(referrers.Count, Math.Min(referrers.Total, referrers.PerPage), "Count should either be equal to Total or PerPage.");

            if (referrers.Total == 0) return;

            Assert.AreEqual(domain, referrers.DomainName, "StatReferrers.Domain should be the same as the searched for domain.");

            foreach (StatReferrer referrer in referrers)
            {
                Assert.IsNotNull(referrer.Url, "StatReferrer.Url should not be null.");
                Assert.AreNotEqual(0, referrer.Views, "StatReferrer.Views should be greater than zero.");
            }

            // Overloads
            referrers = await f.StatsGetPhotostreamReferrersAsync(lastWeek, domain, cancellationToken);
            Assert.IsNotNull(referrers);
        }

        [Test]
        public async Task StatsGetCollectionReferrersBasicTest(CancellationToken cancellationToken = default)
        {
            string domain = "flickr.com";

            Flickr f = AuthInstance;

            var referrers = await f.StatsGetCollectionReferrersAsync(lastWeek, domain, 1, 10, cancellationToken);

            Assert.IsNotNull(referrers, "StatReferrers should not be null.");

            Assert.AreEqual(referrers.Count, Math.Min(referrers.Total, referrers.PerPage), "Count should either be equal to Total or PerPage.");

            if (referrers.Total == 0 && referrers.Pages == 0) return;

            Assert.AreEqual(domain, referrers.DomainName, "StatReferrers.Domain should be the same as the searched for domain.");

            foreach (StatReferrer referrer in referrers)
            {
                Assert.IsNotNull(referrer.Url, "StatReferrer.Url should not be null.");
                Assert.AreNotEqual(0, referrer.Views, "StatReferrer.Views should be greater than zero.");
            }

            // Overloads
            referrers = await f.StatsGetCollectionReferrersAsync(lastWeek, domain, cancellationToken);
            Assert.IsNotNull(referrers);

            referrers = await f.StatsGetCollectionReferrersAsync(lastWeek, domain, collectionId, cancellationToken);
            Assert.IsNotNull(referrers);

            referrers = await f.StatsGetCollectionReferrersAsync(lastWeek, domain, collectionId, 1, 10, cancellationToken);
            Assert.IsNotNull(referrers);
        }
    }
}