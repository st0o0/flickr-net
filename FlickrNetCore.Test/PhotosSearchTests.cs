// ReSharper disable SuggestUseVarKeywordEvident
using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Common;
using FlickrNetCore.Enums;
using FlickrNetCore.Exceptions;
using FlickrNetCore.SearchOptions;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using Shouldly;
using System.Text;

namespace FlickrNetTest
{
    [TestFixture]
    public class PhotosSearchTests : BaseTest
    {
        [Test]
        public async Task PhotosSearchBasicSearch()
        {
            var o = new PhotoSearchOptions { Tags = "Test" };
            var photos = await Instance.PhotosSearchAsync(o);
            Assert.Multiple(() =>
            {
                Assert.That(photos.Total > 0, Is.True, "Total Photos should be greater than zero.");
                Assert.That(photos.Pages > 0, Is.True, "Pages should be greaters than zero.");
            });
            Assert.Multiple(() =>
            {
                Assert.That(photos.PerPage, Is.EqualTo(100), "PhotosPerPage should be 100.");
                Assert.That(photos.Page, Is.EqualTo(1), "Page should be 1.");
            });
            Assert.Multiple(() =>
            {
                Assert.That(photos.Count > 0, Is.True, "Photos.Count should be greater than 0.");
                Assert.That(photos, Has.Count.EqualTo(photos.PerPage));
            });
        }

        [Test]
        public async Task PhotosSearchSignedTest()
        {
            Flickr f = TestData.GetSignedInstance();
            var o = new PhotoSearchOptions { Tags = "Test", PerPage = 5 };
            PhotoCollection photos = await f.PhotosSearchAsync(o);

            Assert.That(photos.PerPage, Is.EqualTo(5), "PerPage should equal 5.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosSearchFavorites()
        {
            var o = new PhotoSearchOptions { UserId = "me", Faves = true, Tags = "cat" };

            PhotoCollection p = await AuthInstance.PhotosSearchAsync(o);
            Assert.Multiple(() =>
            {
                Assert.That(p.Count > 1, Is.True, "Should have returned more than 1 result returned. Count = " + p.Count);
                Assert.That(p.Count < 100, Is.True, "Should be less than 100 results returned. Count = " + p.Count);
            });
        }

        [Test, Ignore("Currently 'Camera' searches are not working.")]
        [Category("AccessTokenRequired")]
        public async Task PhotosSearchCameraIphone()
        {
            var o = new PhotoSearchOptions
            {
                Camera = "iPhone 5S",
                MinUploadDate = DateTime.Now.AddDays(-7),
                MaxUploadDate = DateTime.Now,
                Extras = PhotoSearchExtras.Tags
            };

            var ps = await AuthInstance.PhotosSearchAsync(o);

            Assert.That(ps, Is.Not.Null);
            Assert.That(ps, Is.Not.Empty);
        }

        [Test]
        public async Task PhotoSearchByPathAlias()
        {
            var o = new PhotoSearchOptions
            {
                GroupPathAlias = "api",
                PerPage = 10
            };

            var ps = await Instance.PhotosSearchAsync(o);

            Assert.That(ps, Is.Not.Null);
            Assert.That(ps, Is.Not.Empty);
        }

        [Test]
        public async Task PhotosSearchPerPage()
        {
            var o = new PhotoSearchOptions { PerPage = 10, Tags = "Test" };
            var photos = await Instance.PhotosSearchAsync(o);
            Assert.Multiple(() =>
            {
                Assert.That(photos.Total > 0, Is.True, "TotalPhotos should be greater than 0.");
                Assert.That(photos.Pages > 0, Is.True, "TotalPages should be greater than 0.");
            });
            Assert.Multiple(() =>
            {
                Assert.That(photos.PerPage, Is.EqualTo(10), "PhotosPerPage should be 10.");
                Assert.Multiple(() =>
            {
                Assert.That(photos.Page, Is.EqualTo(1), "PageNumber should be 1.");
                Assert.That(photos, Has.Count.EqualTo(10), "Count should be 10.");
            });
                Assert.That(photos, Has.Count.EqualTo(photos.PerPage));
            });
        }

        [Test]
        public async Task PhotosSearchUserIdTest()
        {
            var o = new PhotoSearchOptions { UserId = TestData.TestUserId };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            foreach (Photo photo in photos)
            {
                Assert.That(photo.UserId, Is.EqualTo(TestData.TestUserId));
            }
        }

        [Test]
        public void PhotosSearchNoApiKey()
        {
            Instance.ApiKey = "";
            Should.Throw<ApiKeyRequiredException>(async () => await Instance.PhotosSearchAsync(new PhotoSearchOptions()));
        }

        [Test]
        public void GetOauthRequestTokenNoApiKey()
        {
            Instance.ApiKey = "";
            Should.Throw<ApiKeyRequiredException>(async () => await Instance.OAuthGetRequestTokenAsync("oob"));
        }

        [Test]
        [Ignore("Flickr still doesn't seem to sort correctly by date posted.")]
        public async Task PhotosSearchSortDateTakenAscending()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DateTakenAscending,
                Extras = PhotoSearchExtras.DateTaken
            };

            var p = await Instance.PhotosSearchAsync(o);

            for (var i = 1; i < p.Count; i++)
            {
                Assert.That(p[i].DateTaken, Is.Not.EqualTo(default(DateTime)));
                Assert.That(p[i].DateTaken >= p[i - 1].DateTaken, Is.True, "Date taken should increase");
            }
        }

