using FlickrNet;
using FlickrNet.CollectionModels;
using FlickrNet.Enums;
using FlickrNet.SearchOptions;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PhotoOwnerNameTest
    /// </summary>
    [TestFixture]
    public class PhotoOwnerNameTest : BaseTest
    {
        [Test]
        public async Task PhotosSearchOwnerNameTest(CancellationToken cancellationToken = default)
        {
            PhotoSearchOptions o = new()
            {
                UserId = TestData.TestUserId,
                PerPage = 10,
                Extras = PhotoSearchExtras.OwnerName
            };

            Flickr f = Instance;
            PhotoCollection photos = await f.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos[0].OwnerName);
        }

        [Test]
        public async Task PhotosGetContactsPublicPhotosOwnerNameTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;
            PhotoCollection photos = await f.PhotosGetContactsPublicPhotosAsync(TestData.TestUserId, PhotoSearchExtras.OwnerName, cancellationToken);

            Assert.IsNotNull(photos[0].OwnerName);
        }
    }
}