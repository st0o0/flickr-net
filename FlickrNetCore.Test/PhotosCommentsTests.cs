using FlickrNetCore;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PhotosCommentsGetListTests
    /// </summary>
    [TestFixture]
    public class PhotosCommentsTests : BaseTest
    {
        [Test]
        public async Task PhotosCommentsGetListBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            PhotoCommentCollection comments = await f.PhotosCommentsGetListAsync("3546335765");

            Assert.That(comments, Is.Not.Null, "PhotoCommentCollection should not be null.");

            Assert.That(comments, Has.Count.EqualTo(1), "Count should be one.");
            Assert.Multiple(() =>
            {
                Assert.That(comments[0].AuthorUserName, Is.EqualTo("ian1001"));
                Assert.That(comments[0].CommentHtml, Is.EqualTo("Sam lucky you NYCis so cool can't wait to go again it's my fav city along with San fran"));
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosCommentsGetRecentForContactsBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosCommentsGetRecentForContactsAsync(cancellationToken);
            Assert.That(photos, Is.Not.Null, "PhotoCollection should not be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosCommentsGetRecentForContactsFullParamTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosCommentsGetRecentForContactsAsync(DateTime.Now.AddHours(-1), PhotoSearchExtras.All, 1, 20);
            Assert.That(photos, Is.Not.Null, "PhotoCollection should not be null.");
            Assert.That(photos.PerPage, Is.EqualTo(20));
        }
    }
}