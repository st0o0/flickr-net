using FlickrNet;
using FlickrNet.CollectionModels;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PhotosetCommentsGetListTests
    /// </summary>
    [TestFixture]
    public class PhotosetCommentsGetListTests : BaseTest
    {
        [Test]
        public async Task PhotosetsCommentsGetListBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            PhotosetCommentCollection comments = await f.PhotosetsCommentsGetListAsync("1335934", cancellationToken);

            Assert.IsNotNull(comments);

            Assert.AreEqual(2, comments.Count);

            Assert.AreEqual("Superchou", comments[0].AuthorUserName);
            Assert.AreEqual("LOL... I had no idea this set existed... what a great afternoon we had :)", comments[0].CommentHtml);
        }
    }
}