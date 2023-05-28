using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
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
        public async Task GetContextTest()
        {
            const string photosetId = "72157594532130119";

            var photos = await Instance.PhotosetsGetPhotosAsync(photosetId);

            var firstPhoto = photos.First();
            var lastPhoto = photos.Last();

            var context1 = await Instance.PhotosetsGetContextAsync(firstPhoto.PhotoId, photosetId);

            Assert.That(context1, Is.Not.Null, "Context should not be null.");
            Assert.Multiple(() =>
            {
                Assert.That(context1.PreviousPhoto, Is.Null, "PreviousPhoto should be null for first photo.");
                Assert.That(context1.NextPhoto, Is.Not.Null, "NextPhoto should not be null.");
            });
            if (firstPhoto.PhotoId != lastPhoto.PhotoId)
            {
                Assert.That(context1.NextPhoto.PhotoId, Is.EqualTo(photos[1].PhotoId), "NextPhoto should be the second photo in photoset.");
            }

            var context2 = await Instance.PhotosetsGetContextAsync(lastPhoto.PhotoId, photosetId);

            Assert.That(context2, Is.Not.Null, "Last photo context should not be null.");
            Assert.Multiple(() =>
            {
                Assert.That(context2.PreviousPhoto, Is.Not.Null, "PreviousPhoto should not be null for first photo.");
                Assert.That(context2.NextPhoto, Is.Null, "NextPhoto should be null.");
            });
            if (firstPhoto.PhotoId != lastPhoto.PhotoId)
            {
                Assert.That(context2.PreviousPhoto.PhotoId, Is.EqualTo(photos[photos.Count - 2].PhotoId), "PreviousPhoto should be the last but one photo in photoset.");
            }
        }

        [Test]
        public async Task PhotosetsGetInfoBasicTest()
        {
            const string photosetId = "72177720308619902";

            var p = await Instance.PhotosetsGetInfoAsync(photosetId);

            Assert.That(p, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(p.PhotosetId, Is.EqualTo(photosetId));
                Assert.Multiple(() =>
            {
                Assert.That(p.Title, Is.EqualTo("Vreden_28042022"));
                Assert.That(p.Description, Is.EqualTo(""));
            });
            });
        }

        [Test]
        public async Task PhotosetsGetListBasicTest()
        {
            PhotosetCollection photosets = await Instance.PhotosetsGetListAsync(TestData.TestUserId);
            Assert.Multiple(() =>
            {
                Assert.That(photosets.Count > 0, Is.True, "Should be at least one photoset");
                Assert.That(photosets.Count > 100, Is.True, "Should be greater than 100 photosets. (" + photosets.Count + " returned)");
            });
            foreach (Photoset set in photosets)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(set.OwnerId, Is.Not.Null, "OwnerId should not be null");
                    Assert.That(set.NumberOfPhotos > 0, Is.True, "NumberOfPhotos should be greater than zero");
                });
                Assert.Multiple(() =>
            {
                Assert.That(set.Title, Is.Not.Null, "Title should not be null");
                Assert.That(set.Description, Is.Not.Null, "Description should not be null");
            });
                Assert.That(set.OwnerId, Is.EqualTo(TestData.TestUserId));
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosetsGetListWithExtras()
        {
            var testUserPhotoSets = await AuthInstance.PhotosetsGetListAsync(TestData.TestUserId, 1, 5);

            testUserPhotoSets.Count.ShouldBeGreaterThan(0, "Should have returned at least 1 set for the authenticated user.");

            var firstPhotoSet = testUserPhotoSets.First();

            firstPhotoSet.PrimaryPhoto.ShouldNotBeNull("Primary Photo should not be null.");
            firstPhotoSet.PrimaryPhoto.LargeSquareThumbnailUrl.ShouldNotBeNullOrEmpty("LargeSquareThumbnailUrl should not be empty.");
        }

        [Test]
        public async Task PhotosetsGetListWebUrlTest()
        {
            PhotosetCollection photosets = await Instance.PhotosetsGetListAsync(TestData.TestUserId);

            Assert.That(photosets.Count > 0, Is.True, "Should be at least one photoset");

            foreach (Photoset set in photosets)
            {
                Assert.That(set.Url, Is.Not.Null);
                string expectedUrl = "https://www.flickr.com/photos/" + TestData.TestUserId + "/sets/" + set.PhotosetId + "/";
                Assert.That(set.Url, Is.EqualTo(expectedUrl));
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosetsCreateAddPhotosTest()
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
            var photoId1 = await AuthInstance.UploadPictureAsync(s, "Test1.jpg", initialPhotoTitle, initialPhotoDescription, initialTags, false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.Visible);

            s.Position = 0;
            // Upload photo a second time
            var photoId2 = await AuthInstance.UploadPictureAsync(s, "Test2.jpg", initialPhotoTitle, initialPhotoDescription, initialTags, false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.Visible);

            // Creat photoset
            Photoset photoset = await AuthInstance.PhotosetsCreateAsync("Test photoset", photoId1);

            try
            {
                var photos = await AuthInstance.PhotosetsGetPhotosAsync(photoset.PhotosetId, PhotoSearchExtras.OriginalFormat | PhotoSearchExtras.Media, PrivacyFilter.None, 1, 30, MediaType.None);

                photos.Count.ShouldBe(1, "Photoset should contain 1 photo");
                photos[0].IsPublic.ShouldBe(false, "Photo 1 should be private");

                // Add second photo to photoset.
                await AuthInstance.PhotosetsAddPhotoAsync(photoset.PhotosetId, photoId2);

                // Remove second photo from photoset
                await AuthInstance.PhotosetsRemovePhotoAsync(photoset.PhotosetId, photoId2);

                await AuthInstance.PhotosetsEditMetaAsync(photoset.PhotosetId, updatedPhotoTitle, updatedPhotoDescription);

                photoset = await AuthInstance.PhotosetsGetInfoAsync(photoset.PhotosetId);

                photoset.Title.ShouldBe(updatedPhotoTitle, "New Title should be set.");
                photoset.Description.ShouldBe(updatedPhotoDescription, "New description should be set");

                await AuthInstance.PhotosetsEditPhotosAsync(photoset.PhotosetId, photoId1, new[] { photoId2, photoId1 });

                await AuthInstance.PhotosetsRemovePhotoAsync(photoset.PhotosetId, photoId2);
            }
            finally
            {
                // Delete photoset completely
                await AuthInstance.PhotosetsDeleteAsync(photoset.PhotosetId);

                // Delete both photos.
                await AuthInstance.PhotoDeleteAsync(photoId1);
                await AuthInstance.PhotoDeleteAsync(photoId2);
            }
        }

        [Test]
        public async Task PhotosetsGetInfoEncodingCorrect()
        {
            Photoset pset = await Instance.PhotosetsGetInfoAsync("72177720308619902");

            Assert.That(pset.Title, Is.EqualTo("Vreden_28042022"));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosetGetInfoGetList()
        {
            const string photosetId = "72177720308619902";

            var photosetInfo = await AuthInstance.PhotosetsGetInfoAsync(photosetId);

            photosetInfo.NumberOfVideos.ShouldBe(0);
            photosetInfo.NumberOfPhotos.ShouldBe(3);

            var photosetList = await AuthInstance.PhotosetsGetListAsync();

            var photosetListInfo = photosetList.First(s => s.PhotosetId == photosetId);
            photosetListInfo.NumberOfVideos.ShouldBe(0);
            photosetListInfo.NumberOfPhotos.ShouldBe(3);
        }
    }
}