using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

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
        public async Task StatsGetPhotoReferrersBasicTest()
        {
            string domain = "flickr.com";

            Flickr f = AuthInstance;

            StatReferrerCollection referrers = await f.StatsGetPhotoReferrersAsync(lastWeek, domain, 1, 10);

            Assert.That(referrers, Is.Not.Null, "StatReferrers should not be null.");
            Assert.Multiple(async () =>
            {
                Assert.That(referrers.Total, Is.Not.EqualTo(0), "StatReferrers.Total should not be zero.");
                Assert.Multiple(() =>
                    {
                        Assert.That(Math.Min(referrers.Total, referrers.PerPage), Is.EqualTo(referrers.Count), "Count should either be equal to Total or PerPage.");

                        Assert.That(referrers.DomainName, Is.EqualTo(domain), "StatReferrers.Domain should be the same as the searched for domain.");
                    });
                foreach (StatReferrer referrer in referrers)
                {
                    Assert.Multiple(() =>
                    {
                        Assert.That(referrer.Url, Is.Not.Null, "StatReferrer.Url should not be null.");
                        Assert.That(referrer.Views, Is.Not.EqualTo(0), "StatReferrer.Views should be greater than zero.");
                    });
                }

                // Overloads
                referrers = await f.StatsGetPhotoReferrersAsync(lastWeek, domain);
                Assert.That(referrers, Is.Not.Null);

                referrers = await f.StatsGetPhotoReferrersAsync(lastWeek, domain, photoId);
                Assert.That(referrers, Is.Not.Null);

                referrers = await f.StatsGetPhotoReferrersAsync(lastWeek, domain, photoId, 1, 10);
                Assert.That(referrers, Is.Not.Null);
            });
        }

        [Test]
        public async Task StatsGetPhotosetsReferrersBasicTest()
        {
            string domain = "flickr.com";

            Flickr f = AuthInstance;

            StatReferrerCollection referrers = await f.StatsGetPhotosetReferrersAsync(lastWeek, domain, 1, 10);
            Assert.Multiple(() =>
            {
                Assert.That(referrers, Is.Not.Null, "StatReferrers should not be null.");

                // I often get 0 referrers for a particular given date. As this method only works for the previous 28 days I cannot pick a fixed date.
                // Therefore we cannot confirm that regerrers.Total is always greater than zero.

                Assert.That(Math.Min(referrers.Total, referrers.PerPage), Is.EqualTo(referrers.Count), "Count should either be equal to Total or PerPage.");
            });
            if (referrers.Total == 0) return;

            Assert.That(referrers.DomainName, Is.EqualTo(domain), "StatReferrers.Domain should be the same as the searched for domain.");

            foreach (StatReferrer referrer in referrers)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(referrer.Url, Is.Not.Null, "StatReferrer.Url should not be null.");
                    Assert.That(referrer.Views, Is.Not.EqualTo(0), "StatReferrer.Views should be greater than zero.");
                });
            }

            // Overloads
            referrers = await f.StatsGetPhotosetReferrersAsync(lastWeek, domain);
            Assert.That(referrers, Is.Not.Null);

            referrers = await f.StatsGetPhotosetReferrersAsync(lastWeek, domain, photosetId);
            Assert.That(referrers, Is.Not.Null);

            referrers = await f.StatsGetPhotosetReferrersAsync(lastWeek, domain, photosetId, 1, 10);
            Assert.That(referrers, Is.Not.Null);
        }

        [Test]
        public async Task StatsGetPhotostreamReferrersBasicTest()
        {
            string domain = "flickr.com";

            Flickr f = AuthInstance;

            StatReferrerCollection referrers = await f.StatsGetPhotostreamReferrersAsync(lastWeek, domain, 1, 10);
            Assert.Multiple(() =>
            {
                Assert.That(referrers, Is.Not.Null, "StatReferrers should not be null.");

                // I often get 0 referrers for a particular given date. As this method only works for the previous 28 days I cannot pick a fixed date.
                // Therefore we cannot confirm that regerrers.Total is always greater than zero.

                Assert.That(Math.Min(referrers.Total, referrers.PerPage), Is.EqualTo(referrers.Count), "Count should either be equal to Total or PerPage.");
            });
            if (referrers.Total == 0) return;

            Assert.That(referrers.DomainName, Is.EqualTo(domain), "StatReferrers.Domain should be the same as the searched for domain.");

            foreach (StatReferrer referrer in referrers)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(referrer.Url, Is.Not.Null, "StatReferrer.Url should not be null.");
                    Assert.That(referrer.Views, Is.Not.EqualTo(0), "StatReferrer.Views should be greater than zero.");
                });
            }

            // Overloads
            referrers = await f.StatsGetPhotostreamReferrersAsync(lastWeek, domain);
            Assert.That(referrers, Is.Not.Null);
        }

        [Test]
        public async Task StatsGetCollectionReferrersBasicTest()
        {
            string domain = "flickr.com";

            Flickr f = AuthInstance;

            var referrers = await f.StatsGetCollectionReferrersAsync(lastWeek, domain, 1, 10);
            Assert.Multiple(() =>
            {
                Assert.That(referrers, Is.Not.Null, "StatReferrers should not be null.");

                Assert.That(Math.Min(referrers.Total, referrers.PerPage), Is.EqualTo(referrers.Count), "Count should either be equal to Total or PerPage.");
            });
            if (referrers.Total == 0 && referrers.Pages == 0) return;

            Assert.That(referrers.DomainName, Is.EqualTo(domain), "StatReferrers.Domain should be the same as the searched for domain.");

            foreach (StatReferrer referrer in referrers)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(referrer.Url, Is.Not.Null, "StatReferrer.Url should not be null.");
                    Assert.That(referrer.Views, Is.Not.EqualTo(0), "StatReferrer.Views should be greater than zero.");
                });
            }

            // Overloads
            referrers = await f.StatsGetCollectionReferrersAsync(lastWeek, domain);
            Assert.That(referrers, Is.Not.Null);

            referrers = await f.StatsGetCollectionReferrersAsync(lastWeek, domain, collectionId);
            Assert.That(referrers, Is.Not.Null);

            referrers = await f.StatsGetCollectionReferrersAsync(lastWeek, domain, collectionId, 1, 10);
            Assert.That(referrers, Is.Not.Null);
        }
    }
}