        [Test]
        [Ignore("Flickr still doesn't seem to sort correctly by date posted.")]
        public async Task PhotosSearchSortDateTakenDescending()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DateTakenDescending,
                Extras = PhotoSearchExtras.DateTaken
            };

            var p = await Instance.PhotosSearchAsync(o);

            for (var i = 1; i < p.Count; i++)
            {
                Assert.That(p[i].DateTaken, Is.Not.EqualTo(default(DateTime)));
                Assert.That(p[i].DateTaken <= p[i - 1].DateTaken, Is.True, "Date taken should decrease.");
            }
        }

        [Test]
        [Ignore("Flickr still doesn't seem to sort correctly by date posted.")]
        public async Task PhotosSearchSortDatePostedAscending()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DatePostedAscending,
                Extras = PhotoSearchExtras.DateUploaded
            };

            var p = await Instance.PhotosSearchAsync(o);

            for (var i = 1; i < p.Count; i++)
            {
                Assert.That(p[i].DateUploaded, Is.Not.EqualTo(default(DateTime)));
                Assert.That(p[i].DateUploaded >= p[i - 1].DateUploaded, Is.True, "Date taken should increase.");
            }
        }

        [Test]
        [Ignore("Flickr still doesn't seem to sort correctly by date posted.")]
        public async Task PhotosSearchSortDataPostedDescending()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DatePostedDescending,
                Extras = PhotoSearchExtras.DateUploaded
            };

            var p = await Instance.PhotosSearchAsync(o);

            for (int i = 1; i < p.Count; i++)
            {
                Assert.That(p[i].DateUploaded, Is.Not.EqualTo(default(DateTime)));
                Assert.That(p[i].DateUploaded <= p[i - 1].DateUploaded, Is.True, "Date taken should increase.");
            }
        }

        [Test]
        public async Task PhotosSearchGetLicenseNotNull()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DatePostedDescending,
                Extras = PhotoSearchExtras.License
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            foreach (Photo photo in photos)
            {
                Assert.That(photo, Is.Not.Null);
            }
        }

        [Test]
        public async Task PhotosSearchGetLicenseAttributionNoDerivs()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DatePostedDescending
            };
            o.Licenses.Add(LicenseType.AttributionNoDerivativesCC);
            o.Extras = PhotoSearchExtras.License;

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            foreach (Photo photo in photos)
            {
                Assert.That(photo.License, Is.EqualTo(LicenseType.AttributionNoDerivativesCC));
            }
        }

        [Test]
        public async Task PhotosSearchGetMultipleLicenses()
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

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

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
        public async Task PhotosSearchGetLicenseNoKnownCopright()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "microsoft",
                SortOrder = PhotoSearchSortOrder.DatePostedDescending
            };
            o.Licenses.Add(LicenseType.NoKnownCopyrightRestrictions);
            o.Extras = PhotoSearchExtras.License;

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            foreach (Photo photo in photos)
            {
                Assert.That(photo.License, Is.EqualTo(LicenseType.NoKnownCopyrightRestrictions));
            }
        }

        [Test]
        public async Task PhotosSearchSearchTwice()
        {
            var o = new PhotoSearchOptions { Tags = "microsoft", PerPage = 10 };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos.PerPage, Is.EqualTo(10), "Per page is not 10");

            o.PerPage = 50;
            photos = await Instance.PhotosSearchAsync(o);
            Assert.That(photos.PerPage, Is.EqualTo(50), "Per page has not changed?");

            photos = await Instance.PhotosSearchAsync(o);
            Assert.That(photos.PerPage, Is.EqualTo(50), "Per page has changed!");
        }

        [Test]
        public async Task PhotosSearchPageTest()
        {
            var o = new PhotoSearchOptions { Tags = "colorful", PerPage = 10, Page = 3 };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos.Page, Is.EqualTo(3));
        }

        [Test]
        public async Task PhotosSearchByColorCode()
        {
            var o = new PhotoSearchOptions
            {
                ColorCodes = new List<string> { "orange" },
                Tags = "colorful"
            };

            var photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "Count should not be zero.");

            foreach (var photo in photos)
            {
                Console.WriteLine(photo.WebUrl);
            }
        }

        [TestCase(Style.BlackAndWhite)]
        [TestCase(Style.DepthOfField)]
        [TestCase(Style.Minimalism)]
        [TestCase(Style.Pattern)]
        public async Task PhotoSearchByStyles(Style style)
        {
            var o = new PhotoSearchOptions
            {
                Text = "nature",
                Page = 1,
                PerPage = 10,
                Styles = new[] { style }
            };

            var photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Null);
            Assert.IsNotEmpty(photos);
        }

        [Test]
        public async Task PhotosSearchIsCommons()
        {
            var o = new PhotoSearchOptions
            {
                IsCommons = true,
                Tags = "newyork",
                PerPage = 10,
                Extras = PhotoSearchExtras.License
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            foreach (Photo photo in photos)
            {
                Assert.That(photo.License, Is.EqualTo(LicenseType.NoKnownCopyrightRestrictions));
            }
        }

        [Test]
        public async Task PhotosSearchDateTakenGranualityTest()
        {
            var o = new PhotoSearchOptions
            {
                UserId = "8748614@N05",
                Tags = "primavera",
                PerPage = 500,
                Extras = PhotoSearchExtras.DateTaken
            };

            await Instance.PhotosSearchAsync(o);
        }

        [Test]
        public async Task PhotosSearchDetailedTest()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "applestore",
                UserId = "41888973@N00",
                Extras = PhotoSearchExtras.All
            };
            PhotoCollection photos = await Instance.PhotosSearchAsync(o);
            Assert.Multiple(() =>
            {
                Assert.That(photos.PerPage, Is.EqualTo(100));
                Assert.Multiple(() =>
            {
                Assert.That(photos.Page, Is.EqualTo(1));

                Assert.That(photos, Is.Not.Empty, "PhotoCollection.Count should not be zero.");
            });
                Assert.That(photos[0].PhotoId, Is.EqualTo("3547139066"));
                Assert.Multiple(() =>
                {
                    Assert.That(photos[0].UserId, Is.EqualTo("41888973@N00"));
                    Assert.Multiple(() =>
                    {
                        Assert.That(photos[0].Secret, Is.EqualTo("bf4e11b1cb"));
                        Assert.That(photos[0].Server, Is.EqualTo("3311"));
                        Assert.That(photos[0].Title, Is.EqualTo("Apple Store!"));
                        Assert.That(photos[0].Farm, Is.EqualTo("4"));
                        Assert.That(photos[0].IsFamily, Is.EqualTo(false));
                        Assert.That(photos[0].IsPublic, Is.EqualTo(true));
                        Assert.That(photos[0].IsFriend, Is.EqualTo(false));
                    });
                    var dateTaken = new DateTime(2009, 5, 18, 4, 21, 46);
                    var dateUploaded = new DateTime(2009, 5, 19, 21, 21, 46);
                    Assert.That(photos[0].LastUpdated > dateTaken, Is.True, "Last updated date was not correct.");
                    Assert.Multiple(() =>
                        {
                            Assert.That(photos[0].DateTaken, Is.EqualTo(dateTaken), "Date taken date was not correct.");
                            Assert.Multiple(() =>
                        {
                            Assert.That(photos[0].DateUploaded, Is.EqualTo(dateUploaded), "Date uploaded date was not correct.");

                            Assert.That(photos[0].OriginalFormat, Is.EqualTo("jpg"), "OriginalFormat should be JPG");
                            Assert.That(photos[0].PlaceId, Is.EqualTo("JjXZOYpUV7IbeGVOUQ"), "PlaceID not set correctly.");

                            Assert.That(photos[0].Description, Is.Not.Null, "Description should not be null.");
                        });
                            foreach (Photo photo in photos)
                            {
                                Assert.That(photo.PhotoId, Is.Not.Null);
                                Assert.That(photo.IsPublic, Is.True);
                                Assert.That(photo.IsFamily, Is.False);
                                Assert.That(photo.IsFriend, Is.False);
                            }
                        });
                });
            });
        }

        [Test]
        public async Task PhotosSearchTagsTest()
        {
            var o = new PhotoSearchOptions { PerPage = 10, Tags = "test", Extras = PhotoSearchExtras.Tags };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            photos.Total.ShouldBeGreaterThan(0);
            photos.Pages.ShouldBeGreaterThan(0);
            photos.PerPage.ShouldBe(10);
            photos.Page.ShouldBe(1);
            photos.Count.ShouldBeInRange(9, 10, "Ideally should be 10, but sometimes returns 9");

            foreach (Photo photo in photos)
            {
                Assert.That(photo.Tags.Count > 0, Is.True, "Should be some tags");
                Assert.That(photo.Tags.Contains("test"), Is.True, "At least one should be 'test'");
            }
        }

        // Flickr sometimes returns different totals for the same search when a different perPage value is used.
        // As I have no control over this, and I am correctly setting the properties as returned I am ignoring this test.
        [Test]
        [Ignore("Flickr often returns different totals than requested.")]
        public async Task PhotosSearchPerPageMultipleTest()
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

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            int totalPhotos1 = photos.Total;

            o.PerPage = 10;

            photos = await Instance.PhotosSearchAsync(o);

            int totalPhotos2 = photos.Total;

            o.PerPage = 100;

            photos = await Instance.PhotosSearchAsync(o);

            int totalPhotos3 = photos.Total;
            Assert.Multiple(() =>
            {
                Assert.That(totalPhotos2, Is.EqualTo(totalPhotos1), "Total Photos 1 & 2 should be equal");
                Assert.That(totalPhotos3, Is.EqualTo(totalPhotos2), "Total Photos 2 & 3 should be equal");
            });
        }

        [Test]
        public async Task PhotosSearchPhotoSearchBoundaryBox()
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

            var ps = await Instance.PhotosSearchAsync(o);

            foreach (var p in ps)
            {
                // Annoying, but sometimes Flickr doesn't return the geo properties for a photo even for this type of search.
                if (Math.Abs(p.Latitude - 0) < 1e-8 && Math.Abs(p.Longitude - 0) < 1e-8) continue;

                Assert.That(p.Latitude > b.MinimumLatitude && b.MaximumLatitude > p.Latitude, Is.True,
                              "Latitude is not within the boundary box. {0} outside {1} and {2} for photo {3}", p.Latitude, b.MinimumLatitude, b.MaximumLatitude, p.WebUrl);
                Assert.That(p.Longitude > b.MinimumLongitude && b.MaximumLongitude > p.Longitude, Is.True,
                              "Longitude is not within the boundary box. {0} outside {1} and {2} for photo {3}", p.Longitude, b.MinimumLongitude, b.MaximumLongitude, p.WebUrl);
            }
        }

        [Test]
        public async Task PhotosSearchLatCultureTest()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("nb-NO");

            var o = new PhotoSearchOptions { HasGeo = true };
            o.Extras |= PhotoSearchExtras.Geo;
            o.Tags = "colorful";
            o.TagMode = TagMode.AllTags;
            o.PerPage = 10;

            await Instance.PhotosSearchAsync(o);
        }

        [Test]
        public async Task PhotosSearchTagCollectionTest()
        {
            var o = new PhotoSearchOptions
            {
                UserId = TestData.TestUserId,
                PerPage = 10,
                Extras = PhotoSearchExtras.Tags
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            foreach (Photo p in photos)
            {
                Assert.That(p.Tags, Is.Not.Null, "Tag Collection should not be null");
                Assert.That(p.Tags.Count > 0, Is.True, "Should be more than one tag for all photos");
                Assert.That(p.Tags[0], Is.Not.Null);
            }
        }

        [Test]
        public async Task PhotosSearchMultipleTagsTest()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "art,collection",
                TagMode = TagMode.AllTags,
                PerPage = 10,
                Extras = PhotoSearchExtras.Tags
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            foreach (Photo p in photos)
            {
                Assert.That(p.Tags, Is.Not.Null, "Tag Collection should not be null");
                Assert.That(p.Tags.Count > 0, Is.True, "Should be more than one tag for all photos");
                Assert.That(p.Tags.Contains("art"), Is.True, "Should contain 'art' tag.");
                Assert.That(p.Tags.Contains("collection"), Is.True, "Should contain 'collection' tag.");
            }
        }

        [Test]
        public async Task PhotosSearchInterestingnessBasicTest()
        {
            var o = new PhotoSearchOptions
            {
                SortOrder = PhotoSearchSortOrder.InterestingnessDescending,
                Tags = "colorful",
                PerPage = 500
            };

            var ps = await Instance.PhotosSearchAsync(o);

            Assert.That(ps, Is.Not.Null, "Photos should not be null");
            Assert.Multiple(() =>
            {
                Assert.That(ps.PerPage, Is.EqualTo(500), "PhotosPerPage should be 500");
                Assert.That(ps, Is.Not.Empty, "Count should be greater than zero.");
            });
        }

        [Test]
        public async Task PhotosSearchGroupIdTest()
        {
            var o = new PhotoSearchOptions();
            o.GroupId = TestData.GroupId;
            o.PerPage = 10;

            var photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty);

            foreach (var photo in photos)
            {
                Assert.That(photo.PhotoId, Is.Not.Null);
            }
        }

        [Test]
        [Ignore("GeoContext filter doesn't appear to be working.")]
        public async Task PhotosSearchGeoContext()
        {
            var o = new PhotoSearchOptions
            {
                HasGeo = true,
                GeoContext = GeoContext.Outdoors,
                Tags = "landscape"
            };

            o.Extras |= PhotoSearchExtras.Geo;

            var col = await Instance.PhotosSearchAsync(o);

            foreach (var p in col)
            {
                Assert.That(p.GeoContext, Is.EqualTo(GeoContext.Outdoors));
            }
        }

        [Test]
        public async Task PhotosSearchLatLongGeoRadiusTest()
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

            var photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "No photos returned by search.");

            foreach (var photo in photos)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(photo.PhotoId, Is.Not.Null);
                    Assert.Multiple(() =>
                {
                    Assert.That(photo.Longitude, Is.Not.EqualTo(0), "Longitude should not be zero.");
                    Assert.That(photo.Latitude, Is.Not.EqualTo(0), "Latitude should not be zero.");
                });
                });
            }
        }

        [Test]
        public async Task PhotosSearchLargeRadiusTest()
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

            var photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "No photos returned by search.");

            foreach (var photo in photos)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(photo.PhotoId, Is.Not.Null);
                    Assert.Multiple(() =>
                {
                    Assert.That(photo.Longitude, Is.Not.EqualTo(0), "Longitude should not be zero.");
                    Assert.That(photo.Latitude, Is.Not.EqualTo(0), "Latitude should not be zero.");
                });
                    LogOnError("Photo ID " + photo.PhotoId,
                   string.Format("Lat={0}, Long={1}", photo.Latitude, photo.Longitude));

                    // Note: +/-1 is not an exact match to 5.4km, but anything outside of these bounds is definitely wrong.
                    Assert.That(photo.Latitude > lat - 1 && photo.Latitude < lat + 1, Is.True,
                      "Latitude not within acceptable range.");
                    Assert.That(photo.Longitude > lon - 1 && photo.Longitude < lon + 1, Is.True,
                      "Latitude not within acceptable range.");
                });
            }
        }

        [Test]
        [Ignore("WOE ID searches don't appear to be working.")]
        public async Task PhotosSearchFullParamTest()
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

            var photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos.Count, Is.EqualTo(0));
        }

        [Test, Ignore("Not currently working for some reason.")]
        public async Task PhotosSearchGalleryPhotos()
        {
            var o = new PhotoSearchOptions { UserId = TestData.TestUserId, InGallery = true, Tags = "art" };
            var photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos, Has.Count.EqualTo(1), "Only one photo should have been returned.");
        }

        [Test]
        public async Task PhotosSearchUrlLimitTest()
        {
            var o = new PhotoSearchOptions { Extras = PhotoSearchExtras.All, TagMode = TagMode.AnyTag };
            var sb = new StringBuilder();
            for (var i = 1; i < 200; i++) sb.Append("tagnumber" + i);
            o.Tags = sb.ToString();

            await Instance.PhotosSearchAsync(o);
        }

        [Test]
        public async Task PhotosSearchRussianCharacters()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "снег"
            };

            var photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Empty, "Search should return some results.");
        }

        [Test]
        public async Task PhotosSearchRussianTagsReturned()
        {
            var o = new PhotoSearchOptions { PerPage = 200, Extras = PhotoSearchExtras.Tags, Tags = "фото" };

            var photos = await Instance.PhotosSearchAsync(o);

            photos.Count.ShouldNotBe(0);
            photos.ShouldContain(p => p.Tags.Any(t => t == "фото"));
        }

        [Test]
        public async Task PhotosSearchRussianTextReturned()
        {
            const string russian = "фото";

            var o = new PhotoSearchOptions { PerPage = 200, Extras = PhotoSearchExtras.Tags | PhotoSearchExtras.Description, Text = russian };

            var photos = await Instance.PhotosSearchAsync(o);

            photos.Count.ShouldNotBe(0);
            photos.ShouldContain(p => p.Tags.Any(t => t == russian) || p.Title.Contains(russian) || p.Description.Contains(russian));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosSearchAuthRussianCharacters()
        {
            var o = new PhotoSearchOptions
            {
                Tags = "снег"
            };

            var photos = await AuthInstance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Empty, "Search should return some results.");
        }

        [Test]
        public async Task PhotosSearchRotation()
        {
            var o = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.Rotation,
                UserId = TestData.TestUserId,
                PerPage = 100
            };
            var photos = await Instance.PhotosSearchAsync(o);
            foreach (var photo in photos)
            {
                Assert.That(photo.Rotation.HasValue, Is.True, "Rotation should be set.");
                if (photo.PhotoId == "6861439677")
                    Assert.That(photo.Rotation, Is.EqualTo(90), "Rotation should be 90 for this photo.");
                if (photo.PhotoId == "6790104907")
                    Assert.That(photo.Rotation, Is.EqualTo(0), "Rotation should be 0 for this photo.");
            }
        }

        [Test]
        public async Task PhotosSearchLarge1600ImageSize()
        {
            var o = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.AllUrls,
                Tags = "colorful",
                MinUploadDate = DateTime.UtcNow.AddDays(-1)
            };

            var photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Null, "PhotosSearch should not return a null instance.");
            Assert.That(photos.Any(), Is.True, "PhotoSearch should have returned some photos.");
            Assert.That(
                photos.Any(
                    p =>
                    !string.IsNullOrEmpty(p.Large1600Url) && p.Large1600Height.HasValue && p.Large1600Width.HasValue), Is.True,
                "At least one photo should have a large1600 image url and height and width.");
        }

        [Test]
        public async Task PhotosSearchLarge2048ImageSize()
        {
            var o = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.Large2048Url,
                Tags = "colorful",
                MinUploadDate = DateTime.UtcNow.AddDays(-1)
            };

            var photos = await Instance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Null, "PhotosSearch should not return a null instance.");
            Assert.That(photos.Any(), Is.True, "PhotoSearch should have returned some photos.");
            Assert.That(
                photos.Any(
                    p =>
                    !string.IsNullOrEmpty(p.Large2048Url) && p.Large2048Height.HasValue && p.Large2048Width.HasValue), Is.True);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosSearchContactsPhotos()
        {
            var contacts = (await AuthInstance.ContactsGetListAsync(1, 1000)).Select(c => c.UserId).ToList();

            // Test with user id = "me"
            var o = new PhotoSearchOptions
            {
                UserId = "me",
                Contacts = ContactSearch.AllContacts,
                PerPage = 50
            };

            var photos = await AuthInstance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Null, "PhotosSearch should not return a null instance.");
            Assert.That(photos.Any(), Is.True, "PhotoSearch should have returned some photos.");
            Assert.That(photos.All(p => p.UserId != TestData.TestUserId), Is.True, "None of the photos should be mine.");
            Assert.That(photos.All(p => contacts.Contains(p.UserId)), Is.True, "UserID not found in list of contacts.");

            // Retest with user id specified explicitly
            o.UserId = TestData.TestUserId;
            photos = await AuthInstance.PhotosSearchAsync(o);

            Assert.That(photos, Is.Not.Null, "PhotosSearch should not return a null instance.");
            Assert.That(photos.Any(), Is.True, "PhotoSearch should have returned some photos.");
            Assert.That(photos.All(p => p.UserId != TestData.TestUserId), Is.True, "None of the photos should be mine.");
            Assert.That(photos.All(p => contacts.Contains(p.UserId)), Is.True, "UserID not found in list of contacts.");
        }

        [Test]
        public async Task SearchByUsername()
        {
            var user = await Instance.PeopleFindByUserNameAsync("Jesus Solana");

            var photos = await Instance.PhotosSearchAsync(new PhotoSearchOptions { Username = "Jesus Solana" });

            Assert.That(photos.First().UserId, Is.EqualTo(user.UserId));
        }

        [Test]
        public async Task SearchByExifExposure()
        {
            var options = new PhotoSearchOptions
            {
                ExifMinExposure = 10,
                ExifMaxExposure = 30,
                Extras = PhotoSearchExtras.PathAlias,
                PerPage = 5
            };

            var photos = await Instance.PhotosSearchAsync(options);

            foreach (var photo in photos)
            {
                Console.WriteLine(photo.WebUrl);
            }
        }

        [Test]
        public async Task SearchByExifAperture()
        {
            var options = new PhotoSearchOptions
            {
                ExifMinAperture = 0.0,
                ExifMaxAperture = 1 / 2,
                Extras = PhotoSearchExtras.PathAlias,
                PerPage = 5
            };

            var photos = await Instance.PhotosSearchAsync(options);

            foreach (var photo in photos)
            {
                Console.WriteLine(photo.WebUrl);
            }
        }

        [Test]
        public async Task SearchByExifFocalLength()
        {
            var options = new PhotoSearchOptions
            {
                ExifMinFocalLength = 400,
                ExifMaxFocalLength = 800,
                Extras = PhotoSearchExtras.PathAlias,
                PerPage = 5
            };

            var photos = await Instance.PhotosSearchAsync(options);

            foreach (var photo in photos)
            {
                Console.WriteLine(photo.WebUrl);
            }
        }

        [Test]
        public async Task ExcludeUserTest()
        {
            var options = new PhotoSearchOptions
            {
                Tags = "colorful",
                MinTakenDate = DateTime.Today.AddDays(-7),
                MaxTakenDate = DateTime.Today.AddDays(-1),
                PerPage = 10
            };

            var photos = await Instance.PhotosSearchAsync(options);

            var firstUserId = photos.First().UserId;

            options.ExcludeUserID = firstUserId;

            var nextPhotos = await Instance.PhotosSearchAsync(options);

            Assert.That(nextPhotos.Any(p => p.UserId == firstUserId), Is.False, "Should not be any photos for user {0}", firstUserId);
        }

        [Test]
        public async Task GetPhotosByFoursquareVenueId()
        {
            var venueid = "4ac518cef964a520f6a520e3";

            var options = new PhotoSearchOptions
            {
                FoursquareVenueID = venueid
            };

            var photos = await Instance.PhotosSearchAsync(options);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "Should have returned some photos for 'Big Ben'");
        }

        [Test]
        public async Task GetPhotosByFoursquareWoeId()
        {
            // Seems to be the same as normal WOE IDs, so not sure what is different about the foursquare ones.
            var woeId = "44417";

            var options = new PhotoSearchOptions
            {
                FoursquareWoeID = woeId
            };

            var photos = await Instance.PhotosSearchAsync(options);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "Should have returned some photos for 'Big Ben'");
        }

        [Test]
        public async Task CountFavesAndCountComments()
        {
            var options = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.CountFaves | PhotoSearchExtras.CountComments,
                Tags = "colorful"
            };

            var photos = await Instance.PhotosSearchAsync(options);

            Assert.That(photos.Any(p => p.CountFaves == null), Is.False, "Should not have any null CountFaves");
            Assert.That(photos.Any(p => p.CountComments == null), Is.False, "Should not have any null CountComments");
        }

        [Test]
        public void ExcessiveTagsShouldNotThrowUriFormatException()
        {
            var list = Enumerable.Range(1, 9000).Select(i => "reallybigtag" + i).ToList();
            var options = new PhotoSearchOptions
            {
                Tags = string.Join(",", list)
            };

            Should.Throw<FlickrApiException>(async () => await Instance.PhotosSearchAsync(options));
        }
    }
}

// ReSharper restore SuggestUseVarKeywordEvident