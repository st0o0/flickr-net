﻿using System;

using NUnit.Framework;
using FlickrNet;
using System.IO;
using FlickrNet.Common;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for CacheTests
    /// </summary>
    [TestFixture]
    public class CacheTests : BaseTest
    {
        [Test]
        public void CacheLocationTest()
        {
            string origLocation = Flickr.CacheLocation;

            Console.WriteLine(origLocation);

            string newLocation = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            Flickr.CacheLocation = newLocation;

            Assert.AreEqual(Flickr.CacheLocation, newLocation);

            Flickr.CacheLocation = origLocation;

            Assert.AreEqual(Flickr.CacheLocation, origLocation);
        }

        [Test]
        public async Task CacheHitTest(CancellationToken cancellationToken = default)
        {
            if (Directory.Exists(Flickr.CacheLocation))
            {
                Directory.Delete(Flickr.CacheLocation, true);
            }

            Flickr f = Instance;
            Flickr.FlushCache();
            f.InstanceCacheDisabled = false;

            await f.PeopleGetPublicPhotosAsync(TestData.TestUserId, cancellationToken);

            string lastUrl = f.LastRequest;

            ICacheItem item = Cache.Responses.Get(lastUrl, TimeSpan.MaxValue, false);

            Assert.IsNotNull(item, "Cache should now contain the item.");
            Assert.IsInstanceOf<ResponseCacheItem>(item);

            var response = item as ResponseCacheItem;

            Assert.IsNotNull(response.Url, "Url should not be null.");
            Assert.AreEqual(lastUrl, response.Url.AbsoluteUri, "Url should match the url requested from the cache.");
        }
    }
}