using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

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
        public async Task PhotosGeoGetPermsBasicTest()
        {
            GeoPermissions perms = await AuthInstance.PhotosGeoGetPermsAsync(TestData.PhotoId, default);

            Assert.That(perms, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(perms.PhotoId, Is.EqualTo(TestData.PhotoId));
                Assert.That(perms.IsPublic, Is.True, "IsPublic should be true.");
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetWithGeoDataBasicTest()
        {
            PhotoCollection photos = await AuthInstance.PhotosGetWithGeoDataAsync(default);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty);
            Assert.Multiple(() =>
            {
                Assert.That(photos.Total, Is.Not.EqualTo(0));
                Assert.Multiple(() =>
                {
                    Assert.That(photos.Page, Is.EqualTo(1));
                    Assert.That(photos.PerPage, Is.Not.EqualTo(0));
                    Assert.That(photos.Pages, Is.Not.EqualTo(0));
                });
                foreach (var p in photos)
                {
                    Assert.That(p.PhotoId, Is.Not.Null);
                }
            });
        }
    }
}