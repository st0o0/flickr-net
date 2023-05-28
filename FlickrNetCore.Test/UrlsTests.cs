using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for UrlsTests
    /// </summary>
    [TestFixture]
    public class UrlsTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task UrlsLookupUserTest1(CancellationToken cancellationToken = default)
        {
            FoundUser user = await AuthInstance.UrlsLookupUserAsync("https://www.flickr.com/photos/samjudson");
            Assert.Multiple(() =>
            {
                Assert.That(user.UserId, Is.EqualTo("41888973@N00"));
                Assert.That(user.UserName, Is.EqualTo("Sam Judson"));
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task UrlsLookupGroup(CancellationToken cancellationToken = default)
        {
            string groupUrl = "https://www.flickr.com/groups/angels_of_the_north/";

            string groupId = await AuthInstance.UrlsLookupGroupAsync(groupUrl);

            Assert.That(groupId, Is.EqualTo("71585219@N00"));
        }

        [Test]
        public async Task UrlsLookupGalleryTest(CancellationToken cancellationToken = default)
        {
            string galleryUrl = "https://www.flickr.com/photos/samjudson/galleries/72157622589312064";

            Flickr f = Instance;

            Gallery gallery = await f.UrlsLookupGalleryAsync(galleryUrl);

            Assert.That(gallery.GalleryUrl, Is.EqualTo(galleryUrl));
        }

        [Test]
        public async Task UrlsGetUserPhotosTest(CancellationToken cancellationToken = default)
        {
            string url = await Instance.UrlsGetUserPhotosAsync(TestData.TestUserId);

            Assert.That(url, Is.EqualTo("https://www.flickr.com/photos/samjudson/"));
        }

        [Test]
        public async Task UrlsGetUserProfileTest(CancellationToken cancellationToken = default)
        {
            string url = await Instance.UrlsGetUserProfileAsync(TestData.TestUserId);

            Assert.That(url, Is.EqualTo("https://www.flickr.com/people/samjudson/"));
        }

        [Test]
        public async Task UrlsGetGroupTest(CancellationToken cancellationToken = default)
        {
            string url = await Instance.UrlsGetGroupAsync(TestData.GroupId);

            Assert.That(url, Is.EqualTo("https://www.flickr.com/groups/lakedistrict/"));
        }
    }
}