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
        public async Task PhotosGetInfoBasicTest()
        {
            PhotoInfo info = await AuthInstance.PhotosGetInfoAsync("4268023123");

            Assert.That(info, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(info.PhotoId, Is.EqualTo("4268023123"));
                Assert.Multiple(() =>
            {
                Assert.That(info.Secret, Is.EqualTo("a4283bac01"));
                Assert.That(info.Server, Is.EqualTo("2795"));
                Assert.That(info.Farm, Is.EqualTo("3"));
                Assert.That(info.DateUploaded, Is.EqualTo(UtilityMethods.UnixTimestampToDate("1263291891")));
                Assert.That(info.IsFavorite, Is.EqualTo(false));
                Assert.That(info.License, Is.EqualTo(LicenseType.AttributionNoncommercialShareAlikeCC));
                Assert.That(info.Rotation, Is.EqualTo(0));
                Assert.That(info.OriginalSecret, Is.EqualTo("9d3d4bf24a"));
                Assert.That(info.OriginalFormat, Is.EqualTo("jpg"));
            });
                Assert.That(info.ViewCount > 87, Is.True, "ViewCount should be greater than 87.");
                Assert.Multiple(() =>
                {
                    Assert.That(info.Media, Is.EqualTo(MediaType.Photos));
                    Assert.Multiple(() =>
                {
                    Assert.That(info.Title, Is.EqualTo("12. Sudoku"));
                    Assert.That(info.Description, Is.EqualTo("It scares me sometimes how much some of my handwriting reminds me of Dad's " +
                                    "- in this photo there is one 5 that especially reminds me of his handwriting."));

                    //Owner
                    Assert.That(info.OwnerUserId, Is.EqualTo("41888973@N00"));

                    //Dates
                    Assert.That(info.DateTaken, Is.EqualTo(new DateTime(2010, 01, 12, 11, 01, 20)), "DateTaken is not set correctly.");
                });

                    //Editability
                    Assert.That(info.CanComment, Is.True, "CanComment should be true when authenticated.");
                    Assert.That(info.CanAddMeta, Is.True, "CanAddMeta should be true when authenticated.");
                    Assert.Multiple(() =>
                    {
                        //Permissions
                        Assert.That(info.PermissionComment, Is.EqualTo(PermissionComment.Everybody));
                        Assert.Multiple(() =>
                {
                    Assert.That(info.PermissionAddMeta, Is.EqualTo(PermissionAddMeta.Everybody));

                    //Visibility

                    // Notes

                    Assert.That(info.Notes, Has.Count.EqualTo(1), "Notes.Count should be one.");
                });
                        Assert.Multiple(() =>
                        {
                            Assert.That(info.Notes[0].NoteId, Is.EqualTo("72157623069944527"));
                            Assert.Multiple(() =>
                            {
                                Assert.That(info.Notes[0].AuthorId, Is.EqualTo("41888973@N00"));
                                Assert.That(info.Notes[0].AuthorName, Is.EqualTo("Sam Judson"));
                                Assert.That(info.Notes[0].XPosition, Is.EqualTo(267));
                                Assert.That(info.Notes[0].YPosition, Is.EqualTo(238));

                                // Tags

                                Assert.That(info.Tags, Has.Count.EqualTo(5));
                            });
                            Assert.Multiple(() =>
                            {
                                Assert.That(info.Tags[0].TagId, Is.EqualTo("78188-4268023123-586"));
                                Assert.Multiple(() =>
                            {
                                Assert.That(info.Tags[0].Raw, Is.EqualTo("green"));

                                // URLs

                                Assert.That(info.Urls, Has.Count.EqualTo(1));
                            });
                                Assert.Multiple(() =>
                            {
                                Assert.That(info.Urls[0].UrlType, Is.EqualTo("photopage"));
                                Assert.That(info.Urls[0].Url, Is.EqualTo("https://www.flickr.com/photos/samjudson/4268023123/"));
                            });
                            });
                        });
                    });
                });
            });
        }

        [Test]
        public async Task PhotosGetInfoUnauthenticatedTest()
        {
            PhotoInfo info = await Instance.PhotosGetInfoAsync("4268023123");

            Assert.That(info, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(info.PhotoId, Is.EqualTo("4268023123"));
                Assert.Multiple(() =>
        {
            Assert.That(info.Secret, Is.EqualTo("a4283bac01"));
            Assert.That(info.Server, Is.EqualTo("2795"));
            Assert.That(info.Farm, Is.EqualTo("3"));
            Assert.That(info.DateUploaded, Is.EqualTo(UtilityMethods.UnixTimestampToDate("1263291891")));
            Assert.That(info.IsFavorite, Is.EqualTo(false));
            Assert.That(info.License, Is.EqualTo(LicenseType.AttributionNoncommercialShareAlikeCC));
            Assert.That(info.Rotation, Is.EqualTo(0));
            Assert.That(info.OriginalSecret, Is.EqualTo("9d3d4bf24a"));
            Assert.That(info.OriginalFormat, Is.EqualTo("jpg"));
        });
                Assert.That(info.ViewCount > 87, Is.True, "ViewCount should be greater than 87.");
                Assert.Multiple(() =>
                {
                    Assert.That(info.Media, Is.EqualTo(MediaType.Photos));
                    Assert.Multiple(() =>
                        {
                            Assert.That(info.Title, Is.EqualTo("12. Sudoku"));
                            Assert.That(info.Description, Is.EqualTo("It scares me sometimes how much some of my handwriting reminds me of Dad's " +
                                        "- in this photo there is one 5 that especially reminds me of his handwriting."));

                            //Owner
                            Assert.That(info.OwnerUserId, Is.EqualTo("41888973@N00"));
                        });

                    //Dates

                    //Editability
                    Assert.That(info.CanComment, Is.False, "CanComment should be false when not authenticated.");
                    Assert.That(info.CanAddMeta, Is.False, "CanAddMeta should be false when not authenticated.");

                    //Permissions
                    Assert.That(info.PermissionComment, Is.Null, "PermissionComment should be null when not authenticated.");
                    Assert.That(info.PermissionAddMeta, Is.Null, "PermissionAddMeta should be null when not authenticated.");

                    //Visibility

                    // Notes

                    Assert.That(info.Notes, Has.Count.EqualTo(1), "Notes.Count should be one.");
                    Assert.Multiple(() =>
                    {
                        Assert.That(info.Notes[0].NoteId, Is.EqualTo("72157623069944527"));
                        Assert.Multiple(() =>
                    {
                        Assert.That(info.Notes[0].AuthorId, Is.EqualTo("41888973@N00"));
                        Assert.That(info.Notes[0].AuthorName, Is.EqualTo("Sam Judson"));
                        Assert.That(info.Notes[0].XPosition, Is.EqualTo(267));
                        Assert.That(info.Notes[0].YPosition, Is.EqualTo(238));

                        // Tags

                        Assert.That(info.Tags, Has.Count.EqualTo(5));
                    });
                        Assert.Multiple(() =>
                    {
                        Assert.That(info.Tags[0].TagId, Is.EqualTo("78188-4268023123-586"));
                        Assert.Multiple(() =>
                    {
                        Assert.That(info.Tags[0].Raw, Is.EqualTo("green"));

                        // URLs

                        Assert.That(info.Urls, Has.Count.EqualTo(1));
                    });
                        Assert.Multiple(() =>
                    {
                        Assert.That(info.Urls[0].UrlType, Is.EqualTo("photopage"));
                        Assert.That(info.Urls[0].Url, Is.EqualTo("https://www.flickr.com/photos/samjudson/4268023123/"));
                    });
                    });
                    });
                });
            });
        }

        [Test]
        public async Task PhotosGetInfoTestUserIssue()
        {
            var photoId = "14042679057";
            var info = await Instance.PhotosGetInfoAsync(photoId);
            Assert.Multiple(() =>
            {
                Assert.That(info.PhotoId, Is.EqualTo(photoId));
                Assert.Multiple(() =>
                {
                    Assert.That(info.OwnerUserId, Is.EqualTo("63226137@N02"));
                    Assert.That(info.WebUrl, Is.EqualTo("https://www.flickr.com/photos/63226137@N02/14042679057/"));
                });
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetInfoTestLocation()
        {
            const string photoId = "4268756940";

            PhotoInfo info = await AuthInstance.PhotosGetInfoAsync(photoId);

            Assert.That(info.Location, Is.Not.Null);
        }

        [Test]
        public async Task PhotosGetInfoWithPeople()
        {
            const string photoId = "3547137580"; // https://www.flickr.com/photos/samjudson/3547137580/in/photosof-samjudson/

            PhotoInfo info = await Instance.PhotosGetInfoAsync(photoId);

            Assert.That(info, Is.Not.Null);
            Assert.That(info.HasPeople, Is.True, "HasPeople should be true.");
        }

        [Test]
        public async Task PhotosGetInfoCanBlogTest()
        {
            var o = new PhotoSearchOptions
            {
                UserId = TestData.TestUserId,
                PerPage = 5
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);
            PhotoInfo info = await Instance.PhotosGetInfoAsync(photos[0].PhotoId);
            Assert.Multiple(() =>
        {
            Assert.That(info.CanBlog, Is.EqualTo(false));
            Assert.That(info.CanDownload, Is.EqualTo(true));
        });
        }

        [Test]
        public async Task PhotosGetInfoDataTakenGranularityTest()
        {
            const string photoid = "4386780023";

            PhotoInfo info = await Instance.PhotosGetInfoAsync(photoid);
            Assert.Multiple(() =>
        {
            Assert.That(info.DateTaken, Is.EqualTo(new DateTime(2009, 1, 1)));
            Assert.That(info.DateTakenGranularity, Is.EqualTo(DateGranularity.Circa));
        });
        }

        [Test]
        public async Task PhotosGetInfoVideoTest()
        {
            const string videoId = "2926486605";

            var info = await Instance.PhotosGetInfoAsync(videoId);

            Assert.That(info, Is.Not.Null);
            Assert.That(info.PhotoId, Is.EqualTo(videoId));
        }

        [Test]
        public void TestPhotoNotFound()
        {
            Should.Throw<PhotoNotFoundException>(async () => await Instance.PhotosGetInfoAsync("abcd"));
        }

        [Test]
        public async Task ShouldReturnPhotoInfoWithGeoData()
        {
            var info = await Instance.PhotosGetInfoAsync("54071193");

            Assert.That(info, Is.Not.Null, "PhotoInfo should not be null.");
            Assert.That(info.Location, Is.Not.Null, "Location should not be null.");
            Assert.Multiple(() =>
        {
            Assert.That(info.Location.Longitude, Is.EqualTo(-180), "Longitude should be -180");
            Assert.That(info.Urls[0].Url, Is.EqualTo("https://www.flickr.com/photos/afdn/54071193/"));
        });
            Assert.That(info.GeoPermissions.IsPublic, Is.True, "GeoPermissions should be public.");
        }

        [Test]
        public async Task ShouldReturnPhotoInfoWithValidUrls()
        {
            var info = await Instance.PhotosGetInfoAsync("9671143400");

            Assert.That(await UrlHelper.Exists(info.Small320Url), Is.True, "Small320Url is not valid url : " + info.Small320Url);
            Assert.That(await UrlHelper.Exists(info.Medium640Url), Is.True, "Medium640Url is not valid url : " + info.Medium640Url);
            Assert.That(await UrlHelper.Exists(info.Medium800Url), Is.True, "Medium800Url is not valid url : " + info.Medium800Url);
            Assert.That(info.LargeUrl, Is.Not.EqualTo(info.SmallUrl), "URLs should all be different.");
        }

        [Test]
        [Ignore("Photo urls appear to have changed to start with 'live' so test is invalid")]
        public async Task PhotoInfoUrlsShouldMatchSizes()
        {
            var photos = await Instance.PhotosSearchAsync(new PhotoSearchOptions
            {
                UserId = TestData.TestUserId,
                PerPage = 1,
                Extras = PhotoSearchExtras.AllUrls
            });

            var photo = photos.First();

            var info = await Instance.PhotosGetInfoAsync(photo.PhotoId);
            Assert.Multiple(() =>
        {
            Assert.That(info.LargeUrl, Is.EqualTo(photo.LargeUrl));
            Assert.That(info.Small320Url, Is.EqualTo(photo.Small320Url));
        });
        }

        [Test]
        [TestCase("46611802@N00", "")]
        public async Task GetInfoWithInvalidXmlTests(string userId, string location)
        {
            var userInfo = await Instance.PeopleGetInfoAsync(userId);
            Assert.Multiple(() =>
        {
            Assert.That(userInfo.UserId, Is.EqualTo(userId));
            Assert.That(userInfo.Location, Is.EqualTo(location));
        });
        }
    }
}