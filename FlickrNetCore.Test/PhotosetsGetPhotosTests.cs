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
        public async Task PhotosetsGetPhotosBasicTest()
        {
            PhotosetPhotoCollection set = await Instance.PhotosetsGetPhotosAsync("72157618515066456", PhotoSearchExtras.All, PrivacyFilter.None, 1, 10);
            Assert.Multiple(() =>
            {
                Assert.That(set.Total, Is.EqualTo(8), "NumberOfPhotos should be 8.");
                Assert.That(set, Has.Count.EqualTo(8), "Should be 8 photos returned.");
            });
        }

        [Test]
        public async Task PhotosetsGetPhotosMachineTagsTest()
        {
            var set = await Instance.PhotosetsGetPhotosAsync("72157594218885767", PhotoSearchExtras.MachineTags, PrivacyFilter.None, 1, 10);

            var machineTagsFound = set.Any(p => !string.IsNullOrEmpty(p.MachineTags));

            Assert.That(machineTagsFound, Is.True, "No machine tags were found in the photoset");
        }

        [Test]
        public async Task PhotosetsGetPhotosFilterMediaTest()
        {
            // https://www.flickr.com/photos/sgoralnick/sets/72157600283870192/
            // Set contains videos and photos
            var theset = await Instance.PhotosetsGetPhotosAsync("72157600283870192", PhotoSearchExtras.Media, PrivacyFilter.None, 1, 100, MediaType.Videos);

            Assert.That(theset.Title, Is.EqualTo("Canon 5D"));

            foreach (var p in theset)
            {
                Assert.That(p.Media, Is.EqualTo("video"), "Should be video.");
            }

            var theset2 = await Instance.PhotosetsGetPhotosAsync("72157600283870192", PhotoSearchExtras.Media, PrivacyFilter.None, 1, 100, MediaType.Photos);
            foreach (var p in theset2)
            {
                Assert.That(p.Media, Is.EqualTo("photo"), "Should be photo.");
            }
        }

        [Test]
        public async Task PhotosetsGetPhotosWebUrlTest()
        {
            var theset = await Instance.PhotosetsGetPhotosAsync("72157618515066456");

            foreach (var p in theset)
            {
                Assert.That(p.UserId, Is.Not.Null, "UserId should not be null.");
                Assert.That(p.UserId, Is.Not.EqualTo(string.Empty), "UserId should not be an empty string.");
                var url = "https://www.flickr.com/photos/" + p.UserId + "/" + p.PhotoId + "/";
                Assert.That(p.WebUrl, Is.EqualTo(url));
            }
        }

        [Test]
        public async Task PhotosetsGetPhotosPrimaryPhotoTest()
        {
            var theset = await Instance.PhotosetsGetPhotosAsync("72157618515066456", 1, 100);

            Assert.That(theset.PrimaryPhotoId, Is.Not.Null, "PrimaryPhotoId should not be null.");

            if (theset.Total >= theset.PerPage) return;

            var primary = theset.FirstOrDefault(p => p.PhotoId == theset.PrimaryPhotoId);

            Assert.That(primary, Is.Not.Null, "Primary photo should have been found.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosetsGetPhotosOrignalTest()
        {
            var photos = await AuthInstance.PhotosetsGetPhotosAsync("72157623027759445", PhotoSearchExtras.AllUrls);

            foreach (var photo in photos)
            {
                Assert.That(photo.OriginalUrl, Is.Not.Null, "Original URL should not be null.");
            }
        }

        [Test]
        public async Task ShouldReturnDateTakenWhenAsked()
        {
            var theset = await Instance.PhotosetsGetPhotosAsync("72157618515066456", PhotoSearchExtras.DateTaken | PhotoSearchExtras.DateUploaded, 1, 10);

            var firstInvalid = theset.FirstOrDefault(p => p.DateTaken == DateTime.MinValue || p.DateUploaded == DateTime.MinValue);

            Assert.That(firstInvalid, Is.Null, "There should not be a photo with not date taken or date uploaded");

            theset = await Instance.PhotosetsGetPhotosAsync("72157618515066456", PhotoSearchExtras.All, 1, 10);

            firstInvalid = theset.FirstOrDefault(p => p.DateTaken == DateTime.MinValue || p.DateUploaded == DateTime.MinValue);

            Assert.That(firstInvalid, Is.Null, "There should not be a photo with not date taken or date uploaded");

            theset = await Instance.PhotosetsGetPhotosAsync("72157618515066456", PhotoSearchExtras.None, 1, 10);

            var noDateCount = theset.Count(p => p.DateTaken == DateTime.MinValue || p.DateUploaded == DateTime.MinValue);

            Assert.That(noDateCount, Is.EqualTo(theset.Count), "All photos should have no date taken set.");
        }
    }
}