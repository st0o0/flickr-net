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
        public async Task UploadPictureBasicTest()
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
            string photoId = await f.UploadPictureAsync(s, "Test.jpg", title, desc, tags, false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.Visible);

            try
            {
                PhotoInfo info = await f.PhotosGetInfoAsync(photoId);
                Assert.Multiple(async () =>
                {
                    Assert.That(info.Title, Is.EqualTo(title));
                    Assert.Multiple(() =>
                {
                    Assert.That(info.Description, Is.EqualTo(desc));
                    Assert.That(info.Tags, Has.Count.EqualTo(2));
                });
                    Assert.Multiple(() =>
                    {
                        Assert.That(info.Tags[0].Raw, Is.EqualTo("testtag1"));
                        Assert.That(info.Tags[1].Raw, Is.EqualTo("testtag2"));
                    });
                    Assert.That(info.IsPublic, Is.False);
                    Assert.That(info.IsFamily, Is.False);
                    Assert.That(info.IsFriend, Is.False);

                    SizeCollection sizes = await f.PhotosGetSizesAsync(photoId);

                    string url = sizes[^1].Source;
                    using HttpClient client = new();
                    byte[] downloadBytes = await client.GetByteArrayAsync(url);
                    string downloadBase64 = Convert.ToBase64String(downloadBytes);

                    Assert.That(downloadBase64, Is.EqualTo(TestData.TestImageBase64));
                });
            }
            finally
            {
                await f.PhotoDeleteAsync(photoId);
            }
        }

        [Test]
        public async Task DownloadAndUploadImage()
        {
            var photos = await AuthInstance.PeopleGetPhotosAsync(PhotoSearchExtras.Small320Url);

            var photo = photos.First();
            var url = photo.Small320Url;

            HttpClient client = new();
            var data = await client.GetByteArrayAsync(url);

            var ms = new MemoryStream(data) { Position = 0 };

            var photoId = await AuthInstance.UploadPictureAsync(ms, "test.jpg", "Test Photo", "Test Description", "", false, false, false, ContentType.Photo, SafetyLevel.Safe, HiddenFromSearch.Hidden);
            Assert.That(photoId, Is.Not.Null, "PhotoId should not be null");

            // Cleanup
            await AuthInstance.PhotoDeleteAsync(photoId);
        }

        [Test]
        public async Task ReplacePictureBasicTest()
        {
            Flickr f = AuthInstance;

            byte[] imageBytes = TestData.TestImageBytes;
            using var s = new MemoryStream(imageBytes);
            s.Position = 0;

            string title = "Test Title";
            string desc = "Test Description\nSecond Line";
            string tags = "testtag1,testtag2";
            string photoId = await f.UploadPictureAsync(s, "Test.jpg", title, desc, tags, false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.Visible);

            try
            {
                s.Position = 0;
                var t = await f.ReplacePictureAsync(s, "Test.jpg", photoId: photoId);
                Assert.That(t, Is.Not.Null);
            }
            finally
            {
                await f.PhotoDeleteAsync(photoId);
            }
        }

        [Test]
        public async Task UploadPictureFromUrl()
        {
            string url = "http://www.google.co.uk/intl/en_com/images/srpr/logo1w.png";
            Flickr f = AuthInstance;

            using (HttpClient client = new())
            {
                using (Stream s = await client.GetStreamAsync(url))
                {
                    string photoId = await f.UploadPictureAsync(s, "google.png", "Google Image", "Google", "", false, false, false, ContentType.Photo, SafetyLevel.None, HiddenFromSearch.None);
                    await f.PhotoDeleteAsync(photoId);
                }
            }
        }

        [Test, Ignore("Long running test")]
        public async Task UploadLargeVideoFromUrl()
        {
            string url = "http://www.sample-videos.com/video/mp4/720/big_buck_bunny_720p_50mb.mp4";

            Flickr f = AuthInstance;

            using HttpClient client = new();
            using Stream s = await client.GetStreamAsync(url);
            string photoId = await f.UploadPictureAsync(s, "bunny.mp4", "Big Buck Bunny", "Sample Video", "", false, false, false, ContentType.Photo, SafetyLevel.None, HiddenFromSearch.None);
            await f.PhotoDeleteAsync(photoId);
        }

        //

        [Test]
        [Ignore("Large time consuming uploads")]
        public async Task UploadPictureVideoTests()
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
                    string photoId = await f.UploadPictureAsync(s, file, "Video Upload Test", file, "video, test", false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.None);
                    await f.PhotoDeleteAsync(photoId);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Failed during upload of " + file + " with exception: " + ex.ToString());
                }
            }
        }
    }
}