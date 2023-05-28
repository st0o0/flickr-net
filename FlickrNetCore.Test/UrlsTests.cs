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
            FoundUser user = await AuthInstance.UrlsLookupUserAsync("https://www.flickr.com/photos/samjudson", cancellationToken);

            Assert.AreEqual("41888973@N00", user.UserId);
            Assert.AreEqual("Sam Judson", user.UserName);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task UrlsLookupGroup(CancellationToken cancellationToken = default)
        {
            string groupUrl = "https://www.flickr.com/groups/angels_of_the_north/";

            string groupId = await AuthInstance.UrlsLookupGroupAsync(groupUrl, cancellationToken);

            Assert.AreEqual("71585219@N00", groupId);
        }

        [Test]
        public async Task UrlsLookupGalleryTest(CancellationToken cancellationToken = default)
        {
            string galleryUrl = "https://www.flickr.com/photos/samjudson/galleries/72157622589312064";

            Flickr f = Instance;

            Gallery gallery = await f.UrlsLookupGalleryAsync(galleryUrl, cancellationToken);

            Assert.AreEqual(galleryUrl, gallery.GalleryUrl);
        }

        [Test]
        public async Task UrlsGetUserPhotosTest(CancellationToken cancellationToken = default)
        {
            string url = await Instance.UrlsGetUserPhotosAsync(TestData.TestUserId, cancellationToken);

            Assert.AreEqual("https://www.flickr.com/photos/samjudson/", url);
        }

        [Test]
        public async Task UrlsGetUserProfileTest(CancellationToken cancellationToken = default)
        {
            string url = await Instance.UrlsGetUserProfileAsync(TestData.TestUserId, cancellationToken);

            Assert.AreEqual("https://www.flickr.com/people/samjudson/", url);
        }

        [Test]
        public async Task UrlsGetGroupTest(CancellationToken cancellationToken = default)
        {
            string url = await Instance.UrlsGetGroupAsync(TestData.GroupId, cancellationToken);

            Assert.AreEqual("https://www.flickr.com/groups/lakedistrict/", url);
        }
    }
}