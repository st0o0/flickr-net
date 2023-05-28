﻿using FlickrNetCore;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PhotosGetCountTests
    /// </summary>
    [TestFixture]
    public class PhotosGetCountTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetCountTakenTest()
        {
            Flickr f = AuthInstance;

            var dates = new List<DateTime>();
            var date1 = new DateTime(2009, 1, 12);
            var date2 = new DateTime(2009, 9, 12);
            var date3 = new DateTime(2009, 12, 12);

            dates.Add(date2);
            dates.Add(date1);
            dates.Add(date3);

            PhotoCountCollection counts = await f.PhotosGetCountsAsync(dates.ToArray(), true);

            Assert.That(counts, Is.Not.Null, "PhotoCounts should not be null.");
            Assert.That(counts, Has.Count.EqualTo(2), "PhotoCounts.Count should be two.");

            Console.WriteLine(f.LastResponse);
            Assert.Multiple(() =>
            {
                Assert.That(counts[0].FromDate, Is.EqualTo(date1), "FromDate should be 12th January.");
                Assert.Multiple(() =>
            {
                Assert.That(counts[0].ToDate, Is.EqualTo(date2), "ToDate should be 12th July.");
                Assert.That(counts[1].FromDate, Is.EqualTo(date2), "FromDate should be 12th July.");
                Assert.That(counts[1].ToDate, Is.EqualTo(date3), "ToDate should be 12th December.");
            });
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetCountUloadTest()
        {
            Flickr f = AuthInstance;

            var dates = new List<DateTime>();
            var date1 = new DateTime(2009, 7, 12);
            var date2 = new DateTime(2009, 9, 12);
            var date3 = new DateTime(2009, 12, 12);

            dates.Add(date2);
            dates.Add(date1);
            dates.Add(date3);

            PhotoCountCollection counts = await f.PhotosGetCountsAsync(dates.ToArray(), false);

            Assert.That(counts, Is.Not.Null, "PhotoCounts should not be null.");
            Assert.That(counts, Has.Count.EqualTo(2), "PhotoCounts.Count should be two.");
            Assert.Multiple(() =>
            {
                Assert.That(counts[0].FromDate, Is.EqualTo(date1), "FromDate should be 12th July.");
                Assert.Multiple(() =>
            {
                Assert.That(counts[0].ToDate, Is.EqualTo(date2), "ToDate should be 12th September.");
                Assert.That(counts[1].FromDate, Is.EqualTo(date2), "FromDate should be 12th September.");
                Assert.That(counts[1].ToDate, Is.EqualTo(date3), "ToDate should be 12th December.");
            });
            });
        }
    }
}