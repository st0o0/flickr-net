using FlickrNetCore;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PhotosetCommentsGetListTests
    /// </summary>
    [TestFixture]
    public class PhotosetCommentsGetListTests : BaseTest
    {
        [Test]
        public async Task PhotosetsCommentsGetListBasicTest()
        {
            Flickr f = Instance;

            PhotosetCommentCollection comments = await f.PhotosetsCommentsGetListAsync("72177720308619902");

            Assert.That(comments, Is.Not.Null);

            Assert.That(comments, Has.Count.EqualTo(0));
        }
    }
}