using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using System.ComponentModel;
using CategoryAttribute = NUnit.Framework.CategoryAttribute;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for FavouritesGetPublicListTests
    /// </summary>
    [TestFixture]
    public class FavoritesTests : BaseTest
    {
        [Test]
        public async Task FavoritesGetPublicListBasicTest()
        {
            const string userId = "77788903@N00";

            var p = await Instance.FavoritesGetPublicListAsync(userId, default);

            Assert.That(p, Is.Not.Null, "PhotoCollection should not be null instance.");
            Assert.That(p, Is.Not.Empty, "PhotoCollection.Count should be greater than zero.");
        }

        [Test]
        public async Task FavouritesGetPublicListWithDates()
        {
            var allFavourites = await Instance.FavoritesGetPublicListAsync(TestData.TestUserId, default);

            var firstFiveFavourites = allFavourites.OrderBy(p => p.DateFavorited).Take(5).ToList();

            var minDate = firstFiveFavourites.Min(p => p.DateFavorited).GetValueOrDefault();
            var maxDate = firstFiveFavourites.Max(p => p.DateFavorited).GetValueOrDefault();

            var subsetOfFavourites = await Instance.FavoritesGetPublicListAsync(TestData.TestUserId, minDate, maxDate,
                                                                     PhotoSearchExtras.None, 0, 0, default);

            Assert.That(subsetOfFavourites, Has.Count.EqualTo(5), "Should be 5 favourites in subset");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task FavoritesGetListBasicTest()
        {
            var photos = await AuthInstance.FavoritesGetListAsync(default);
            Assert.That(photos, Is.Not.Null, "PhotoCollection should not be null instance.");
            Assert.That(photos, Is.Not.Empty, "PhotoCollection.Count should be greater than zero.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task FavoritesGetListFullParamTest()
        {
            var photos = await AuthInstance.FavoritesGetListAsync(TestData.TestUserId, DateTime.Now.AddYears(-4), DateTime.Now, PhotoSearchExtras.All, 1, 10, default);
            Assert.That(photos, Is.Not.Null, "PhotoCollection should not be null.");

            Assert.That(photos, Is.Not.Empty, "Count should be greater than zero.");
        }

        [Test]
        public async Task FavoritesGetContext()
        {
            const string photoId = "2502963121";
            const string userId = "41888973@N00";

            var context = await Instance.FavoritesGetContextAsync(photoId, userId, default);

            Assert.That(context, Is.Not.Null);
            Assert.That(context.Count, Is.Not.EqualTo(0), "Count should be greater than zero");
            Assert.Multiple(() =>
            {
                Assert.That(context.PreviousPhotos, Has.Count.EqualTo(1), "Should be 1 previous photo.");
                Assert.That(context.NextPhotos, Has.Count.EqualTo(1), "Should be 1 next photo.");
            });
        }

        [Test]
        public async Task FavoritesGetContextMorePrevious()
        {
            const string photoId = "2502963121";
            const string userId = "41888973@N00";

            var context = await Instance.FavoritesGetContextAsync(photoId, userId, 3, 4, PhotoSearchExtras.Description, default);

            Assert.That(context, Is.Not.Null);
            Assert.That(context.Count, Is.Not.EqualTo(0), "Count should be greater than zero");
            Assert.Multiple(() =>
            {
                Assert.That(context.PreviousPhotos, Has.Count.EqualTo(3), "Should be 3 previous photo.");
                Assert.That(context.NextPhotos, Has.Count.EqualTo(4), "Should be 4 next photo.");
            });
        }
    }
}