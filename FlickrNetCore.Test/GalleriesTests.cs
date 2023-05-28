using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using Shouldly;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for GalleriesTests
    /// </summary>
    [TestFixture]
    public class GalleriesTests : BaseTest
    {
        [Test]
        public async Task GalleriesGetListUserIdTest()
        {
            Flickr f = Instance;

            GalleryCollection galleries = await f.GalleriesGetListAsync(TestData.TestUserId, default);

            Assert.That(galleries, Is.Not.Null, "GalleryCollection should not be null.");
            Assert.That(galleries, Is.Not.Empty, "Count should not be zero.");

            foreach (var g in galleries)
            {
                Assert.That(g, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(g.Title, Is.Not.Null, "Title should not be null.");
                    Assert.Multiple(() =>
                    {
                        Assert.That(g.GalleryId, Is.Not.Null, "GalleryId should not be null.");
                        Assert.That(g.GalleryUrl, Is.Not.Null, "GalleryUrl should not be null.");
                    });
                });
            }
        }

        [Test]
        public async Task GalleriesGetListForPhotoTest()
        {
            string photoId = "2891347068";

            var galleries = await Instance.GalleriesGetListForPhotoAsync(photoId, default);

            Assert.That(galleries, Is.Not.Null, "GalleryCollection should not be null.");
            Assert.That(galleries, Is.Not.Empty, "Count should not be zero.");

            foreach (var g in galleries)
            {
                Assert.That(g, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(g.Title, Is.Not.Null, "Title should not be null.");
                    Assert.Multiple(() =>
                    {
                        Assert.That(g.GalleryId, Is.Not.Null, "GalleryId should not be null.");
                        Assert.That(g.GalleryUrl, Is.Not.Null, "GalleryUrl should not be null.");
                    });
                });
            }
        }

        [Test]
        public async Task GalleriesGetPhotos()
        {
            // Dogs + Tennis Balls
            // https://www.flickr.com/photos/lesliescarter/galleries/72157622656415345
            string galleryId = "13834290-72157622656415345";

            Flickr f = Instance;

            GalleryPhotoCollection photos = await f.GalleriesGetPhotosAsync(galleryId, PhotoSearchExtras.All, default);

            Console.WriteLine(f.LastRequest);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Has.Count.EqualTo(15), "Count should be fifteen.");

            foreach (var photo in photos)
            {
                //This gallery has a comment on each photo.
                Assert.That(photo.Comment, Is.Not.Null, "GalleryPhoto.Comment shoult not be null.");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GalleriesEditPhotosTest()
        {
            Flickr.FlushCache();
            Flickr.CacheDisabled = true;

            Flickr f = AuthInstance;

            string galleryId = "78188-72157622589312064";

            var gallery = await f.GalleriesGetInfoAsync(galleryId);

            Console.WriteLine("GalleryUrl = " + gallery.GalleryUrl);

            var photos = await f.GalleriesGetPhotosAsync(galleryId, default);

            var photoIds = photos.Select(x => x.PhotoId);

            await f.GalleriesEditPhotosAsync(galleryId, gallery.PrimaryPhotoId, photoIds, default);

            var photos2 = await f.GalleriesGetPhotosAsync(gallery.GalleryId, default);

            Assert.That(photos2, Has.Count.EqualTo(photos.Count));

            for (int i = 0; i < photos.Count; i++)
            {
                Assert.That(photos2[i].PhotoId, Is.EqualTo(photos[i].PhotoId));
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GalleriesEditMetaTest()
        {
            Flickr.FlushCache();
            Flickr.CacheDisabled = true;

            Flickr f = AuthInstance;

            string galleryId = "78188-72157622589312064";

            string title = "Great Entrances to Hell";
            string description = "A guide to what makes a great photo for the Entrances to Hell group: " +
                                 "<a href=\"https://www.flickr.com/groups/entrancetohell\">www.flickr.com/groups/entrancetohell</a>\n\n";
            description += DateTime.Now.ToString();

            await f.GalleriesEditMetaAsync(galleryId, title, description, default);

            Gallery gallery = await f.GalleriesGetInfoAsync(galleryId, default);
            Assert.Multiple(() =>
            {
                Assert.That(gallery.Title, Is.EqualTo(title));
                Assert.That(gallery.Description, Is.EqualTo(description));
            });
        }

        [Test, Category("AccessTokenRequired")]
        public async Task GalleriesAddRemovePhoto()
        {
            string photoId = "18841298081";
            string galleryId = "78188-72157622589312064";
            string comment = "no comment";

            Flickr f = AuthInstance;
            await f.GalleriesAddPhotoAsync(galleryId, photoId, comment, default);

            var photos = await f.GalleriesGetPhotosAsync(galleryId, default);

            photos.ShouldContain(p => p.PhotoId == photoId);

            await f.GalleriesRemovePhoto(galleryId, photoId, "", default);

            photos = await f.GalleriesGetPhotosAsync(galleryId, default);
            photos.ShouldNotContain(p => p.PhotoId == photoId);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GalleriesEditPhotoTest()
        {
            Flickr.FlushCache();
            Flickr.CacheDisabled = true;

            string photoId = "486875512";
            string galleryId = "78188-72157622589312064";

            string comment = "You don't get much better than this for the best Entrance to Hell.\n\n" + DateTime.Now.ToString();

            Flickr f = AuthInstance;
            await f.GalleriesEditPhotoAsync(galleryId, photoId, comment, default);

            var photos = await f.GalleriesGetPhotosAsync(galleryId, default);

            bool found = false;

            foreach (var photo in photos)
            {
                if (photo.PhotoId == photoId)
                {
                    Assert.That(photo.Comment, Is.EqualTo(comment), "Comment should have been updated.");
                    found = true;
                    break;
                }
            }

            Assert.That(found, Is.True, "Should have found the photo in the gallery.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GalleriesEditComplexTest()
        {
            Flickr.CacheDisabled = true;
            Flickr.FlushCache();

            string primaryPhotoId = "486875512";
            string comment = "You don't get much better than this for the best Entrance to Hell.\n\n" + DateTime.Now.ToString();
            string galleryId = "78188-72157622589312064";

            Flickr f = AuthInstance;

            // Get photos
            var photos = await f.GalleriesGetPhotosAsync(galleryId, default);

            var photoIds = photos.Select(x => x.PhotoId).ToList();

            // Remove the last one.
            GalleryPhoto photo = photos.Last(p => p.PhotoId != primaryPhotoId);
            photoIds.Remove(photo.PhotoId);

            // Update the gallery
            await f.GalleriesEditPhotosAsync(galleryId, primaryPhotoId, photoIds, default);

            // Check removed photo no longer returned.
            var photos2 = await f.GalleriesGetPhotosAsync(galleryId, default);

            Assert.That(photos2, Has.Count.EqualTo(photos.Count - 1), "Should be one less photo.");

            bool found = false;
            foreach (var p in photos2)
            {
                if (p.PhotoId == photo.PhotoId)
                {
                    found = true;
                    break;
                }
            }
            Assert.That(false, Is.False, "Should not have found the photo in the gallery.");

            // Add photo back in
            await f.GalleriesAddPhotoAsync(galleryId, photo.PhotoId, photo.Comment, default);

            var photos3 = await f.GalleriesGetPhotosAsync(galleryId, default);
            Assert.That(photos3, Has.Count.EqualTo(photos.Count), "Count should match now photo added back in.");

            found = false;
            foreach (var p in photos3)
            {
                if (p.PhotoId == photo.PhotoId)
                {
                    Assert.That(p.Comment, Is.EqualTo(photo.Comment), "Comment should have been updated.");
                    found = true;
                    break;
                }
            }

            Assert.That(found, Is.True, "Should have found the photo in the gallery.");
        }
    }
}