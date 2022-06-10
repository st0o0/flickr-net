using FlickrNet;
using FlickrNet.CollectionModels;
using FlickrNet.Enums;
using FlickrNet.Models;
using FlickrNet.SearchOptions;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    [TestFixture]
    public class PhotosTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosSetDatesTest(CancellationToken cancellationToken = default)
        {
            var f = AuthInstance;
            var photoId = TestData.PhotoId;

            var info = await f.PhotosGetInfoAsync(photoId, cancellationToken);

            await f.PhotosSetDatesAsync(photoId, info.DatePosted, info.DateTaken, info.DateTakenGranularity, cancellationToken);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosAddTagsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;
            string testtag = "thisisatesttag";
            string photoId = "6282363572";

            // Add the tag
            await f.PhotosAddTagsAsync(photoId, new[] { testtag }, cancellationToken);
            // Add second tag using different signature
            await f.PhotosAddTagsAsync(photoId, new string[] { testtag + "2" }, cancellationToken);

            // Get list of tags
            var tags = await f.TagsGetListPhotoAsync(photoId, cancellationToken);

            // Find the tag in the collection
            var tagsToRemove = tags.Where(t => t.TagText.StartsWith(testtag, StringComparison.Ordinal));

            foreach (var tag in tagsToRemove)
            {
                // Remove the tag
                await f.PhotosRemoveTagAsync(tag.TagId, cancellationToken);
            }
        }

        [Test]
        public async Task PhotosGetAllContextsBasicTest(CancellationToken cancellationToken = default)
        {
            var a = await Instance.PhotosGetAllContextsAsync("4114887196", cancellationToken);

            Assert.IsNotNull(a);
            Assert.IsNotNull(a.Groups, "Groups should not be null.");
            Assert.IsNotNull(a.Sets, "Sets should not be null.");

            Assert.AreEqual(1, a.Groups.Count, "Groups.Count should be one.");
            Assert.AreEqual(1, a.Sets.Count, "Sets.Count should be one.");
        }

        [Test]
        public async Task PhotosGetPopular(CancellationToken cancellationToken = default)
        {
            // TODO: LOST
            //var photos = await Instance.PhotosGetPopular(TestData.TestUserId);

            //photos.ShouldNotBeEmpty();
        }

        [Test]
        public async Task PhotosGetExifTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            ExifTagCollection tags = await f.PhotosGetExifAsync("4268023123", cancellationToken);

            Console.WriteLine(f.LastResponse);

            Assert.IsNotNull(tags, "ExifTagCollection should not be null.");

            Assert.IsTrue(tags.Count > 20, "More than twenty parts of EXIF data should be returned.");

            Assert.AreEqual("IFD0", tags[0].TagSpace, "First tags TagSpace is not set correctly.");
            Assert.AreEqual(0, tags[0].TagSpaceId, "First tags TagSpaceId is not set correctly.");
            Assert.AreEqual("Compression", tags[0].Tag, "First tags Tag is not set correctly.");
            Assert.AreEqual("Compression", tags[0].Label, "First tags Label is not set correctly.");
            Assert.AreEqual("JPEG (old-style)", tags[0].Raw, "First tags RAW is not correct.");
            Assert.IsNull(tags[0].Clean, "First tags Clean should be null.");
        }

        [Test]
        public async Task PhotosGetContextBasicTest(CancellationToken cancellationToken = default)
        {
            var context = await Instance.PhotosGetContextAsync("3845365350", cancellationToken);

            Assert.IsNotNull(context);

            Assert.AreEqual("3844573707", context.PreviousPhoto.PhotoId);
            Assert.AreEqual("3992605178", context.NextPhoto.PhotoId);
        }

        [Test]
        public async Task PhotosGetExifIPhoneTest(CancellationToken cancellationToken = default)
        {
            bool bFound = false;
            Flickr f = Instance;

            ExifTagCollection tags = await f.PhotosGetExifAsync("5899928191", cancellationToken);

            Assert.AreEqual("Apple iPhone 4", tags.Camera, "Camera property should be set correctly.");

            foreach (ExifTag tag in tags)
            {
                if (tag.Tag == "Model")
                {
                    Assert.IsTrue(tag.Raw == "iPhone 4", "Model tag is not 'iPhone 4'");
                    bFound = true;
                    break;
                }
            }
            Assert.IsTrue(bFound, "Model tag not found.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetNotInSetAllParamsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetNotInSetAsync(1, 10, PhotoSearchExtras.All, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreEqual(10, photos.Count);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetNotInSetNoParamsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetNotInSetAsync(cancellationToken);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetNotInSetPagesTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetNotInSetAsync(1, 11, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreEqual(11, photos.Count);

            // Overloads
            await f.PhotosGetNotInSetAsync(cancellationToken);
            await f.PhotosGetNotInSetAsync(1, cancellationToken);
            await f.PhotosGetNotInSetAsync(new PartialSearchOptions() { Page = 1, PerPage = 10, PrivacyFilter = PrivacyFilter.CompletelyPrivate }, cancellationToken);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetPermsBasicTest(CancellationToken cancellationToken = default)
        {
            var p = await AuthInstance.PhotosGetPermsAsync("4114887196", cancellationToken);

            Assert.IsNotNull(p);
            Assert.AreEqual("4114887196", p.PhotoId);
            Assert.AreNotEqual(PermissionComment.Nobody, p.PermissionComment);
        }

        [Test]
        public async Task PhotosGetRecentBlankTest(CancellationToken cancellationToken = default)
        {
            var photos = await Instance.PhotosGetRecentAsync(cancellationToken);

            Assert.IsNotNull(photos);
        }

        [Test]
        public async Task PhotosGetRecentAllParamsTest(CancellationToken cancellationToken = default)
        {
            var photos = await Instance.PhotosGetRecentAsync(1, 20, PhotoSearchExtras.All, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreEqual(20, photos.PerPage);
            Assert.AreEqual(20, photos.Count);
        }

        [Test]
        public async Task PhotosGetRecentPagesTest(CancellationToken cancellationToken = default)
        {
            var photos = await Instance.PhotosGetRecentAsync(1, 20, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreEqual(20, photos.PerPage);
            Assert.AreEqual(20, photos.Count);
        }

        [Test]
        public async Task PhotosGetRecentExtrasTest(CancellationToken cancellationToken = default)
        {
            var photos = await Instance.PhotosGetRecentAsync(PhotoSearchExtras.OwnerName, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count);

            var photo = photos.First();
            Assert.IsNotNull(photo.OwnerName);
        }

        [Test]
        public async Task PhotosGetSizes10Test(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions { Tags = "microsoft", PerPage = 10 };

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (var p in photos)
            {
                var sizes = await Instance.PhotosGetSizesAsync(p.PhotoId, cancellationToken);
                foreach (var s in sizes)
                {
                }
            }
        }

        [Test]
        public async Task PhotosGetSizesBasicTest(CancellationToken cancellationToken = default)
        {
            var sizes = await Instance.PhotosGetSizesAsync("4114887196", cancellationToken);

            Assert.IsNotNull(sizes);
            Assert.AreNotEqual(0, sizes.Count);

            foreach (Size s in sizes)
            {
                Assert.IsNotNull(s.Label, "Label should not be null.");
                Assert.IsNotNull(s.Source, "Source should not be null.");
                Assert.IsNotNull(s.Url, "Url should not be null.");
                Assert.AreNotEqual(0, s.Height, "Height should not be zero.");
                Assert.AreNotEqual(0, s.Width, "Width should not be zero.");
                Assert.AreNotEqual(MediaType.None, s.MediaType, "MediaType should be set.");
            }
        }

        [Test]
        public async Task PhotosGetSizesVideoTest(CancellationToken cancellationToken = default)
        {
            //https://www.flickr.com/photos/tedsherarts/4399135415/
            var sizes = await Instance.PhotosGetSizesAsync("4399135415", cancellationToken);

            sizes.ShouldContain(s => s.MediaType == MediaType.Videos, "At least one size should contain a Video media type.");
            sizes.ShouldContain(s => s.MediaType == MediaType.Photos, "At least one size should contain a Photo media type.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetUntaggedAllParamsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetUntaggedAsync(1, 10, PhotoSearchExtras.All, cancellationToken);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetUntaggedNoParamsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetUntaggedAsync(cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count);

            var photo = photos.First();
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetUntaggedExtrasTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetUntaggedAsync(PhotoSearchExtras.OwnerName, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count);

            var photo = photos.First();

            Assert.IsNotNull(photo.OwnerName);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetUntaggedPagesTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var photos = await f.PhotosGetUntaggedAsync(1, 10, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreEqual(10, photos.Count);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosRecentlyUpdatedTests(CancellationToken cancellationToken = default)
        {
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);
            var f = AuthInstance;

            var photos = await f.PhotosRecentlyUpdatedAsync(sixMonthsAgo, PhotoSearchExtras.All, 1, 20, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreEqual(20, photos.PerPage);
            Assert.AreNotEqual(0, photos.Count);

            // Overloads

            photos = await f.PhotosRecentlyUpdatedAsync(sixMonthsAgo, cancellationToken);
            photos = await f.PhotosRecentlyUpdatedAsync(sixMonthsAgo, PhotoSearchExtras.DateTaken, cancellationToken);
            photos = await f.PhotosRecentlyUpdatedAsync(sixMonthsAgo, 1, 10, cancellationToken);
        }

        [Test]
        public async Task PhotosSearchDoesLargeExist(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.AllUrls,
                PerPage = 50,
                Tags = "test"
            };

            PhotoCollection photos = await Instance.PhotosSearchAsync(o, cancellationToken);

            foreach (Photo p in photos)
            {
                Assert.IsTrue(p.DoesLargeExist == true || p.DoesLargeExist == false);
                Assert.IsTrue(p.DoesMediumExist == true || p.DoesMediumExist == false);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosSetMetaLargeDescription(CancellationToken cancellationToken = default)
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
            await f.PhotosSetMetaAsync(photoId, title, description, cancellationToken);
        }

        [Test]
        public async Task PhotosUploadCheckTicketsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            string[] tickets = new string[3];
            tickets[0] = "invalid1";
            tickets[1] = "invalid2";
            tickets[2] = "invalid3";

            var t = await f.PhotosUploadCheckTicketsAsync(tickets, cancellationToken);

            Assert.AreEqual(3, t.Count);

            Assert.AreEqual("invalid1", t[0].TicketId);
            Assert.IsNull(t[0].PhotoId);
            Assert.IsTrue(t[0].InvalidTicketId);
        }

        [Test]
        public async Task PhotosPeopleGetListTest(CancellationToken cancellationToken = default)
        {
            var photoId = "3547137580";

            var people = await Instance.PhotosPeopleGetListAsync(photoId, cancellationToken);

            Assert.AreNotEqual(0, people.Total, "Total should not be zero.");
            Assert.AreNotEqual(0, people.Count, "Count should not be zero.");
            Assert.AreEqual(people.Count, people.Total, "Count should equal Total.");

            foreach (var person in people)
            {
                Assert.IsNotNull(person.UserId, "UserId should not be null.");
                Assert.IsNotNull(person.PhotostreamUrl, "PhotostreamUrl should not be null.");
                Assert.IsNotNull(person.BuddyIconUrl, "BuddyIconUrl should not be null.");
            }
        }

        [Test]
        public async Task PhotosPeopleGetListSpecificUserTest(CancellationToken cancellationToken = default)
        {
            string photoId = "104267998"; // https://www.flickr.com/photos/thunderchild5/104267998/
            string userId = "41888973@N00"; //sam judsons nsid

            Flickr f = Instance;
            PhotoPersonCollection ppl = await f.PhotosPeopleGetListAsync(photoId, cancellationToken);
            PhotoPerson pp = ppl[0];
            Assert.AreEqual(userId, pp.UserId);
            Assert.IsTrue(pp.BuddyIconUrl.Contains(".staticflickr.com/"), "Buddy icon doesn't contain correct details.");
        }

        [Test]
        public async Task WebUrlContainsUserIdIfPathAliasIsEmpty(CancellationToken cancellationToken = default)
        {
            var options = new PhotoSearchOptions
            {
                UserId = "39858630@N06",
                PerPage = 1,
                Extras = PhotoSearchExtras.PathAlias
            };

            var f = Instance;
            var photos = await f.PhotosSearchAsync(options, cancellationToken);

            string webUrl = photos[0].WebUrl;
            string userPart = webUrl.Split('/')[4];

            Console.WriteLine("WebUrl is: " + webUrl);
            Assert.AreNotEqual(userPart, string.Empty, "User part of the URL cannot be empty");
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

            Assert.AreNotEqual(userPart, string.Empty, "User part of the URL cannot be empty");
        }

        [Test]
        public async Task PhotosTestLargeSquareSmall320(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                Extras = PhotoSearchExtras.LargeSquareUrl | PhotoSearchExtras.Small320Url,
                UserId = TestData.TestUserId,
                PerPage = 10
            };

            var photos = await Instance.PhotosSearchAsync(o, cancellationToken);
            Assert.IsTrue(photos.Count > 0, "Should return more than zero photos.");

            foreach (var photo in photos)
            {
                Assert.IsNotNull(photo.Small320Url, "Small320Url should not be null.");
                Assert.IsNotNull(photo.LargeSquareThumbnailUrl, "LargeSquareThumbnailUrl should not be null.");
            }
        }
    }
}