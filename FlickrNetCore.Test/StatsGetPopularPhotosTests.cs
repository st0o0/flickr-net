using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class StatsGetPopularPhotosTests : BaseTest
    {
        [Test]
        public async Task StatsGetPopularPhotosBasic()
        {
            PopularPhotoCollection photos = await AuthInstance.StatsGetPopularPhotosAsync(DateTime.MinValue, PopularitySort.None, 0, 0);

            Assert.That(photos, Is.Not.Null, "PopularPhotos should not be null.");
            Assert.Multiple(() =>
            {
                Assert.That(photos.Total, Is.Not.EqualTo(0), "PopularPhotos.Total should not be zero.");
                Assert.Multiple(() =>
            {
                Assert.That(photos, Is.Not.Empty, "PopularPhotos.Count should not be zero.");
                Assert.That(Math.Min(photos.Total, photos.PerPage), Is.EqualTo(photos.Count), "PopularPhotos.Count should equal either PopularPhotos.Total or PopularPhotos.PerPage.");
            });
                foreach (Photo p in photos)
                {
                    Assert.That(p.PhotoId, Is.Not.Null, "Photo.PhotoId should not be null.");
                }

                foreach (PopularPhoto p in photos)
                {
                    Assert.Multiple(() =>
                    {
                        Assert.That(p.PhotoId, Is.Not.Null, "PopularPhoto.PhotoId should not be null.");
                        Assert.That(p.StatViews, Is.Not.EqualTo(0), "PopularPhoto.StatViews should not be zero.");
                    });
                }
            });
        }

        [Test]
        public async Task StatsGetPopularPhotosNoParamsTest()
        {
            Flickr f = AuthInstance;

            PopularPhotoCollection photos = await f.StatsGetPopularPhotosAsync();

            Assert.That(photos, Is.Not.Null, "PopularPhotos should not be null.");
            Assert.Multiple(() =>
            {
                Assert.That(photos.Total, Is.Not.EqualTo(0), "PopularPhotos.Total should not be zero.");
                Assert.Multiple(() =>
            {
                Assert.That(photos, Is.Not.Empty, "PopularPhotos.Count should not be zero.");
                Assert.That(Math.Min(photos.Total, photos.PerPage), Is.EqualTo(photos.Count), "PopularPhotos.Count should equal either PopularPhotos.Total or PopularPhotos.PerPage.");
            });
                foreach (Photo p in photos)
                {
                    Assert.That(p.PhotoId, Is.Not.Null, "Photo.PhotoId should not be null.");
                }

                foreach (PopularPhoto p in photos)
                {
                    Assert.Multiple(() =>
                {
                    Assert.That(p.PhotoId, Is.Not.Null, "PopularPhoto.PhotoId should not be null.");
                    Assert.That(p.StatViews, Is.Not.EqualTo(0), "PopularPhoto.StatViews should not be zero.");
                });
                }
            });
        }

        [Test]
        public async Task StatsGetPopularPhotosOtherTest()
        {
            var lastWeek = DateTime.Today.AddDays(-7);

            var photos = await AuthInstance.StatsGetPopularPhotosAsync(lastWeek);
            Assert.That(photos, Is.Not.Null, "PopularPhotos should not be null.");

            photos = await AuthInstance.StatsGetPopularPhotosAsync(PopularitySort.Favorites);
            Assert.That(photos, Is.Not.Null, "PopularPhotos should not be null.");

            photos = await AuthInstance.StatsGetPopularPhotosAsync(lastWeek, 1, 10);
            Assert.That(photos, Is.Not.Null, "PopularPhotos should not be null.");
            Assert.That(photos, Has.Count.EqualTo(10), "Date search popular photos should return 10 photos.");

            photos = await AuthInstance.StatsGetPopularPhotosAsync(PopularitySort.Favorites, 1, 10);
            Assert.That(photos, Is.Not.Null, "PopularPhotos should not be null.");
            Assert.That(photos, Has.Count.EqualTo(10), "Favorite search popular photos should return 10 photos.");
        }
    }
}