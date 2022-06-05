using NUnit.Framework;
using FlickrNet;
using FlickrNet.Models;
using System.Threading.Tasks;
using System.Threading;
using FlickrNet.CollectionModels;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for GeoTests
    /// </summary>
    [TestFixture]
    public class GeoTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGeoGetPermsBasicTest(CancellationToken cancellationToken = default)
        {
            GeoPermissions perms = await AuthInstance.PhotosGeoGetPermsAsync(TestData.PhotoId, cancellationToken);

            Assert.IsNotNull(perms);
            Assert.AreEqual(TestData.PhotoId, perms.PhotoId);
            Assert.IsTrue(perms.IsPublic, "IsPublic should be true.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetWithGeoDataBasicTest(CancellationToken cancellationToken = default)
        {
            PhotoCollection photos = await AuthInstance.PhotosGetWithGeoDataAsync(cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count);
            Assert.AreNotEqual(0, photos.Total);
            Assert.AreEqual(1, photos.Page);
            Assert.AreNotEqual(0, photos.PerPage);
            Assert.AreNotEqual(0, photos.Pages);

            foreach (var p in photos)
            {
                Assert.IsNotNull(p.PhotoId);
            }
        }
    }
}