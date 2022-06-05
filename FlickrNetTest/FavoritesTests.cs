using System;
using System.Linq;

using NUnit.Framework;
using FlickrNet;
using System.Threading.Tasks;
using System.Threading;
using FlickrNet.CollectionModels;
using FlickrNet.Enums;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for FavouritesGetPublicListTests
    /// </summary>
    [TestFixture]
    public class FavoritesTests : BaseTest
    {
        [Test]
        public async Task FavoritesGetPublicListBasicTest(CancellationToken cancellationToken = default)
        {
            const string userId = "77788903@N00";

            var p = await Instance.FavoritesGetPublicListAsync(userId, cancellationToken);

            Assert.IsNotNull(p, "PhotoCollection should not be null instance.");
            Assert.AreNotEqual(0, p.Count, "PhotoCollection.Count should be greater than zero.");
        }

        [Test]
        public async Task FavouritesGetPublicListWithDates(CancellationToken cancellationToken = default)
        {
            var allFavourites = await Instance.FavoritesGetPublicListAsync(TestData.TestUserId, cancellationToken);

            var firstFiveFavourites = allFavourites.OrderBy(p => p.DateFavorited).Take(5).ToList();

            var minDate = firstFiveFavourites.Min(p => p.DateFavorited).GetValueOrDefault();
            var maxDate = firstFiveFavourites.Max(p => p.DateFavorited).GetValueOrDefault();

            var subsetOfFavourites = await Instance.FavoritesGetPublicListAsync(TestData.TestUserId, minDate, maxDate,
                                                                     PhotoSearchExtras.None, 0, 0, cancellationToken);

            Assert.AreEqual(5, subsetOfFavourites.Count, "Should be 5 favourites in subset");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task FavoritesGetListBasicTest(CancellationToken cancellationToken = default)
        {
            var photos = await AuthInstance.FavoritesGetListAsync(cancellationToken);
            Assert.IsNotNull(photos, "PhotoCollection should not be null instance.");
            Assert.AreNotEqual(0, photos.Count, "PhotoCollection.Count should be greater than zero.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task FavoritesGetListFullParamTest(CancellationToken cancellationToken = default)
        {
            var photos = await AuthInstance.FavoritesGetListAsync(TestData.TestUserId, DateTime.Now.AddYears(-4), DateTime.Now, PhotoSearchExtras.All, 1, 10, cancellationToken);
            Assert.IsNotNull(photos, "PhotoCollection should not be null.");

            Assert.IsTrue(photos.Count > 0, "Count should be greater than zero.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task FavoritesGetListPartialParamTest(CancellationToken cancellationToken = default)
        {
            PhotoCollection photos = await AuthInstance.FavoritesGetListAsync(TestData.TestUserId, 2, 20, cancellationToken);
            Assert.IsNotNull(photos, "PhotoCollection should not be null instance.");
            Assert.AreNotEqual(0, photos.Count, "PhotoCollection.Count should be greater than zero.");
            Assert.AreEqual(2, photos.Page);
            Assert.AreEqual(20, photos.PerPage);
            Assert.AreEqual(20, photos.Count);
        }

        [Test]
        public async Task FavoritesGetContext(CancellationToken cancellationToken = default)
        {
            const string photoId = "2502963121";
            const string userId = "41888973@N00";

            var context = await Instance.FavoritesGetContextAsync(photoId, userId, cancellationToken);

            Assert.IsNotNull(context);
            Assert.AreNotEqual(0, context.Count, "Count should be greater than zero");
            Assert.AreEqual(1, context.PreviousPhotos.Count, "Should be 1 previous photo.");
            Assert.AreEqual(1, context.NextPhotos.Count, "Should be 1 next photo.");
        }

        [Test]
        public async Task FavoritesGetContextMorePrevious(CancellationToken cancellationToken = default)
        {
            const string photoId = "2502963121";
            const string userId = "41888973@N00";

            var context = await Instance.FavoritesGetContextAsync(photoId, userId, 3, 4, PhotoSearchExtras.Description, cancellationToken);

            Assert.IsNotNull(context);
            Assert.AreNotEqual(0, context.Count, "Count should be greater than zero");
            Assert.AreEqual(3, context.PreviousPhotos.Count, "Should be 3 previous photo.");
            Assert.AreEqual(4, context.NextPhotos.Count, "Should be 4 next photo.");
        }
    }
}