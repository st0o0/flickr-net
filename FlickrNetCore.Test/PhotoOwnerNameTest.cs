using FlickrNetCore;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.SearchOptions;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PhotoOwnerNameTest
    /// </summary>
    [TestFixture]
    public class PhotoOwnerNameTest : BaseTest
    {
        [Test]
        public async Task PhotosSearchOwnerNameTest()
        {
            PhotoSearchOptions o = new()
            {
                UserId = TestData.TestUserId,
                PerPage = 10,
                Extras = PhotoSearchExtras.OwnerName
            };

            Flickr f = Instance;
            PhotoCollection photos = await f.PhotosSearchAsync(o);

            Assert.That(photos[0].OwnerName, Is.Not.Null);
        }

        [Test]
        public async Task PhotosGetContactsPublicPhotosOwnerNameTest()
        {
            Flickr f = Instance;
            PhotoCollection photos = await f.PhotosGetContactsPublicPhotosAsync(TestData.TestUserId, PhotoSearchExtras.OwnerName);

            Assert.That(photos[0].OwnerName, Is.Not.Null);
        }
    }
}