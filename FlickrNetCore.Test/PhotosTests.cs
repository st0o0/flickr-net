using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.SearchOptions;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using Shouldly;
using System.Net;

namespace FlickrNetTest
{
    [TestFixture]
    public class PhotosTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosSetDatesTest()
        {
            var f = AuthInstance;
            var photoId = TestData.PhotoId;

            var info = await f.PhotosGetInfoAsync(photoId);

            await f.PhotosSetDatesAsync(photoId, info.DatePosted, info.DateTaken, info.DateTakenGranularity);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosAddTagsTest()
        {
            Flickr f = AuthInstance;
            string testtag = "thisisatesttag";
            string photoId = "6282363572";

            // Add the tag
            await f.PhotosAddTagsAsync(photoId, new[] { testtag });
            // Add second tag using different signature
            await f.PhotosAddTagsAsync(photoId, new string[] { testtag + "2" });

            // Get list of tags
            var tags = await f.TagsGetListPhotoAsync(photoId);

            // Find the tag in the collection
            var tagsToRemove = tags.Where(t => t.TagText.StartsWith(testtag, StringComparison.Ordinal));

            foreach (var tag in tagsToRemove)
            {
                // Remove the tag
                await f.PhotosRemoveTagAsync(tag.TagId);
            }
        }

        [Test]
        public async Task PhotosGetAllContextsBasicTest()
        {
            var a = await Instance.PhotosGetAllContextsAsync("4114887196");

            Assert.That(a, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(a.Groups, Is.Not.Null, "Groups should not be null.");
                Assert.That(a.Sets, Is.Not.Null, "Sets should not be null.");
            });
            Assert.Multiple(() =>
            {
                Assert.That(a.Groups, Has.Count.EqualTo(1), "Groups.Count should be one.");
                Assert.That(a.Sets, Has.Count.EqualTo(1), "Sets.Count should be one.");
            });
        }

        [Test]
        public async Task PhotosGetPopular()
        {
            var photos = await Instance.PhotosGetPopularAsync(TestData.TestUserId);

            photos.ShouldNotBeEmpty();
        }

        [Test]
        public async Task PhotosGetExifTest()
        {
            Flickr f = Instance;

            ExifTagCollection tags = await f.PhotosGetExifAsync("4268023123");

            Console.WriteLine(f.LastResponse);

            Assert.That(tags, Is.Not.Null, "ExifTagCollection should not be null.");

            Assert.That(tags.Count > 20, Is.True, "More than twenty parts of EXIF data should be returned.");
            Assert.Multiple(() =>
            {
                Assert.That(tags[0].TagSpace, Is.EqualTo("IFD0"), "First tags TagSpace is not set correctly.");
                Assert.Multiple(() =>
                {
                    Assert.That(tags[0].TagSpaceId, Is.EqualTo(0), "First tags TagSpaceId is not set correctly.");
                    Assert.That(tags[0].Tag, Is.EqualTo("Compression"), "First tags Tag is not set correctly.");
                    Assert.That(tags[0].Label, Is.EqualTo("Compression"), "First tags Label is not set correctly.");
                    Assert.That(tags[0].Raw, Is.EqualTo("JPEG (old-style)"), "First tags RAW is not correct.");
                });
                Assert.That(tags[0].Clean, Is.Null, "First tags Clean should be null.");
            });
        }

