using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    public class PhotosGetFavouritesTests : BaseTest
    {
        [Test]
        public async Task PhotosGetFavoritesNoFavourites()
        {
            // No favourites
            PhotoFavoriteCollection favs = await Instance.PhotosGetFavoritesAsync(TestData.PhotoId);

            Assert.That(favs.Count, Is.EqualTo(0), "Should have no favourites");
        }

        [Test]
        public async Task PhotosGetFavoritesHasFavourites()
        {
            PhotoFavoriteCollection favs = await Instance.PhotosGetFavoritesAsync(TestData.FavouritedPhotoId, 500, 1);

            Assert.That(favs, Is.Not.Null, "PhotoFavourites instance should not be null.");
            Assert.Multiple(() =>
            {
                Assert.That(favs.Count > 0, Is.True, "PhotoFavourites.Count should not be zero.");

                Assert.That(favs, Has.Count.EqualTo(50), "Should be 50 favourites listed (maximum returned)");
            });
            foreach (PhotoFavorite p in favs)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(string.IsNullOrEmpty(p.UserId), Is.False, "Should have a user ID.");
                    Assert.Multiple(() =>
                {
                    Assert.That(string.IsNullOrEmpty(p.UserName), Is.False, "Should have a user name.");
                    Assert.That(p.FavoriteDate, Is.Not.EqualTo(default(DateTime)), "Favourite Date should not be default Date value");
                });
                    Assert.That(p.FavoriteDate < DateTime.Now, Is.True, "Favourite Date should be in the past.");
                });
            }
        }

        [Test]
        public async Task PhotosGetFavoritesPaging()
        {
            PhotoFavoriteCollection favs = await Instance.PhotosGetFavoritesAsync(TestData.FavouritedPhotoId, 10, 1);

            Assert.That(favs, Has.Count.EqualTo(10), "PhotoFavourites.Count should be 10.");
            Assert.Multiple(() =>
            {
                Assert.That(favs.PerPage, Is.EqualTo(10), "PhotoFavourites.PerPage should be 10");
                Assert.That(favs.Page, Is.EqualTo(1), "PhotoFavourites.Page should be 1.");
            });
            Assert.Multiple(() =>
            {
                Assert.That(favs.Total > 100, Is.True, "PhotoFavourites.Total should be greater than 100.");
                Assert.That(favs.Pages > 10, Is.True, "PhotoFavourites.Pages should be greater than 10.");
            });
        }

        [Test]
        public async Task PhotosGetFavoritesPagingTwo()
        {
            PhotoFavoriteCollection favs = await Instance.PhotosGetFavoritesAsync(TestData.FavouritedPhotoId, 10, 2);

            Assert.That(favs, Has.Count.EqualTo(10), "PhotoFavourites.Count should be 10.");
            Assert.Multiple(() =>
            {
                Assert.That(favs.PerPage, Is.EqualTo(10), "PhotoFavourites.PerPage should be 10");
                Assert.That(favs.Page, Is.EqualTo(2), "PhotoFavourites.Page should be 2.");
            });
        }
    }
}