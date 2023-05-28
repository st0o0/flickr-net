using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PhotosUploadTests
    /// </summary>
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class PhotosUploadTests : BaseTest
    {
        [Test]
        public async Task UploadPictureBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            f.OnUploadProgress += (sender, args) =>
            {
                // Do nothing
            };

            byte[] imageBytes = TestData.TestImageBytes;
            var s = new MemoryStream(imageBytes);
            s.Position = 0;

            string title = "Test Title";
            string desc = "Test Description\nSecond Line";
            string tags = "testtag1,testtag2";
            string photoId = await f.UploadPictureAsync(s, "Test.jpg", title, desc, tags, false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.Visible, cancellationToken: cancellationToken);

            try
            {
                PhotoInfo info = await f.PhotosGetInfoAsync(photoId, cancellationToken);

                Assert.AreEqual(title, info.Title);
                Assert.AreEqual(desc, info.Description);
                Assert.AreEqual(2, info.Tags.Count);
                Assert.AreEqual("testtag1", info.Tags[0].Raw);
                Assert.AreEqual("testtag2", info.Tags[1].Raw);

                Assert.IsFalse(info.IsPublic);
                Assert.IsFalse(info.IsFamily);
                Assert.IsFalse(info.IsFriend);

                SizeCollection sizes = await f.PhotosGetSizesAsync(photoId, cancellationToken);

                string url = sizes[^1].Source;
                using HttpClient client = new();
                byte[] downloadBytes = await client.GetByteArrayAsync(url, cancellationToken);
                string downloadBase64 = Convert.ToBase64String(downloadBytes);

                Assert.AreEqual(TestData.TestImageBase64, downloadBase64);
            }
            finally
            {
                await f.PhotoDeleteAsync(photoId, cancellationToken);
            }
        }

        [Test]
        public async Task DownloadAndUploadImage(CancellationToken cancellationToken = default)
        {
            var photos = await AuthInstance.PeopleGetPhotosAsync(PhotoSearchExtras.Small320Url, cancellationToken);

            var photo = photos.First();
            var url = photo.Small320Url;

            HttpClient client = new();
            var data = await client.GetByteArrayAsync(url, cancellationToken);

            var ms = new MemoryStream(data) { Position = 0 };

            var photoId = await AuthInstance.UploadPictureAsync(ms, "test.jpg", "Test Photo", "Test Description", "", false, false, false, ContentType.Photo, SafetyLevel.Safe, HiddenFromSearch.Hidden, cancellationToken: cancellationToken);
            Assert.IsNotNull(photoId, "PhotoId should not be null");

            // Cleanup
            await AuthInstance.PhotoDeleteAsync(photoId, cancellationToken);
        }

        [Test]
        public async Task ReplacePictureBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            byte[] imageBytes = TestData.TestImageBytes;
            var s = new MemoryStream(imageBytes);
            s.Position = 0;

            string title = "Test Title";
            string desc = "Test Description\nSecond Line";
            string tags = "testtag1,testtag2";
            string photoId = await f.UploadPictureAsync(s, "Test.jpg", title, desc, tags, false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.Visible, cancellationToken: cancellationToken);

            try
            {
                s.Position = 0;
                await f.ReplacePictureAsync(s, "Test.jpg", photoId, cancellationToken: cancellationToken);
            }
            finally
            {
                await f.PhotoDeleteAsync(photoId, cancellationToken);
            }
        }

        [Test]
        public async Task UploadPictureFromUrl(CancellationToken cancellationToken = default)
        {
            string url = "http://www.google.co.uk/intl/en_com/images/srpr/logo1w.png";
            Flickr f = AuthInstance;

            using (HttpClient client = new())
            {
                using (Stream s = await client.GetStreamAsync(url, cancellationToken))
                {
                    string photoId = await f.UploadPictureAsync(s, "google.png", "Google Image", "Google", "", false, false, false, ContentType.Photo, SafetyLevel.None, HiddenFromSearch.None, cancellationToken: cancellationToken);
                    await f.PhotoDeleteAsync(photoId, cancellationToken);
                }
            }
        }

        [Test, Ignore("Long running test")]
        public async Task UploadLargeVideoFromUrl(CancellationToken cancellationToken = default)
        {
            string url = "http://www.sample-videos.com/video/mp4/720/big_buck_bunny_720p_50mb.mp4";

            Flickr f = AuthInstance;

            using HttpClient client = new();
            using Stream s = await client.GetStreamAsync(url, cancellationToken);
            string photoId = await f.UploadPictureAsync(s, "bunny.mp4", "Big Buck Bunny", "Sample Video", "", false, false, false, ContentType.Photo, SafetyLevel.None, HiddenFromSearch.None, cancellationToken: cancellationToken);
            await f.PhotoDeleteAsync(photoId, cancellationToken);
        }

        //

        [Test]
        [Ignore("Large time consuming uploads")]
        public async Task UploadPictureVideoTests(CancellationToken cancellationToken = default)
        {
            // Samples downloaded from http://support.apple.com/kb/HT1425
            // sample_mpeg2.m2v does not upload
            string[] filenames = { "sample_mpeg4.mp4", "sample_sorenson.mov", "sample_iTunes.mov", "sample_iPod.m4v", "sample.3gp", "sample_3GPP2.3g2" };
            // Copy files to this directory.
            string directory = @"Z:\Code Projects\FlickrNet\Samples\";

            foreach (string file in filenames)
            {
                try
                {
                    using Stream s = new FileStream(Path.Combine(directory, file), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    Flickr f = AuthInstance;
                    string photoId = await f.UploadPictureAsync(s, file, "Video Upload Test", file, "video, test", false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.None, cancellationToken: cancellationToken);
                    await f.PhotoDeleteAsync(photoId, cancellationToken);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Failed during upload of " + file + " with exception: " + ex.ToString());
                }
            }
        }
    }
}