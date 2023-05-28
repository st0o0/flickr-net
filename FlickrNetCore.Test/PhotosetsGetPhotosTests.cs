using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for FlickrPhotoSetGetPhotos
    /// </summary>
    [TestFixture]
    public class PhotosetsGetPhotosTests : BaseTest
    {
        [Test]
        public async Task PhotosetsGetPhotosBasicTest(CancellationToken cancellationToken = default)
        {
            PhotosetPhotoCollection set = await Instance.PhotosetsGetPhotosAsync("72157618515066456", PhotoSearchExtras.All, PrivacyFilter.None, 1, 10, cancellationToken);

            Assert.AreEqual(8, set.Total, "NumberOfPhotos should be 8.");
            Assert.AreEqual(8, set.Count, "Should be 8 photos returned.");
        }

        [Test]
        public async Task PhotosetsGetPhotosMachineTagsTest(CancellationToken cancellationToken = default)
        {
            var set = await Instance.PhotosetsGetPhotosAsync("72157594218885767", PhotoSearchExtras.MachineTags, PrivacyFilter.None, 1, 10, cancellationToken);

            var machineTagsFound = set.Any(p => !string.IsNullOrEmpty(p.MachineTags));

            Assert.IsTrue(machineTagsFound, "No machine tags were found in the photoset");
        }

        [Test]
        public async Task PhotosetsGetPhotosFilterMediaTest(CancellationToken cancellationToken = default)
        {
            // https://www.flickr.com/photos/sgoralnick/sets/72157600283870192/
            // Set contains videos and photos
            var theset = await Instance.PhotosetsGetPhotosAsync("72157600283870192", PhotoSearchExtras.Media, PrivacyFilter.None, 1, 100, MediaType.Videos, cancellationToken);

            Assert.AreEqual("Canon 5D", theset.Title);

            foreach (var p in theset)
            {
                Assert.AreEqual("video", p.Media, "Should be video.");
            }

            var theset2 = await Instance.PhotosetsGetPhotosAsync("72157600283870192", PhotoSearchExtras.Media, PrivacyFilter.None, 1, 100, MediaType.Photos, cancellationToken);
            foreach (var p in theset2)
            {
                Assert.AreEqual("photo", p.Media, "Should be photo.");
            }
        }

        [Test]
        public async Task PhotosetsGetPhotosWebUrlTest(CancellationToken cancellationToken = default)
        {
            var theset = await Instance.PhotosetsGetPhotosAsync("72157618515066456", cancellationToken);

            foreach (var p in theset)
            {
                Assert.IsNotNull(p.UserId, "UserId should not be null.");
                Assert.AreNotEqual(string.Empty, p.UserId, "UserId should not be an empty string.");
                var url = "https://www.flickr.com/photos/" + p.UserId + "/" + p.PhotoId + "/";
                Assert.AreEqual(url, p.WebUrl);
            }
        }

        [Test]
        public async Task PhotosetsGetPhotosPrimaryPhotoTest(CancellationToken cancellationToken = default)
        {
            var theset = await Instance.PhotosetsGetPhotosAsync("72157618515066456", 1, 100, cancellationToken);

            Assert.IsNotNull(theset.PrimaryPhotoId, "PrimaryPhotoId should not be null.");

            if (theset.Total >= theset.PerPage) return;

            var primary = theset.FirstOrDefault(p => p.PhotoId == theset.PrimaryPhotoId);

            Assert.IsNotNull(primary, "Primary photo should have been found.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosetsGetPhotosOrignalTest(CancellationToken cancellationToken = default)
        {
            var photos = await AuthInstance.PhotosetsGetPhotosAsync("72157623027759445", PhotoSearchExtras.AllUrls, cancellationToken);

            foreach (var photo in photos)
            {
                Assert.IsNotNull(photo.OriginalUrl, "Original URL should not be null.");
            }
        }

        [Test]
        public async Task ShouldReturnDateTakenWhenAsked(CancellationToken cancellationToken = default)
        {
            var theset = await Instance.PhotosetsGetPhotosAsync("72157618515066456", PhotoSearchExtras.DateTaken | PhotoSearchExtras.DateUploaded, 1, 10, cancellationToken);

            var firstInvalid = theset.FirstOrDefault(p => p.DateTaken == DateTime.MinValue || p.DateUploaded == DateTime.MinValue);

            Assert.IsNull(firstInvalid, "There should not be a photo with not date taken or date uploaded");

            theset = await Instance.PhotosetsGetPhotosAsync("72157618515066456", PhotoSearchExtras.All, 1, 10, cancellationToken);

            firstInvalid = theset.FirstOrDefault(p => p.DateTaken == DateTime.MinValue || p.DateUploaded == DateTime.MinValue);

            Assert.IsNull(firstInvalid, "There should not be a photo with not date taken or date uploaded");

            theset = await Instance.PhotosetsGetPhotosAsync("72157618515066456", PhotoSearchExtras.None, 1, 10, cancellationToken);

            var noDateCount = theset.Count(p => p.DateTaken == DateTime.MinValue || p.DateUploaded == DateTime.MinValue);

            Assert.AreEqual(theset.Count, noDateCount, "All photos should have no date taken set.");
        }
    }
}