using System;

using NUnit.Framework;
using FlickrNet;
using System.Threading.Tasks;
using System.Threading;
using FlickrNet.CollectionModels;
using FlickrNet.Enums;

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

            PhotoCommentCollection comments = await f.PhotosCommentsGetListAsync("3546335765", cancellationToken);

            Assert.IsNotNull(comments, "PhotoCommentCollection should not be null.");

            Assert.AreEqual(1, comments.Count, "Count should be one.");

            Assert.AreEqual("ian1001", comments[0].AuthorUserName);
            Assert.AreEqual("Sam lucky you NYCis so cool can't wait to go again it's my fav city along with San fran", comments[0].CommentHtml);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosCommentsGetRecentForContactsBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosCommentsGetRecentForContactsAsync(cancellationToken);
            Assert.IsNotNull(photos, "PhotoCollection should not be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosCommentsGetRecentForContactsFullParamTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosCommentsGetRecentForContactsAsync(DateTime.Now.AddHours(-1), PhotoSearchExtras.All, 1, 20, cancellationToken);
            Assert.IsNotNull(photos, "PhotoCollection should not be null.");
            Assert.AreEqual(20, photos.PerPage);
        }
    }
}