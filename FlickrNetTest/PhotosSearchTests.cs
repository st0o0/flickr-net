﻿// ReSharper disable SuggestUseVarKeywordEvident
using FlickrNet;
using FlickrNet.CollectionModels;
using FlickrNet.Common;
using FlickrNet.Enums;
using FlickrNet.Exceptions;
using FlickrNet.Models;
using FlickrNet.SearchOptions;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    [TestFixture]
    public class PhotosSearchTests : BaseTest
    {
        [Test]
        public async Task PhotosSearchBasicSearch(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { Tags = "Test" };
            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsTrue(photos.Total > 0, "Total Photos should be greater than zero.");
            Assert.IsTrue(photos.Pages > 0, "Pages should be greaters than zero.");
            Assert.AreEqual(100, photos.PerPage, "PhotosPerPage should be 100.");
            Assert.AreEqual(1, photos.Page, "Page should be 1.");

            Assert.IsTrue(photos.Count > 0, "Photos.Count should be greater than 0.");
            Assert.AreEqual(photos.PerPage, photos.Count);
        }

        [Test]
        public async Task PhotosSearchSignedTest(CancellationToken cancellationToken = default)
        {
            Flickr f = TestData.GetSignedInstance();
            var o = new PhotoSearchOptions { Tags = "Test", PerPage = 5 };
            PhotoCollection photos = await f.PhotosSearchAsync(o, cancellationToken);

            Assert.AreEqual(5, photos.PerPage, "PerPage should equal 5.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosSearchFavorites(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { UserId = "me", Faves = true, Tags = "cat" };

            PhotoCollection p = await AuthInstance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsTrue(p.Count > 1, "Should have returned more than 1 result returned. Count = " + p.Count);
            Assert.IsTrue(p.Count < 100, "Should be less than 100 results returned. Count = " + p.Count);
        }

        [Test, Ignore("Currently 'Camera' searches are not working.")]
        [Category("AccessTokenRequired")]
        public async Task PhotosSearchCameraIphone(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Camera = "iPhone 5S",
                MinUploadDate = DateTime.Now.AddDays(-7),
                MaxUploadDate = DateTime.Now,
                Extras = PhotoSearchExtras.Tags
            };

            var ps = await AuthInstance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(ps);
            Assert.AreNotEqual(0, ps.Count);
        }

        [Test]
        public async Task PhotoSearchByPathAlias(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                GroupPathAlias = "api",
                PerPage = 10
            };

            var ps = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(ps);
            Assert.AreNotEqual(0, ps.Count);
        }

        [Test]
        public async Task PhotosSearchPerPage(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { PerPage = 10, Tags = "Test" };
            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsTrue(photos.Total > 0, "TotalPhotos should be greater than 0.");
            Assert.IsTrue(photos.Pages > 0, "TotalPages should be greater than 0.");
            Assert.AreEqual(10, photos.PerPage, "PhotosPerPage should be 10.");
            Assert.AreEqual(1, photos.Page, "PageNumber should be 1.");
            Assert.AreEqual(10, photos.Count, "Count should be 10.");
            Assert.AreEqual(photos.PerPage, photos.Count);
        }

        [Test]
        public async Task PhotosSearchUserIdTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { UserId = TestData.TestUserId };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (Photo photo in photos)
            {
                Assert.AreEqual(TestData.TestUserId, photo.UserId);
            }
        }

        [Test]
        public async Task PhotosSearchNoApiKey(CancellationToken cancellationToken = default)
        {
            Instance.ApiKey = "";
            Should.Throw<ApiKeyRequiredException>(async () => await Instance.PhotosSearchAsync(new PhotoSearchOptions(), cancellationToken));
        }

        [Test]
        public async Task GetOauthRequestTokenNoApiKey(CancellationToken cancellationToken)
        {
            Instance.ApiKey = "";
            Should.Throw<ApiKeyRequiredException>(async () => await Instance.OAuthGetRequestTokenAsync("oob", cancellationToken));
        }

        [Test]
        [Ignore("Flickr still doesn't seem to sort correctly by date posted.")]
        public async Task PhotosSearchSortDateTakenAscending(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DateTakenAscending,
                Extras = PhotoSearchExtras.DateTaken
            };

            var p = await Instance.PhotosSearchAsync(o, cancellationToken);

            for (var i = 1; i < p.Count; i++)
            {
                Assert.AreNotEqual(default(DateTime), p[i].DateTaken);
                Assert.IsTrue(p[i].DateTaken >= p[i - 1].DateTaken, "Date taken should increase");
            }
        }

        [Test]
        [Ignore("Flickr still doesn't seem to sort correctly by date posted.")]
        public async Task PhotosSearchSortDateTakenDescending(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DateTakenDescending,
                Extras = PhotoSearchExtras.DateTaken
            };

            var p = await Instance.PhotosSearchAsync(o, cancellationToken);

            for (var i = 1; i < p.Count; i++)
            {
                Assert.AreNotEqual(default(DateTime), p[i].DateTaken);
                Assert.IsTrue(p[i].DateTaken <= p[i - 1].DateTaken, "Date taken should decrease.");
            }
        }

        [Test]
        [Ignore("Flickr still doesn't seem to sort correctly by date posted.")]
        public async Task PhotosSearchSortDatePostedAscending(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DatePostedAscending,
                Extras = PhotoSearchExtras.DateUploaded
            };

            var p = await Instance.PhotosSearchAsync(o, cancellationToken);

            for (var i = 1; i < p.Count; i++)
            {
                Assert.AreNotEqual(default(DateTime), p[i].DateUploaded);
                Assert.IsTrue(p[i].DateUploaded >= p[i - 1].DateUploaded, "Date taken should increase.");
            }
        }

        [Test]
        [Ignore("Flickr still doesn't seem to sort correctly by date posted.")]
        public async Task PhotosSearchSortDataPostedDescending(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DatePostedDescending,
                Extras = PhotoSearchExtras.DateUploaded
            };

            var p = await Instance.PhotosSearchAsync(o, cancellationToken);

            for (int i = 1; i < p.Count; i++)
            {
                Assert.AreNotEqual(default(DateTime), p[i].DateUploaded);
                Assert.IsTrue(p[i].DateUploaded <= p[i - 1].DateUploaded, "Date taken should increase.");
            }
        }

        [Test]
        public async Task PhotosSearchGetLicenseNotNull(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DatePostedDescending,
                Extras = PhotoSearchExtras.License
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (Photo photo in photos)
            {
                Assert.IsNotNull(photo.License);
            }
        }

        [Test]
        public async Task PhotosSearchGetLicenseAttributionNoDerivs(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DatePostedDescending
            };
            o.Licenses.Add(LicenseType.AttributionNoDerivativesCC);
            o.Extras = PhotoSearchExtras.License;

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (Photo photo in photos)
            {
                Assert.AreEqual(LicenseType.AttributionNoDerivativesCC, photo.License);
            }
        }

        [Test]
        public async Task PhotosSearchGetMultipleLicenses(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                PerPage = 500,
                SortOrder = PhotoSearchSortOrder.DatePostedDescending
            };
            o.Licenses.Add(LicenseType.AttributionNoDerivativesCC);
            o.Licenses.Add(LicenseType.AttributionNoncommercialNoDerivativesCC);
            o.Extras = PhotoSearchExtras.License | PhotoSearchExtras.OwnerName;

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (Photo photo in photos)
            {
                if (photo.License != LicenseType.AttributionNoDerivativesCC &&
                    photo.License != LicenseType.AttributionNoncommercialNoDerivativesCC)
                {
                    Assert.Fail("License not one of selected licenses: " + photo.License.ToString());
                }
            }
        }

        [Test]
        public async Task PhotosSearchGetLicenseNoKnownCopright(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DatePostedDescending
            };
            o.Licenses.Add(LicenseType.NoKnownCopyrightRestrictions);
            o.Extras = PhotoSearchExtras.License;

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (Photo photo in photos)
            {
                Assert.AreEqual(LicenseType.NoKnownCopyrightRestrictions, photo.License);
            }
        }

        [Test]
        public async Task PhotosSearchSearchTwice(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { Tags = "microsoft", PerPage = 10 };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.AreEqual(10, photos.PerPage, "Per page is not 10");

            o.PerPage = 50;
            photos = await Instance.PhotosSearchAsync(o, cancellationToken);
            Assert.AreEqual(50, photos.PerPage, "Per page has not changed?");

            photos = await Instance.PhotosSearchAsync(o, cancellationToken);
            Assert.AreEqual(50, photos.PerPage, "Per page has changed!");
        }

        [Test]
        public async Task PhotosSearchPageTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { Tags = "colorful", PerPage = 10, Page = 3 };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.AreEqual(3, photos.Page);
        }

        [Test]
        public async Task PhotosSearchByColorCode(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                ColorCodes = new List<string> { "orange" },
                Tags = "colorful"
            };

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count, "Count should not be zero.");

            foreach (var photo in photos)
            {
                Console.WriteLine(photo.WebUrl);
            }
        }

        [TestCase(Style.BlackAndWhite)]
        [TestCase(Style.DepthOfField)]
        [TestCase(Style.Minimalism)]
        [TestCase(Style.Pattern)]
        public async Task PhotoSearchByStyles(Style style, CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Text = "nature",
                Page = 1,
                PerPage = 10,
                Styles = new[] { style }
            };

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.IsNotEmpty(photos);
        }

        [Test]
        public async Task PhotosSearchIsCommons(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                IsCommons = true,
                Tags = "newyork",
                PerPage = 10,
                Extras = PhotoSearchExtras.License
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (Photo photo in photos)
            {
                Assert.AreEqual(LicenseType.NoKnownCopyrightRestrictions, photo.License);
            }
        }

        [Test]
        public async Task PhotosSearchDateTakenGranualityTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                UserId = "8748614@N05",
                Tags = "primavera",
                PerPage = 500,
                Extras = PhotoSearchExtras.DateTaken
            };

            await Instance.PhotosSearchAsync(o, cancellationToken);
        }

        [Test]
        public async Task PhotosSearchDetailedTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "applestore",
                UserId = "41888973@N00",
                Extras = PhotoSearchExtras.All
            };
            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.AreEqual(100, photos.PerPage);
            Assert.AreEqual(1, photos.Page);

            Assert.AreNotEqual(0, photos.Count, "PhotoCollection.Count should not be zero.");

            Assert.AreEqual("3547139066", photos[0].PhotoId);
            Assert.AreEqual("41888973@N00", photos[0].UserId);
            Assert.AreEqual("bf4e11b1cb", photos[0].Secret);
            Assert.AreEqual("3311", photos[0].Server);
            Assert.AreEqual("Apple Store!", photos[0].Title);
            Assert.AreEqual("4", photos[0].Farm);
            Assert.AreEqual(false, photos[0].IsFamily);
            Assert.AreEqual(true, photos[0].IsPublic);
            Assert.AreEqual(false, photos[0].IsFriend);

            var dateTaken = new DateTime(2009, 5, 18, 4, 21, 46);
            var dateUploaded = new DateTime(2009, 5, 19, 21, 21, 46);
            Assert.IsTrue(photos[0].LastUpdated > dateTaken, "Last updated date was not correct.");
            Assert.AreEqual(dateTaken, photos[0].DateTaken, "Date taken date was not correct.");
            Assert.AreEqual(dateUploaded, photos[0].DateUploaded, "Date uploaded date was not correct.");

            Assert.AreEqual("jpg", photos[0].OriginalFormat, "OriginalFormat should be JPG");
            Assert.AreEqual("JjXZOYpUV7IbeGVOUQ", photos[0].PlaceId, "PlaceID not set correctly.");

            Assert.IsNotNull(photos[0].Description, "Description should not be null.");

            foreach (Photo photo in photos)
            {
                Assert.IsNotNull(photo.PhotoId);
                Assert.IsTrue(photo.IsPublic);
                Assert.IsFalse(photo.IsFamily);
                Assert.IsFalse(photo.IsFriend);
            }
        }

        [Test]
        public async Task PhotosSearchTagsTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { PerPage = 10, Tags = "test", Extras = PhotoSearchExtras.Tags };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            photos.Total.ShouldBeGreaterThan(0);
            photos.Pages.ShouldBeGreaterThan(0);
            photos.PerPage.ShouldBe(10);
            photos.Page.ShouldBe(1);
            photos.Count.ShouldBeInRange(9, 10, "Ideally should be 10, but sometimes returns 9");

            foreach (Photo photo in photos)
            {
                Assert.IsTrue(photo.Tags.Count > 0, "Should be some tags");
                Assert.IsTrue(photo.Tags.Contains("test"), "At least one should be 'test'");
            }
        }

        // Flickr sometimes returns different totals for the same search when a different perPage value is used.
        // As I have no control over this, and I am correctly setting the properties as returned I am ignoring this test.
        [Test]
        [Ignore("Flickr often returns different totals than requested.")]
        public async Task PhotosSearchPerPageMultipleTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { Tags = "microsoft" };
            o.Licenses.Add(LicenseType.AttributionCC);
            o.Licenses.Add(LicenseType.AttributionNoDerivativesCC);
            o.Licenses.Add(LicenseType.AttributionNoncommercialCC);
            o.Licenses.Add(LicenseType.AttributionNoncommercialNoDerivativesCC);
            o.Licenses.Add(LicenseType.AttributionNoncommercialShareAlikeCC);
            o.Licenses.Add(LicenseType.AttributionShareAlikeCC);

            o.MinUploadDate = DateTime.Today.AddDays(-4);
            o.MaxUploadDate = DateTime.Today.AddDays(-2);

            o.PerPage = 1;

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            int totalPhotos1 = photos.Total;

            o.PerPage = 10;

            photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            int totalPhotos2 = photos.Total;

            o.PerPage = 100;

            photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            int totalPhotos3 = photos.Total;

            Assert.AreEqual(totalPhotos1, totalPhotos2, "Total Photos 1 & 2 should be equal");
            Assert.AreEqual(totalPhotos2, totalPhotos3, "Total Photos 2 & 3 should be equal");
        }

        [Test]
        public async Task PhotosSearchPhotoSearchBoundaryBox(CancellationToken cancellationToken = default)
        {
            var b = new BoundaryBox(103.675997, 1.339811, 103.689456, 1.357764, GeoAccuracy.World);
            var o = new PhotoSearchOptions
            {
                HasGeo = true,
                BoundaryBox = b,
                Accuracy = b.Accuracy,
                MinUploadDate = DateTime.Now.AddYears(-1),
                MaxUploadDate = DateTime.Now,
                Extras = PhotoSearchExtras.Geo | PhotoSearchExtras.PathAlias,
                Tags = "colorful"
            };

            var ps = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (var p in ps)
            {
                // Annoying, but sometimes Flickr doesn't return the geo properties for a photo even for this type of search.
                if (Math.Abs(p.Latitude - 0) < 1e-8 && Math.Abs(p.Longitude - 0) < 1e-8) continue;

                Assert.IsTrue(p.Latitude > b.MinimumLatitude && b.MaximumLatitude > p.Latitude,
                              "Latitude is not within the boundary box. {0} outside {1} and {2} for photo {3}", p.Latitude, b.MinimumLatitude, b.MaximumLatitude, p.WebUrl);
                Assert.IsTrue(p.Longitude > b.MinimumLongitude && b.MaximumLongitude > p.Longitude,
                              "Longitude is not within the boundary box. {0} outside {1} and {2} for photo {3}", p.Longitude, b.MinimumLongitude, b.MaximumLongitude, p.WebUrl);
            }
        }

        [Test]
        public async Task PhotosSearchLatCultureTest(CancellationToken cancellationToken = default)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("nb-NO");

            var o = new PhotoSearchOptions { HasGeo = true };
            o.Extras |= PhotoSearchExtras.Geo;
            o.Tags = "colorful";
            o.TagMode = TagMode.AllTags;
            o.PerPage = 10;

            await Instance.PhotosSearchAsync(o, cancellationToken);
        }

        [Test]
        public async Task PhotosSearchTagCollectionTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                UserId = TestData.TestUserId,
                PerPage = 10,
                Extras = PhotoSearchExtras.Tags
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (Photo p in photos)
            {
                Assert.IsNotNull(p.Tags, "Tag Collection should not be null");
                Assert.IsTrue(p.Tags.Count > 0, "Should be more than one tag for all photos");
                Assert.IsNotNull(p.Tags[0]);
            }
        }

        [Test]
        public async Task PhotosSearchMultipleTagsTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "art,collection",
                TagMode = TagMode.AllTags,
                PerPage = 10,
                Extras = PhotoSearchExtras.Tags
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (Photo p in photos)
            {
                Assert.IsNotNull(p.Tags, "Tag Collection should not be null");
                Assert.IsTrue(p.Tags.Count > 0, "Should be more than one tag for all photos");
                Assert.IsTrue(p.Tags.Contains("art"), "Should contain 'art' tag.");
                Assert.IsTrue(p.Tags.Contains("collection"), "Should contain 'collection' tag.");
            }
        }

        [Test]
        public async Task PhotosSearchInterestingnessBasicTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                SortOrder = PhotoSearchSortOrder.InterestingnessDescending,
                Tags = "colorful",
                PerPage = 500
            };

            var ps = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(ps, "Photos should not be null");
            Assert.AreEqual(500, ps.PerPage, "PhotosPerPage should be 500");
            Assert.AreNotEqual(0, ps.Count, "Count should be greater than zero.");
        }

        [Test]
        public async Task PhotosSearchGroupIdTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions();
            o.GroupId = TestData.GroupId;
            o.PerPage = 10;

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count);

            foreach (var photo in photos)
            {
                Assert.IsNotNull(photo.PhotoId);
            }
        }

        [Test]
        [Ignore("GeoContext filter doesn't appear to be working.")]
        public async Task PhotosSearchGeoContext(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                HasGeo = true,
                GeoContext = GeoContext.Outdoors,
                Tags = "landscape"
            };

            o.Extras |= PhotoSearchExtras.Geo;

            var col = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (var p in col)
            {
                Assert.AreEqual(GeoContext.Outdoors, p.GeoContext);
            }
        }

        [Test]
        public async Task PhotosSearchLatLongGeoRadiusTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                HasGeo = true,
                MinTakenDate = DateTime.Today.AddYears(-3),
                PerPage = 10,
                Latitude = 61.600447,
                Longitude = 5.035064,
                Radius = 4.75f,
                RadiusUnits = RadiusUnit.Kilometers
            };
            o.Extras |= PhotoSearchExtras.Geo;

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count, "No photos returned by search.");

            foreach (var photo in photos)
            {
                Assert.IsNotNull(photo.PhotoId);
                Assert.AreNotEqual(0, photo.Longitude, "Longitude should not be zero.");
                Assert.AreNotEqual(0, photo.Latitude, "Latitude should not be zero.");
            }
        }

        [Test]
        public async Task PhotosSearchLargeRadiusTest(CancellationToken cancellationToken = default)
        {
            const double lat = 61.600447;
            const double lon = 5.035064;

            var o = new PhotoSearchOptions
            {
                PerPage = 100,
                HasGeo = true,
                MinTakenDate = DateTime.Today.AddYears(-3),
                Latitude = lat,
                Longitude = lon,
                Radius = 5.432123456f,
                RadiusUnits = RadiusUnit.Kilometers
            };
            o.Extras |= PhotoSearchExtras.Geo;

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count, "No photos returned by search.");

            foreach (var photo in photos)
            {
                Assert.IsNotNull(photo.PhotoId);
                Assert.AreNotEqual(0, photo.Longitude, "Longitude should not be zero.");
                Assert.AreNotEqual(0, photo.Latitude, "Latitude should not be zero.");

                LogOnError("Photo ID " + photo.PhotoId,
                           string.Format("Lat={0}, Long={1}", photo.Latitude, photo.Longitude));

                // Note: +/-1 is not an exact match to 5.4km, but anything outside of these bounds is definitely wrong.
                Assert.IsTrue(photo.Latitude > lat - 1 && photo.Latitude < lat + 1,
                              "Latitude not within acceptable range.");
                Assert.IsTrue(photo.Longitude > lon - 1 && photo.Longitude < lon + 1,
                              "Latitude not within acceptable range.");
            }
        }

        [Test]
        [Ignore("WOE ID searches don't appear to be working.")]
        public async Task PhotosSearchFullParamTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                UserId = TestData.TestUserId,
                Tags = "microsoft",
                TagMode = TagMode.AllTags,
                Text = "microsoft",
                MachineTagMode = MachineTagMode.AllTags,
                MachineTags = "dc:author=*",
                MinTakenDate = DateTime.Today.AddYears(-1),
                MaxTakenDate = DateTime.Today,
                PrivacyFilter = PrivacyFilter.PublicPhotos,
                SafeSearch = SafetyLevel.Safe,
                ContentType = ContentTypeSearch.PhotosOnly,
                HasGeo = false,
                WoeId = "30079",
                PlaceId = "X9sTR3BSUrqorQ"
            };

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreEqual(0, photos.Count);
        }

        [Test, Ignore("Not currently working for some reason.")]
        public async Task PhotosSearchGalleryPhotos(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { UserId = TestData.TestUserId, InGallery = true, Tags = "art" };
            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.AreEqual(1, photos.Count, "Only one photo should have been returned.");
        }

        [Test]
        public async Task PhotosSearchUrlLimitTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { Extras = PhotoSearchExtras.All, TagMode = TagMode.AnyTag };
            var sb = new StringBuilder();
            for (var i = 1; i < 200; i++) sb.Append("tagnumber" + i);
            o.Tags = sb.ToString();

            await Instance.PhotosSearchAsync(o, cancellationToken);
        }

        [Test]
        public async Task PhotosSearchRussianCharacters(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "снег"
            };

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.AreNotEqual(0, photos.Count, "Search should return some results.");
        }

        [Test]
        public async Task PhotosSearchRussianTagsReturned(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { PerPage = 200, Extras = PhotoSearchExtras.Tags, Tags = "фото" };

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            photos.Count.ShouldNotBe(0);
            photos.ShouldContain(p => p.Tags.Any(t => t == "фото"));
        }

        [Test]
        public async Task PhotosSearchRussianTextReturned(CancellationToken cancellationToken = default)
        {
            const string russian = "фото";

            var o = new PhotoSearchOptions { PerPage = 200, Extras = PhotoSearchExtras.Tags | PhotoSearchExtras.Description, Text = russian };

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            photos.Count.ShouldNotBe(0);
            photos.ShouldContain(p => p.Tags.Any(t => t == russian) || p.Title.Contains(russian) || p.Description.Contains(russian));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosSearchAuthRussianCharacters(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Tags = "снег"
            };

            var photos = await AuthInstance.PhotosSearchAsync(o, cancellationToken);

            Assert.AreNotEqual(0, photos.Count, "Search should return some results.");
        }

        [Test]
        public async Task PhotosSearchRotation(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.Rotation,
                UserId = TestData.TestUserId,
                PerPage = 100
            };
            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);
            foreach (var photo in photos)
            {
                Assert.IsTrue(photo.Rotation.HasValue, "Rotation should be set.");
                if (photo.PhotoId == "6861439677")
                    Assert.AreEqual(90, photo.Rotation, "Rotation should be 90 for this photo.");
                if (photo.PhotoId == "6790104907")
                    Assert.AreEqual(0, photo.Rotation, "Rotation should be 0 for this photo.");
            }
        }

        [Test]
        public async Task PhotosSearchLarge1600ImageSize(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.AllUrls,
                Tags = "colorful",
                MinUploadDate = DateTime.UtcNow.AddDays(-1)
            };

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos, "PhotosSearch should not return a null instance.");
            Assert.IsTrue(photos.Any(), "PhotoSearch should have returned some photos.");
            Assert.IsTrue(
                photos.Any(
                    p =>
                    !string.IsNullOrEmpty(p.Large1600Url) && p.Large1600Height.HasValue && p.Large1600Width.HasValue),
                "At least one photo should have a large1600 image url and height and width.");
        }

        [Test]
        public async Task PhotosSearchLarge2048ImageSize(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.Large2048Url,
                Tags = "colorful",
                MinUploadDate = DateTime.UtcNow.AddDays(-1)
            };

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos, "PhotosSearch should not return a null instance.");
            Assert.IsTrue(photos.Any(), "PhotoSearch should have returned some photos.");
            Assert.IsTrue(
                photos.Any(
                    p =>
                    !string.IsNullOrEmpty(p.Large2048Url) && p.Large2048Height.HasValue && p.Large2048Width.HasValue));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosSearchContactsPhotos(CancellationToken cancellationToken = default)
        {
            var contacts = (await AuthInstance.ContactsGetListAsync(1, 1000, cancellationToken)).Select(c => c.UserId).ToList();

            // Test with user id = "me"
            var o = new PhotoSearchOptions
            {
                UserId = "me",
                Contacts = ContactSearch.AllContacts,
                PerPage = 50
            };

            var photos = await AuthInstance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos, "PhotosSearch should not return a null instance.");
            Assert.IsTrue(photos.Any(), "PhotoSearch should have returned some photos.");
            Assert.IsTrue(photos.All(p => p.UserId != TestData.TestUserId), "None of the photos should be mine.");
            Assert.IsTrue(photos.All(p => contacts.Contains(p.UserId)), "UserID not found in list of contacts.");

            // Retest with user id specified explicitly
            o.UserId = TestData.TestUserId;
            photos = await AuthInstance.PhotosSearchAsync(o, cancellationToken);

            Assert.IsNotNull(photos, "PhotosSearch should not return a null instance.");
            Assert.IsTrue(photos.Any(), "PhotoSearch should have returned some photos.");
            Assert.IsTrue(photos.All(p => p.UserId != TestData.TestUserId), "None of the photos should be mine.");
            Assert.IsTrue(photos.All(p => contacts.Contains(p.UserId)), "UserID not found in list of contacts.");
        }

        [Test]
        public async Task SearchByUsername(CancellationToken cancellationToken = default)
        {
            var user = await Instance.PeopleFindByUserNameAsync("Jesus Solana", cancellationToken);

            var photos = await Instance.PhotosSearchAsync(new PhotoSearchOptions { Username = "Jesus Solana" }, cancellationToken);

            Assert.AreEqual(user.UserId, photos.First().UserId);
        }

        [Test]
        public async Task SearchByExifExposure(CancellationToken cancellationToken = default)
        {
            var options = new PhotoSearchOptions
            {
                ExifMinExposure = 10,
                ExifMaxExposure = 30,
                Extras = PhotoSearchExtras.PathAlias,
                PerPage = 5
            };

            var photos = await Instance.PhotosSearchAsync(options, cancellationToken);

            foreach (var photo in photos)
            {
                Console.WriteLine(photo.WebUrl);
            }
        }

        [Test]
        public async Task SearchByExifAperture(CancellationToken cancellationToken = default)
        {
            var options = new PhotoSearchOptions
            {
                ExifMinAperture = 0.0,
                ExifMaxAperture = 1 / 2,
                Extras = PhotoSearchExtras.PathAlias,
                PerPage = 5
            };

            var photos = await Instance.PhotosSearchAsync(options, cancellationToken);

            foreach (var photo in photos)
            {
                Console.WriteLine(photo.WebUrl);
            }
        }

        [Test]
        public async Task SearchByExifFocalLength(CancellationToken cancellationToken = default)
        {
            var options = new PhotoSearchOptions
            {
                ExifMinFocalLength = 400,
                ExifMaxFocalLength = 800,
                Extras = PhotoSearchExtras.PathAlias,
                PerPage = 5
            };

            var photos = await Instance.PhotosSearchAsync(options, cancellationToken);

            foreach (var photo in photos)
            {
                Console.WriteLine(photo.WebUrl);
            }
        }

        [Test]
        public async Task ExcludeUserTest(CancellationToken cancellationToken = default)
        {
            var options = new PhotoSearchOptions
            {
                Tags = "colorful",
                MinTakenDate = DateTime.Today.AddDays(-7),
                MaxTakenDate = DateTime.Today.AddDays(-1),
                PerPage = 10
            };

            var photos = await Instance.PhotosSearchAsync(options, cancellationToken);

            var firstUserId = photos.First().UserId;

            options.ExcludeUserID = firstUserId;

            var nextPhotos = await Instance.PhotosSearchAsync(options, cancellationToken);

            Assert.IsFalse(nextPhotos.Any(p => p.UserId == firstUserId), "Should not be any photos for user {0}", firstUserId);
        }

        [Test]
        public async Task GetPhotosByFoursquareVenueId(CancellationToken cancellationToken = default)
        {
            var venueid = "4ac518cef964a520f6a520e3";

            var options = new PhotoSearchOptions
            {
                FoursquareVenueID = venueid
            };

            var photos = await Instance.PhotosSearchAsync(options, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count, "Should have returned some photos for 'Big Ben'");
        }

        [Test]
        public async Task GetPhotosByFoursquareWoeId(CancellationToken cancellationToken = default)
        {
            // Seems to be the same as normal WOE IDs, so not sure what is different about the foursquare ones.
            var woeId = "44417";

            var options = new PhotoSearchOptions
            {
                FoursquareWoeID = woeId
            };

            var photos = await Instance.PhotosSearchAsync(options, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count, "Should have returned some photos for 'Big Ben'");
        }

        [Test]
        public async Task CountFavesAndCountComments(CancellationToken cancellationToken = default)
        {
            var options = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.CountFaves | PhotoSearchExtras.CountComments,
                Tags = "colorful"
            };

            var photos = await Instance.PhotosSearchAsync(options, cancellationToken);

            Assert.IsFalse(photos.Any(p => p.CountFaves == null), "Should not have any null CountFaves");
            Assert.IsFalse(photos.Any(p => p.CountComments == null), "Should not have any null CountComments");
        }

        [Test]
        public async Task ExcessiveTagsShouldNotThrowUriFormatException(CancellationToken cancellationToken = default)
        {
            var list = Enumerable.Range(1, 9000).Select(i => "reallybigtag" + i).ToList();
            var options = new PhotoSearchOptions
            {
                Tags = string.Join(",", list)
            };

            Should.Throw<FlickrApiException>(async () => await Instance.PhotosSearchAsync(options, cancellationToken));
        }
    }
}

// ReSharper restore SuggestUseVarKeywordEvident