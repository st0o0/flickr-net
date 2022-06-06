using System;

using NUnit.Framework;
using FlickrNet;
using Shouldly;
using FlickrNet.CollectionModels;
using System.Threading.Tasks;
using System.Threading;
using FlickrNet.Models;
using FlickrNet.Enums;
using FlickrNet.Exceptions;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PeopleTests
    /// </summary>
    [TestFixture]
    public class PeopleTests : BaseTest
    {
        [Test]
        public async Task PeopleGetPhotosOfBasicTest(CancellationToken cancellationToken = default)
        {
            PeoplePhotoCollection p = await Instance.PeopleGetPhotosOfAsync(TestData.TestUserId, cancellationToken);

            Assert.IsNotNull(p, "PeoplePhotos should not be null.");
            Assert.AreNotEqual(0, p.Count, "PeoplePhotos.Count should be greater than zero.");
            Assert.IsTrue(p.PerPage >= p.Count, "PerPage should be the same or greater than the number of photos returned.");
        }

        [Test]
        public async Task PeopleGetPhotosOfAuthRequired(CancellationToken cancellationToken = default)
        {
            Should.Throw<SignatureRequiredException>(async () => await Instance.PeopleGetPhotosOfAsync(cancellationToken));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetPhotosOfMe(CancellationToken cancellationToken = default)
        {
            PeoplePhotoCollection p = await AuthInstance.PeopleGetPhotosOfAsync(cancellationToken);

            Assert.IsNotNull(p, "PeoplePhotos should not be null.");
            Assert.AreNotEqual(0, p.Count, "PeoplePhotos.Count should be greater than zero.");
            Assert.IsTrue(p.PerPage >= p.Count, "PerPage should be the same or greater than the number of photos returned.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetPhotosBasicTest(CancellationToken cancellationToken = default)
        {
            PhotoCollection photos = await AuthInstance.PeopleGetPhotosAsync(cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count, "Count should not be zero.");
            Assert.IsTrue(photos.Total > 1000, "Total should be greater than 1000.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetPhotosFullParamTest(CancellationToken cancellationToken = default)
        {
            PhotoCollection photos = await AuthInstance.PeopleGetPhotosAsync(TestData.TestUserId, SafetyLevel.Safe, new DateTime(2010, 1, 1),
                                                       new DateTime(2012, 1, 1), new DateTime(2010, 1, 1),
                                                       new DateTime(2012, 1, 1), ContentTypeSearch.All,
                                                       PrivacyFilter.PublicPhotos, PhotoSearchExtras.All, 1, 20, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreEqual(20, photos.Count, "Count should be twenty.");
        }

        [Test]
        public async Task PeopleGetInfoBasicTestUnauth(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;
            Person p = await f.PeopleGetInfoAsync(TestData.TestUserId, cancellationToken);

            Assert.AreEqual("Sam Judson", p.UserName);
            Assert.AreEqual("Sam Judson", p.RealName);
            Assert.AreEqual("samjudson", p.PathAlias);
            Assert.IsFalse(p.IsPro, "IsPro should be true.");
            Assert.AreEqual("Newcastle, UK", p.Location);
            Assert.AreEqual("+00:00", p.TimeZoneOffset);
            Assert.AreEqual("GMT: Dublin, Edinburgh, Lisbon, London", p.TimeZoneLabel);
            Assert.IsNotNull(p.Description, "Description should not be null.");
            Assert.IsTrue(p.Description.Length > 0, "Description should not be empty");
        }

        [Test]
        public async Task PeopleGetInfoGenderNoAuthTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;
            Person p = await f.PeopleGetInfoAsync("10973297@N00", cancellationToken);

            Assert.IsNotNull(p, "Person object should be returned");
            Assert.IsNull(p.Gender, "Gender should be null as not authenticated.");

            Assert.IsNull(p.IsReverseContact, "IsReverseContact should not be null.");
            Assert.IsNull(p.IsContact, "IsContact should be null.");
            Assert.IsNull(p.IsIgnored, "IsIgnored should be null.");
            Assert.IsNull(p.IsFriend, "IsFriend should be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetInfoGenderTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;
            Person p = await f.PeopleGetInfoAsync("10973297@N00", cancellationToken);

            Assert.IsNotNull(p, "Person object should be returned");
            Assert.AreEqual("F", p.Gender, "Gender of M should be returned");

            Assert.IsNotNull(p.IsReverseContact, "IsReverseContact should not be null.");
            Assert.IsNotNull(p.IsContact, "IsContact should not be null.");
            Assert.IsNotNull(p.IsIgnored, "IsIgnored should not be null.");
            Assert.IsNotNull(p.IsFriend, "IsFriend should not be null.");

            Assert.IsNotNull(p.PhotosSummary, "PhotosSummary should not be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetInfoBuddyIconTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;
            Person p = await f.PeopleGetInfoAsync(TestData.TestUserId, cancellationToken);
            Assert.IsTrue(p.BuddyIconUrl.Contains(".staticflickr.com/"), "Buddy icon doesn't contain correct details.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetInfoSelfTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            Person p = await f.PeopleGetInfoAsync(TestData.TestUserId, cancellationToken);

            Assert.IsNotNull(p.MailboxSha1Hash, "MailboxSha1Hash should not be null.");
            Assert.IsNotNull(p.PhotosSummary, "PhotosSummary should not be null.");
            Assert.AreNotEqual(0, p.PhotosSummary.Views, "PhotosSummary.Views should not be zero.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetGroupsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var groups = await f.PeopleGetGroupsAsync(TestData.TestUserId, cancellationToken);

            Assert.IsNotNull(groups);
            Assert.AreNotEqual(0, groups.Count);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetLimitsTest(CancellationToken cancellationToken = default)
        {
            var f = AuthInstance;

            var limits = await f.PeopleGetLimitsAsync(cancellationToken);

            Assert.IsNotNull(limits);

            Assert.AreEqual(0, limits.MaximumDisplayPixels);
            Assert.AreEqual(209715200, limits.MaximumPhotoUpload);
            Assert.AreEqual(1073741824, limits.MaximumVideoUpload);
            Assert.AreEqual(180, limits.MaximumVideoDuration);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleFindByUsername(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            FoundUser user = await f.PeopleFindByUserNameAsync("Sam Judson", cancellationToken);

            Assert.AreEqual("41888973@N00", user.UserId);
            Assert.AreEqual("Sam Judson", user.UserName);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleFindByEmail(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            FoundUser user = await f.PeopleFindByEmailAsync("samjudson@gmail.com", cancellationToken);

            Assert.AreEqual("41888973@N00", user.UserId);
            Assert.AreEqual("Sam Judson", user.UserName);
        }

        [Test]
        public async Task PeopleGetPublicPhotosBasicTest(CancellationToken cancellationToken = default)
        {
            var f = Instance;
            var photos = await f.PeopleGetPublicPhotosAsync(TestData.TestUserId, 1, 100, SafetyLevel.None, PhotoSearchExtras.OriginalDimensions, cancellationToken);

            Assert.IsNotNull(photos);
            Assert.AreNotEqual(0, photos.Count);

            foreach (var photo in photos)
            {
                Assert.IsNotNull(photo.PhotoId, "PhotoId should not be null.");
                Assert.AreEqual(TestData.TestUserId, photo.UserId);
                Assert.AreNotEqual(0, photo.OriginalWidth, "OriginalWidth should not be zero.");
                Assert.AreNotEqual(0, photo.OriginalHeight, "OriginalHeight should not be zero.");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetPublicGroupsBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            GroupInfoCollection groups = await f.PeopleGetPublicGroupsAsync(TestData.TestUserId, cancellationToken);

            Assert.AreNotEqual(0, groups.Count, "PublicGroupInfoCollection.Count should not be zero.");

            foreach (GroupInfo group in groups)
            {
                Assert.IsNotNull(group.GroupId, "GroupId should not be null.");
                Assert.IsNotNull(group.GroupName, "GroupName should not be null.");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetUploadStatusBasicTest(CancellationToken cancellationToken = default)
        {
            var u = await AuthInstance.PeopleGetUploadStatusAsync(cancellationToken);

            Assert.IsNotNull(u);
            Assert.IsNotNull(u.UserId);
            Assert.IsNotNull(u.UserName);
            Assert.AreNotEqual(0, u.FileSizeMax);
        }

        [Test]
        public async Task PeopleGetInfoBlankDate(CancellationToken cancellationToken = default)
        {
            var p = await Instance.PeopleGetInfoAsync("18387778@N00", cancellationToken);
        }

        [Test]
        public async Task PeopleGetInfoZeroDate(CancellationToken cancellationToken = default)
        {
            var p = await Instance.PeopleGetInfoAsync("47963952@N03", cancellationToken);
        }

        [Test]
        public async Task PeopleGetInfoInternationalCharacters(CancellationToken cancellationToken = default)
        {
            var p = await Instance.PeopleGetInfoAsync("24754141@N08", cancellationToken);

            Assert.AreEqual("24754141@N08", p.UserId, "UserId should match.");
            Assert.AreEqual("Pierre Hsiu 脩丕政", p.RealName, "RealName should match");
        }
    }
}