﻿using FlickrNetCore;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    public class PhotosGetContactsPublicPhotosTests : BaseTest
    {
        [Test]
        public async Task PhotosGetContactsPublicPhotosUserIdExtrasTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            string userId = TestData.TestUserId;
            PhotoSearchExtras extras = PhotoSearchExtras.All;
            var photos = await f.PhotosGetContactsPublicPhotosAsync(userId, extras);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "Should have returned more than 0 photos");
        }

        [Test]
        public async Task PhotosGetContactsPublicPhotosAllParamsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            string userId = TestData.TestUserId;

            int count = 4; // TODO: Initialize to an appropriate value
            bool justFriends = true; // TODO: Initialize to an appropriate value
            bool singlePhoto = true; // TODO: Initialize to an appropriate value
            bool includeSelf = false; // TODO: Initialize to an appropriate value
            PhotoSearchExtras extras = PhotoSearchExtras.None;

            var photos = await f.PhotosGetContactsPublicPhotosAsync(userId, count, justFriends, singlePhoto, includeSelf, extras);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "Should have returned more than 0 photos");
        }

        [Test]
        public async Task PhotosGetContactsPublicPhotosExceptExtrasTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            string userId = TestData.TestUserId;

            int count = 4;
            bool justFriends = true;
            bool singlePhoto = true;
            bool includeSelf = false;

            var photos = await f.PhotosGetContactsPublicPhotosAsync(userId, count, justFriends, singlePhoto, includeSelf);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "Should have returned more than 0 photos");
        }

        [Test]
        public async Task PhotosGetContactsPublicPhotosUserIdTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            string userId = TestData.TestUserId;

            var photos = await f.PhotosGetContactsPublicPhotosAsync(userId);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "Should have returned more than 0 photos");
        }

        [Test]
        public async Task PhotosGetContactsPublicPhotosUserIdCountExtrasTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            string userId = TestData.TestUserId;

            int count = 5;
            PhotoSearchExtras extras = PhotoSearchExtras.None;

            var photos = await f.PhotosGetContactsPublicPhotosAsync(userId, count, extras);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "Should have returned more than 0 photos");
        }

        [Test]
        public async Task PhotosGetContactsPublicPhotosUserIdCountTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            string userId = TestData.TestUserId;

            int count = 5;

            var photos = await f.PhotosGetContactsPublicPhotosAsync(userId, count);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "Should have returned more than 0 photos");
        }
    }
}