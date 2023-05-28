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
        public async Task StatsGetPopularPhotosBasic(CancellationToken cancellationToken = default)
        {
            PopularPhotoCollection photos = await AuthInstance.StatsGetPopularPhotosAsync(DateTime.MinValue, PopularitySort.None, 0, 0, cancellationToken);

            Assert.IsNotNull(photos, "PopularPhotos should not be null.");

            Assert.AreNotEqual(0, photos.Total, "PopularPhotos.Total should not be zero.");
            Assert.AreNotEqual(0, photos.Count, "PopularPhotos.Count should not be zero.");
            Assert.AreEqual(photos.Count, Math.Min(photos.Total, photos.PerPage), "PopularPhotos.Count should equal either PopularPhotos.Total or PopularPhotos.PerPage.");

            foreach (Photo p in photos)
            {
                Assert.IsNotNull(p.PhotoId, "Photo.PhotoId should not be null.");
            }

            foreach (PopularPhoto p in photos)
            {
                Assert.IsNotNull(p.PhotoId, "PopularPhoto.PhotoId should not be null.");
                Assert.AreNotEqual(0, p.StatViews, "PopularPhoto.StatViews should not be zero.");
            }
        }

        [Test]
        public async Task StatsGetPopularPhotosNoParamsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            PopularPhotoCollection photos = await f.StatsGetPopularPhotosAsync(cancellationToken);

            Assert.IsNotNull(photos, "PopularPhotos should not be null.");

            Assert.AreNotEqual(0, photos.Total, "PopularPhotos.Total should not be zero.");
            Assert.AreNotEqual(0, photos.Count, "PopularPhotos.Count should not be zero.");
            Assert.AreEqual(photos.Count, Math.Min(photos.Total, photos.PerPage), "PopularPhotos.Count should equal either PopularPhotos.Total or PopularPhotos.PerPage.");

            foreach (Photo p in photos)
            {
                Assert.IsNotNull(p.PhotoId, "Photo.PhotoId should not be null.");
            }

            foreach (PopularPhoto p in photos)
            {
                Assert.IsNotNull(p.PhotoId, "PopularPhoto.PhotoId should not be null.");
                Assert.AreNotEqual(0, p.StatViews, "PopularPhoto.StatViews should not be zero.");
            }
        }

        [Test]
        public async Task StatsGetPopularPhotosOtherTest(CancellationToken cancellationToken = default)
        {
            var lastWeek = DateTime.Today.AddDays(-7);

            var photos = await AuthInstance.StatsGetPopularPhotosAsync(lastWeek, cancellationToken);
            Assert.IsNotNull(photos, "PopularPhotos should not be null.");

            photos = await AuthInstance.StatsGetPopularPhotosAsync(PopularitySort.Favorites, cancellationToken);
            Assert.IsNotNull(photos, "PopularPhotos should not be null.");

            photos = await AuthInstance.StatsGetPopularPhotosAsync(lastWeek, 1, 10, cancellationToken);
            Assert.IsNotNull(photos, "PopularPhotos should not be null.");
            Assert.AreEqual(10, photos.Count, "Date search popular photos should return 10 photos.");

            photos = await AuthInstance.StatsGetPopularPhotosAsync(PopularitySort.Favorites, 1, 10, cancellationToken);
            Assert.IsNotNull(photos, "PopularPhotos should not be null.");
            Assert.AreEqual(10, photos.Count, "Favorite search popular photos should return 10 photos.");
        }
    }
}