        [Test]
        public async Task PhotosGetContextBasicTest()
        {
            var context = await Instance.PhotosGetContextAsync("3845365350");

            Assert.That(context, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(context.PreviousPhoto.PhotoId, Is.EqualTo("3844573707"));
                Assert.That(context.NextPhoto.PhotoId, Is.EqualTo("3992605178"));
            });
        }

        [Test]
        public async Task PhotosGetExifIPhoneTest()
        {
            bool bFound = false;
            Flickr f = Instance;

            ExifTagCollection tags = await f.PhotosGetExifAsync("5899928191");

            Assert.That(tags.Camera, Is.EqualTo("Apple iPhone 4"), "Camera property should be set correctly.");

            foreach (ExifTag tag in tags)
            {
                if (tag.Tag == "Model")
                {
                    Assert.That(tag.Raw == "iPhone 4", Is.True, "Model tag is not 'iPhone 4'");
                    bFound = true;
                    break;
                }
            }
            Assert.That(bFound, Is.True, "Model tag not found.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetNotInSetAllParamsTest()
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetNotInSetAsync(1, 10, PhotoSearchExtras.All);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Has.Count.EqualTo(10));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetNotInSetNoParamsTest()
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetNotInSetAsync();
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetNotInSetPagesTest()
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetNotInSetAsync(1, 11);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Has.Count.EqualTo(11));

            // Overloads
            await f.PhotosGetNotInSetAsync();
            await f.PhotosGetNotInSetAsync(1);
            await f.PhotosGetNotInSetAsync(new PartialSearchOptions() { Page = 1, PerPage = 10, PrivacyFilter = PrivacyFilter.CompletelyPrivate });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetPermsBasicTest()
        {
            var p = await AuthInstance.PhotosGetPermsAsync("4114887196");

            Assert.That(p, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(p.PhotoId, Is.EqualTo("4114887196"));
                Assert.That(p.PermissionComment, Is.Not.EqualTo(PermissionComment.Nobody));
            });
        }

        [Test]
        public async Task PhotosGetRecentBlankTest()
        {
            var photos = await Instance.PhotosGetRecentAsync();

            Assert.That(photos, Is.Not.Null);
        }

        [Test]
        public async Task PhotosGetRecentAllParamsTest()
        {
            var photos = await Instance.PhotosGetRecentAsync(1, 20, PhotoSearchExtras.All);

            Assert.That(photos, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(photos.PerPage, Is.EqualTo(20));
                Assert.That(photos, Has.Count.EqualTo(20));
            });
        }

        [Test]
        public async Task PhotosGetRecentPagesTest()
        {
            var photos = await Instance.PhotosGetRecentAsync(1, 20);

            Assert.That(photos, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(photos.PerPage, Is.EqualTo(20));
                Assert.That(photos, Has.Count.EqualTo(20));
            });
        }

        [Test]
        public async Task PhotosGetRecentExtrasTest()
        {
            var photos = await Instance.PhotosGetRecentAsync(PhotoSearchExtras.OwnerName);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty);

            var photo = photos.First();
            Assert.That(photo.OwnerName, Is.Not.Null);
        }

        [Test]
        public async Task PhotosGetSizes10Test()
        {
            var o = new PhotoSearchOptions { Tags = "microsoft", PerPage = 10 };

            var photos = await Instance.PhotosSearchAsync(o);

            foreach (var p in photos)
            {
                var sizes = await Instance.PhotosGetSizesAsync(p.PhotoId);
                foreach (var s in sizes)
                {
                }
            }
        }

        [Test]
        public async Task PhotosGetSizesBasicTest()
        {
            var sizes = await Instance.PhotosGetSizesAsync("4114887196");

            Assert.That(sizes, Is.Not.Null);
            Assert.That(sizes, Is.Not.Empty);

            foreach (Size s in sizes)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(s.Label, Is.Not.Null, "Label should not be null.");
                    Assert.Multiple(() =>
                {
                    Assert.That(s.Source, Is.Not.Null, "Source should not be null.");
                    Assert.That(s.Url, Is.Not.Null, "Url should not be null.");
                });
                    Assert.That(s.Height, Is.Not.EqualTo(0), "Height should not be zero.");
                    Assert.That(s.Width, Is.Not.EqualTo(0), "Width should not be zero.");
                    Assert.That(s.MediaType, Is.Not.EqualTo(MediaType.None), "MediaType should be set.");
                });
            }
        }

        [Test]
        public async Task PhotosGetSizesVideoTest()
        {
            //https://www.flickr.com/photos/tedsherarts/4399135415/
            var sizes = await Instance.PhotosGetSizesAsync("4399135415");

            sizes.ShouldContain(s => s.MediaType == MediaType.Videos, "At least one size should contain a Video media type.");
            sizes.ShouldContain(s => s.MediaType == MediaType.Photos, "At least one size should contain a Photo media type.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetUntaggedAllParamsTest()
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetUntaggedAsync(1, 10, PhotoSearchExtras.All);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetUntaggedNoParamsTest()
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetUntaggedAsync();

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty);

            var photo = photos.First();
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetUntaggedExtrasTest()
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetUntaggedAsync(PhotoSearchExtras.OwnerName);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty);

            var photo = photos.First();

            Assert.That(photo.OwnerName, Is.Not.Null);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetUntaggedPagesTest()
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetUntaggedAsync(1, 10);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Has.Count.EqualTo(10));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosRecentlyUpdatedTests()
        {
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);
            var f = AuthInstance;

            var photos = await f.PhotosRecentlyUpdatedAsync(sixMonthsAgo, PhotoSearchExtras.All, 1, 20);

            Assert.That(photos, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(photos.PerPage, Is.EqualTo(20));
                Assert.That(photos, Is.Not.Empty);
            });

            // Overloads

            photos = await f.PhotosRecentlyUpdatedAsync(sixMonthsAgo);
            photos = await f.PhotosRecentlyUpdatedAsync(sixMonthsAgo, PhotoSearchExtras.DateTaken);
            photos = await f.PhotosRecentlyUpdatedAsync(sixMonthsAgo, 1, 10);
        }

        [Test]
        public async Task PhotosSearchDoesLargeExist()
        {
            var o = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.AllUrls,
                PerPage = 50,
                Tags = "test"
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o);

            foreach (Photo p in photos)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(p.DoesLargeExist == true || p.DoesLargeExist == false, Is.True);
                    Assert.That(p.DoesMediumExist == true || p.DoesMediumExist == false, Is.True);
                });
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosSetMetaLargeDescription()
        {
            string description;

            using (WebClient wc = new WebClient())
            {
                description = wc.DownloadString("http://en.wikipedia.org/wiki/Scots_Pine");
                // Limit to size of a url to 65519 characters, so chop the description down to a large but not too large size.
                description = description.Substring(0, 6551);
            }

            string title = "Blacksway Cat";
            string photoId = "5279984467";

            Flickr f = AuthInstance;
            await f.PhotosSetMetaAsync(photoId, title, description);
        }

        [Test]
        public async Task PhotosUploadCheckTicketsTest()
        {
            Flickr f = Instance;

            string[] tickets = new string[3];
            tickets[0] = "invalid1";
            tickets[1] = "invalid2";
            tickets[2] = "invalid3";

            var t = await f.PhotosUploadCheckTicketsAsync(tickets);

            Assert.That(t, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(t[0].TicketId, Is.EqualTo("invalid1"));
                Assert.That(t[0].PhotoId, Is.Null);
                Assert.That(t[0].InvalidTicketId, Is.True);
            });
        }

        [Test]
        public async Task PhotosPeopleGetListTest()
        {
            var photoId = "3547137580";

            var people = await Instance.PhotosPeopleGetListAsync(photoId);
            Assert.Multiple(() =>
            {
                Assert.That(people.Total, Is.Not.EqualTo(0), "Total should not be zero.");
                Assert.That(people, Is.Not.Empty, "Count should not be zero.");
            });
            Assert.That(people.Total, Is.EqualTo(people.Count), "Count should equal Total.");

            foreach (var person in people)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(person.UserId, Is.Not.Null, "UserId should not be null.");
                    Assert.Multiple(() =>
                {
                    Assert.That(person.PhotostreamUrl, Is.Not.Null, "PhotostreamUrl should not be null.");
                    Assert.That(person.BuddyIconUrl, Is.Not.Null, "BuddyIconUrl should not be null.");
                });
                });
            }
        }

        [Test]
        public async Task PhotosPeopleGetListSpecificUserTest()
        {
            string photoId = "104267998"; // https://www.flickr.com/photos/thunderchild5/104267998/
            string userId = "41888973@N00"; //sam judsons nsid

            Flickr f = Instance;
            PhotoPersonCollection ppl = await f.PhotosPeopleGetListAsync(photoId);
            PhotoPerson pp = ppl[0];
            Assert.Multiple(() =>
            {
                Assert.That(pp.UserId, Is.EqualTo(userId));
                Assert.That(pp.BuddyIconUrl.Contains(".staticflickr.com/"), Is.True, "Buddy icon doesn't contain correct details.");
            });
        }

        [Test]
        public async Task WebUrlContainsUserIdIfPathAliasIsEmpty()
        {
            var options = new PhotoSearchOptions
            {
                UserId = "39858630@N06",
                PerPage = 1,
                Extras = PhotoSearchExtras.PathAlias
            };

            var f = Instance;
            var photos = await f.PhotosSearchAsync(options);

            string webUrl = photos[0].WebUrl;
            string userPart = webUrl.Split('/')[4];

            Console.WriteLine("WebUrl is: " + webUrl);
            Assert.That(string.Empty, Is.Not.EqualTo(userPart), "User part of the URL cannot be empty");
        }

        [Test]
        public void PhotostreamUrlContainsUserIdIfPathAliasIsEmpty()
        {
            var photoPerson = new PhotoPerson()
            {
                PathAlias = string.Empty,
                UserId = "UserId",
            };

            string userPart = photoPerson.PhotostreamUrl.Split('/')[4];

            Assert.That(string.Empty, Is.Not.EqualTo(userPart), "User part of the URL cannot be empty");
        }

        [Test]
        public async Task PhotosTestLargeSquareSmall320()
        {
            var o = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.LargeSquareUrl | PhotoSearchExtras.Small320Url,
                UserId = TestData.TestUserId,
                PerPage = 10
            };

            var photos = await Instance.PhotosSearchAsync(o);
            Assert.That(photos.Count > 0, Is.True, "Should return more than zero photos.");

            foreach (var photo in photos)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(photo.Small320Url, Is.Not.Null, "Small320Url should not be null.");
                    Assert.That(photo.LargeSquareThumbnailUrl, Is.Not.Null, "LargeSquareThumbnailUrl should not be null.");
                });
            }
        }
    }
}
