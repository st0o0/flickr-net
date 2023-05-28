using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Common;
using FlickrNetCore.Enums;
using FlickrNetCore.Exceptions;
using FlickrNetCore.SearchOptions;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using Shouldly;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PhotosGetInfoTests
    /// </summary>
    [TestFixture]
    public class PhotosGetInfoTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetInfoBasicTest(CancellationToken cancellationToken = default)
        {
            PhotoInfo info = await AuthInstance.PhotosGetInfoAsync("4268023123", cancellationToken);

            Assert.IsNotNull(info);

            Assert.AreEqual("4268023123", info.PhotoId);
            Assert.AreEqual("a4283bac01", info.Secret);
            Assert.AreEqual("2795", info.Server);
            Assert.AreEqual("3", info.Farm);
            Assert.AreEqual(UtilityMethods.UnixTimestampToDate("1263291891"), info.DateUploaded);
            Assert.AreEqual(false, info.IsFavorite);
            Assert.AreEqual(LicenseType.AttributionNoncommercialShareAlikeCC, info.License);
            Assert.AreEqual(0, info.Rotation);
            Assert.AreEqual("9d3d4bf24a", info.OriginalSecret);
            Assert.AreEqual("jpg", info.OriginalFormat);
            Assert.IsTrue(info.ViewCount > 87, "ViewCount should be greater than 87.");
            Assert.AreEqual(MediaType.Photos, info.Media);

            Assert.AreEqual("12. Sudoku", info.Title);
            Assert.AreEqual("It scares me sometimes how much some of my handwriting reminds me of Dad's " +
                            "- in this photo there is one 5 that especially reminds me of his handwriting.", info.Description);

            //Owner
            Assert.AreEqual("41888973@N00", info.OwnerUserId);

            //Dates
            Assert.AreEqual(new DateTime(2010, 01, 12, 11, 01, 20), info.DateTaken, "DateTaken is not set correctly.");

            //Editability
            Assert.IsTrue(info.CanComment, "CanComment should be true when authenticated.");
            Assert.IsTrue(info.CanAddMeta, "CanAddMeta should be true when authenticated.");

            //Permissions
            Assert.AreEqual(PermissionComment.Everybody, info.PermissionComment);
            Assert.AreEqual(PermissionAddMeta.Everybody, info.PermissionAddMeta);

            //Visibility

            // Notes

            Assert.AreEqual(1, info.Notes.Count, "Notes.Count should be one.");
            Assert.AreEqual("72157623069944527", info.Notes[0].NoteId);
            Assert.AreEqual("41888973@N00", info.Notes[0].AuthorId);
            Assert.AreEqual("Sam Judson", info.Notes[0].AuthorName);
            Assert.AreEqual(267, info.Notes[0].XPosition);
            Assert.AreEqual(238, info.Notes[0].YPosition);

            // Tags

            Assert.AreEqual(5, info.Tags.Count);
            Assert.AreEqual("78188-4268023123-586", info.Tags[0].TagId);
            Assert.AreEqual("green", info.Tags[0].Raw);

            // URLs

            Assert.AreEqual(1, info.Urls.Count);
            Assert.AreEqual("photopage", info.Urls[0].UrlType);
            Assert.AreEqual("https://www.flickr.com/photos/samjudson/4268023123/", info.Urls[0].Url);
        }

        [Test]
        public async Task PhotosGetInfoUnauthenticatedTest(CancellationToken cancellationToken = default)
        {
            PhotoInfo info = await Instance.PhotosGetInfoAsync("4268023123", cancellationToken);

            Assert.IsNotNull(info);

            Assert.AreEqual("4268023123", info.PhotoId);
            Assert.AreEqual("a4283bac01", info.Secret);
            Assert.AreEqual("2795", info.Server);
            Assert.AreEqual("3", info.Farm);
            Assert.AreEqual(UtilityMethods.UnixTimestampToDate("1263291891"), info.DateUploaded);
            Assert.AreEqual(false, info.IsFavorite);
            Assert.AreEqual(LicenseType.AttributionNoncommercialShareAlikeCC, info.License);
            Assert.AreEqual(0, info.Rotation);
            Assert.AreEqual("9d3d4bf24a", info.OriginalSecret);
            Assert.AreEqual("jpg", info.OriginalFormat);
            Assert.IsTrue(info.ViewCount > 87, "ViewCount should be greater than 87.");
            Assert.AreEqual(MediaType.Photos, info.Media);

            Assert.AreEqual("12. Sudoku", info.Title);
            Assert.AreEqual("It scares me sometimes how much some of my handwriting reminds me of Dad's " +
                            "- in this photo there is one 5 that especially reminds me of his handwriting.", info.Description);

            //Owner
            Assert.AreEqual("41888973@N00", info.OwnerUserId);

            //Dates

            //Editability
            Assert.IsFalse(info.CanComment, "CanComment should be false when not authenticated.");
            Assert.IsFalse(info.CanAddMeta, "CanAddMeta should be false when not authenticated.");

            //Permissions
            Assert.IsNull(info.PermissionComment, "PermissionComment should be null when not authenticated.");
            Assert.IsNull(info.PermissionAddMeta, "PermissionAddMeta should be null when not authenticated.");

            //Visibility

            // Notes

            Assert.AreEqual(1, info.Notes.Count, "Notes.Count should be one.");
            Assert.AreEqual("72157623069944527", info.Notes[0].NoteId);
            Assert.AreEqual("41888973@N00", info.Notes[0].AuthorId);
            Assert.AreEqual("Sam Judson", info.Notes[0].AuthorName);
            Assert.AreEqual(267, info.Notes[0].XPosition);
            Assert.AreEqual(238, info.Notes[0].YPosition);

            // Tags

            Assert.AreEqual(5, info.Tags.Count);
            Assert.AreEqual("78188-4268023123-586", info.Tags[0].TagId);
            Assert.AreEqual("green", info.Tags[0].Raw);

            // URLs

            Assert.AreEqual(1, info.Urls.Count);
            Assert.AreEqual("photopage", info.Urls[0].UrlType);
            Assert.AreEqual("https://www.flickr.com/photos/samjudson/4268023123/", info.Urls[0].Url);
        }

        [Test]
        public async Task PhotosGetInfoTestUserIssue(CancellationToken cancellationToken = default)
        {
            var photoId = "14042679057";
            var info = await Instance.PhotosGetInfoAsync(photoId, cancellationToken);

            Assert.AreEqual(photoId, info.PhotoId);
            Assert.AreEqual("63226137@N02", info.OwnerUserId);
            Assert.AreEqual("https://www.flickr.com/photos/63226137@N02/14042679057/", info.WebUrl);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetInfoTestLocation(CancellationToken cancellationToken = default)
        {
            const string photoId = "4268756940";

            PhotoInfo info = await AuthInstance.PhotosGetInfoAsync(photoId, cancellationToken);

            Assert.IsNotNull(info.Location);
        }

        [Test]
        public async Task PhotosGetInfoWithPeople(CancellationToken cancellationToken = default)
        {
            const string photoId = "3547137580"; // https://www.flickr.com/photos/samjudson/3547137580/in/photosof-samjudson/

            PhotoInfo info = await Instance.PhotosGetInfoAsync(photoId, cancellationToken);

            Assert.IsNotNull(info);
            Assert.IsTrue(info.HasPeople, "HasPeople should be true.");
        }

        [Test]
        public async Task PhotosGetInfoCanBlogTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                UserId = TestData.TestUserId,
                PerPage = 5
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);
            PhotoInfo info = await Instance.PhotosGetInfoAsync(photos[0].PhotoId, cancellationToken);

            Assert.AreEqual(false, info.CanBlog);
            Assert.AreEqual(true, info.CanDownload);
        }

        [Test]
        public async Task PhotosGetInfoDataTakenGranularityTest(CancellationToken cancellationToken = default)
        {
            const string photoid = "4386780023";

            PhotoInfo info = await Instance.PhotosGetInfoAsync(photoid, cancellationToken);

            Assert.AreEqual(new DateTime(2009, 1, 1), info.DateTaken);
            Assert.AreEqual(DateGranularity.Circa, info.DateTakenGranularity);
        }

        [Test]
        public async Task PhotosGetInfoVideoTest(CancellationToken cancellationToken = default)
        {
            const string videoId = "2926486605";

            var info = await Instance.PhotosGetInfoAsync(videoId, cancellationToken);

            Assert.IsNotNull(info);
            Assert.AreEqual(videoId, info.PhotoId);
        }

        [Test]
        public void TestPhotoNotFound(CancellationToken cancellationToken = default)
        {
            Should.Throw<PhotoNotFoundException>(async () => await Instance.PhotosGetInfoAsync("abcd", cancellationToken));
        }

        [Test]
        public async Task ShouldReturnPhotoInfoWithGeoData(CancellationToken cancellationToken = default)
        {
            var info = await Instance.PhotosGetInfoAsync("54071193", cancellationToken);

            Assert.IsNotNull(info, "PhotoInfo should not be null.");
            Assert.IsNotNull(info.Location, "Location should not be null.");
            Assert.AreEqual(-180, info.Location.Longitude, "Longitude should be -180");
            Assert.AreEqual("https://www.flickr.com/photos/afdn/54071193/", info.Urls[0].Url);
            Assert.IsTrue(info.GeoPermissions.IsPublic, "GeoPermissions should be public.");
        }

        [Test]
        public async Task ShouldReturnPhotoInfoWithValidUrls(CancellationToken cancellationToken = default)
        {
            var info = await Instance.PhotosGetInfoAsync("9671143400", cancellationToken);

            Assert.IsTrue(await UrlHelper.Exists(info.Small320Url, cancellationToken), "Small320Url is not valid url : " + info.Small320Url);
            Assert.IsTrue(await UrlHelper.Exists(info.Medium640Url, cancellationToken), "Medium640Url is not valid url : " + info.Medium640Url);
            Assert.IsTrue(await UrlHelper.Exists(info.Medium800Url, cancellationToken), "Medium800Url is not valid url : " + info.Medium800Url);
            Assert.AreNotEqual(info.SmallUrl, info.LargeUrl, "URLs should all be different.");
        }

        [Test]
        [Ignore("Photo urls appear to have changed to start with 'live' so test is invalid")]
        public async Task PhotoInfoUrlsShouldMatchSizes(CancellationToken cancellationToken = default)
        {
            var photos = await Instance.PhotosSearchAsync(new PhotoSearchOptions
            {
                UserId = TestData.TestUserId,
                PerPage = 1,
                Extras = PhotoSearchExtras.AllUrls
            }, cancellationToken);

            var photo = photos.First();

            var info = await Instance.PhotosGetInfoAsync(photo.PhotoId, cancellationToken);

            Assert.AreEqual(photo.LargeUrl, info.LargeUrl);
            Assert.AreEqual(photo.Small320Url, info.Small320Url);
        }

        [Test]
        [TestCase("46611802@N00", "")]
        public async Task GetInfoWithInvalidXmlTests(string userId, string location, CancellationToken cancellationToken = default)
        {
            var userInfo = await Instance.PeopleGetInfoAsync(userId, cancellationToken);
            Assert.That(userInfo.UserId, Is.EqualTo(userId));
            Assert.That(userInfo.Location, Is.EqualTo(location));
        }
    }
}