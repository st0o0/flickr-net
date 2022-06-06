using FlickrNet.CollectionModels;
using FlickrNet.Models;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    [TestFixture]
    public class PhotosGetFavouritesTests : BaseTest
    {
        [Test]
        public async Task PhotosGetFavoritesNoFavourites(CancellationToken cancellationToken = default)
        {
            // No favourites
            PhotoFavoriteCollection favs = await Instance.PhotosGetFavoritesAsync(TestData.PhotoId, cancellationToken);

            Assert.AreEqual(0, favs.Count, "Should have no favourites");
        }

        [Test]
        public async Task PhotosGetFavoritesHasFavourites(CancellationToken cancellationToken = default)
        {
            PhotoFavoriteCollection favs = await Instance.PhotosGetFavoritesAsync(TestData.FavouritedPhotoId, 500, 1, cancellationToken);

            Assert.IsNotNull(favs, "PhotoFavourites instance should not be null.");

            Assert.IsTrue(favs.Count > 0, "PhotoFavourites.Count should not be zero.");

            Assert.AreEqual(50, favs.Count, "Should be 50 favourites listed (maximum returned)");

            foreach (PhotoFavorite p in favs)
            {
                Assert.IsFalse(string.IsNullOrEmpty(p.UserId), "Should have a user ID.");
                Assert.IsFalse(string.IsNullOrEmpty(p.UserName), "Should have a user name.");
                Assert.AreNotEqual(default(DateTime), p.FavoriteDate, "Favourite Date should not be default Date value");
                Assert.IsTrue(p.FavoriteDate < DateTime.Now, "Favourite Date should be in the past.");
            }
        }

        [Test]
        public async Task PhotosGetFavoritesPaging(CancellationToken cancellationToken = default)
        {
            PhotoFavoriteCollection favs = await Instance.PhotosGetFavoritesAsync(TestData.FavouritedPhotoId, 10, 1, cancellationToken);

            Assert.AreEqual(10, favs.Count, "PhotoFavourites.Count should be 10.");
            Assert.AreEqual(10, favs.PerPage, "PhotoFavourites.PerPage should be 10");
            Assert.AreEqual(1, favs.Page, "PhotoFavourites.Page should be 1.");
            Assert.IsTrue(favs.Total > 100, "PhotoFavourites.Total should be greater than 100.");
            Assert.IsTrue(favs.Pages > 10, "PhotoFavourites.Pages should be greater than 10.");
        }

        [Test]
        public async Task PhotosGetFavoritesPagingTwo(CancellationToken cancellationToken = default)
        {
            PhotoFavoriteCollection favs = await Instance.PhotosGetFavoritesAsync(TestData.FavouritedPhotoId, 10, 2, cancellationToken);

            Assert.AreEqual(10, favs.Count, "PhotoFavourites.Count should be 10.");
            Assert.AreEqual(10, favs.PerPage, "PhotoFavourites.PerPage should be 10");
            Assert.AreEqual(2, favs.Page, "PhotoFavourites.Page should be 2.");
        }
    }
}