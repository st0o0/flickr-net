using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlickrNet;
using FlickrNet.CollectionModels;
using FlickrNet.Enums;
using FlickrNet.Models;
using NUnit.Framework;
using Shouldly;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for FlickrPhotosetsGetList
    /// </summary>
    [TestFixture]
    public class PhotosetsTests : BaseTest
    {
        [Test]
        public async Task GetContextTest(CancellationToken cancellationToken = default)
        {
            const string photosetId = "72157594532130119";

            var photos = await Instance.PhotosetsGetPhotosAsync(photosetId, cancellationToken);

            var firstPhoto = photos.First();
            var lastPhoto = photos.Last();

            var context1 = await Instance.PhotosetsGetContextAsync(firstPhoto.PhotoId, photosetId, cancellationToken);

            Assert.IsNotNull(context1, "Context should not be null.");
            Assert.IsNull(context1.PreviousPhoto, "PreviousPhoto should be null for first photo.");
            Assert.IsNotNull(context1.NextPhoto, "NextPhoto should not be null.");

            if (firstPhoto.PhotoId != lastPhoto.PhotoId)
            {
                Assert.AreEqual(photos[1].PhotoId, context1.NextPhoto.PhotoId, "NextPhoto should be the second photo in photoset.");
            }

            var context2 = await Instance.PhotosetsGetContextAsync(lastPhoto.PhotoId, photosetId, cancellationToken);

            Assert.IsNotNull(context2, "Last photo context should not be null.");
            Assert.IsNotNull(context2.PreviousPhoto, "PreviousPhoto should not be null for first photo.");
            Assert.IsNull(context2.NextPhoto, "NextPhoto should be null.");

            if (firstPhoto.PhotoId != lastPhoto.PhotoId)
            {
                Assert.AreEqual(photos[photos.Count - 2].PhotoId, context2.PreviousPhoto.PhotoId, "PreviousPhoto should be the last but one photo in photoset.");
            }
        }

        [Test]
        public async Task PhotosetsGetInfoBasicTest(CancellationToken cancellationToken = default)
        {
            const string photosetId = "72157594532130119";

            var p = await Instance.PhotosetsGetInfoAsync(photosetId, cancellationToken);

            Assert.IsNotNull(p);
            Assert.AreEqual(photosetId, p.PhotosetId);
            Assert.AreEqual("Places: Derwent Walk, Gateshead", p.Title);
            Assert.AreEqual("It's near work, so I go quite a bit...", p.Description);
        }

        [Test]
        public async Task PhotosetsGetListBasicTest(CancellationToken cancellationToken = default)
        {
            PhotosetCollection photosets = await Instance.PhotosetsGetListAsync(TestData.TestUserId, cancellationToken);

            Assert.IsTrue(photosets.Count > 0, "Should be at least one photoset");
            Assert.IsTrue(photosets.Count > 100, "Should be greater than 100 photosets. (" + photosets.Count + " returned)");

            foreach (Photoset set in photosets)
            {
                Assert.IsNotNull(set.OwnerId, "OwnerId should not be null");
                Assert.IsTrue(set.NumberOfPhotos > 0, "NumberOfPhotos should be greater than zero");
                Assert.IsNotNull(set.Title, "Title should not be null");
                Assert.IsNotNull(set.Description, "Description should not be null");
                Assert.AreEqual(TestData.TestUserId, set.OwnerId);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosetsGetListWithExtras(CancellationToken cancellationToken = default)
        {
            var testUserPhotoSets = await AuthInstance.PhotosetsGetListAsync(TestData.TestUserId, 1, 5, cancellationToken);

            testUserPhotoSets.Count.ShouldBeGreaterThan(0, "Should have returned at least 1 set for the authenticated user.");

            var firstPhotoSet = testUserPhotoSets.First();

            firstPhotoSet.PrimaryPhoto.ShouldNotBeNull("Primary Photo should not be null.");
            firstPhotoSet.PrimaryPhoto.LargeSquareThumbnailUrl.ShouldNotBeNullOrEmpty("LargeSquareThumbnailUrl should not be empty.");
        }

        [Test]
        public async Task PhotosetsGetListWebUrlTest(CancellationToken cancellationToken = default)
        {
            PhotosetCollection photosets = await Instance.PhotosetsGetListAsync(TestData.TestUserId, cancellationToken);

            Assert.IsTrue(photosets.Count > 0, "Should be at least one photoset");

            foreach (Photoset set in photosets)
            {
                Assert.IsNotNull(set.Url);
                string expectedUrl = "https://www.flickr.com/photos/" + TestData.TestUserId + "/sets/" + set.PhotosetId + "/";
                Assert.AreEqual(expectedUrl, set.Url);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosetsCreateAddPhotosTest(CancellationToken cancellationToken = default)
        {
            byte[] imageBytes = TestData.TestImageBytes;
            var s = new MemoryStream(imageBytes);

            const string initialPhotoTitle = "Test Title";
            const string updatedPhotoTitle = "New Test Title";
            const string initialPhotoDescription = "Test Description\nSecond Line";
            const string updatedPhotoDescription = "New Test Description";
            const string initialTags = "testtag1,testtag2";

            s.Position = 0;
            // Upload photo once
            var photoId1 = await AuthInstance.UploadPictureAsync(s, "Test1.jpg", initialPhotoTitle, initialPhotoDescription, initialTags, false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.Visible, cancellationToken: cancellationToken);

            s.Position = 0;
            // Upload photo a second time
            var photoId2 = await AuthInstance.UploadPictureAsync(s, "Test2.jpg", initialPhotoTitle, initialPhotoDescription, initialTags, false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.Visible, cancellationToken: cancellationToken);

            // Creat photoset
            Photoset photoset = await AuthInstance.PhotosetsCreateAsync("Test photoset", photoId1, cancellationToken);

            try
            {
                var photos = await AuthInstance.PhotosetsGetPhotosAsync(photoset.PhotosetId, PhotoSearchExtras.OriginalFormat | PhotoSearchExtras.Media, PrivacyFilter.None, 1, 30, MediaType.None, cancellationToken);

                photos.Count.ShouldBe(1, "Photoset should contain 1 photo");
                photos[0].IsPublic.ShouldBe(false, "Photo 1 should be private");

                // Add second photo to photoset.
                await AuthInstance.PhotosetsAddPhotoAsync(photoset.PhotosetId, photoId2, cancellationToken);

                // Remove second photo from photoset
                await AuthInstance.PhotosetsRemovePhotoAsync(photoset.PhotosetId, photoId2, cancellationToken);

                await AuthInstance.PhotosetsEditMetaAsync(photoset.PhotosetId, updatedPhotoTitle, updatedPhotoDescription, cancellationToken);

                photoset = await AuthInstance.PhotosetsGetInfoAsync(photoset.PhotosetId, cancellationToken);

                photoset.Title.ShouldBe(updatedPhotoTitle, "New Title should be set.");
                photoset.Description.ShouldBe(updatedPhotoDescription, "New description should be set");

                await AuthInstance.PhotosetsEditPhotosAsync(photoset.PhotosetId, photoId1, new[] { photoId2, photoId1 }, cancellationToken);

                await AuthInstance.PhotosetsRemovePhotoAsync(photoset.PhotosetId, photoId2, cancellationToken);
            }
            finally
            {
                // Delete photoset completely
                await AuthInstance.PhotosetsDeleteAsync(photoset.PhotosetId, cancellationToken);

                // Delete both photos.
                await AuthInstance.PhotoDeleteAsync(photoId1, cancellationToken);
                await AuthInstance.PhotoDeleteAsync(photoId2, cancellationToken);
            }
        }

        [Test]
        public async Task PhotosetsGetInfoEncodingCorrect(CancellationToken cancellationToken = default)
        {
            Photoset pset = await Instance.PhotosetsGetInfoAsync("72157627650627399", cancellationToken);

            Assert.AreEqual("Sítio em Arujá - 14/08/2011", pset.Title);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosetGetInfoGetList(CancellationToken cancellationToken = default)
        {
            const string photosetId = "72157660633195178";

            var photosetInfo = await AuthInstance.PhotosetsGetInfoAsync(photosetId, cancellationToken);

            photosetInfo.NumberOfVideos.ShouldBe(2);
            photosetInfo.NumberOfPhotos.ShouldBe(72);

            var photosetList = await AuthInstance.PhotosetsGetListAsync(cancellationToken);

            var photosetListInfo = photosetList.First(s => s.PhotosetId == photosetId);
            photosetListInfo.NumberOfVideos.ShouldBe(2);
            photosetListInfo.NumberOfPhotos.ShouldBe(72);
        }
    }
}