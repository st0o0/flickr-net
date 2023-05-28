using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Exceptions;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using Shouldly;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PeopleTests
    /// </summary>
    [TestFixture]
    public class PeopleTests : BaseTest
    {
        [Test]
        public async Task PeopleGetPhotosOfBasicTest()
        {
            PeoplePhotoCollection p = await Instance.PeopleGetPhotosOfAsync(TestData.TestUserId);

            Assert.That(p, Is.Not.Null, "PeoplePhotos should not be null.");
            Assert.That(p, Is.Not.Empty, "PeoplePhotos.Count should be greater than zero.");
            Assert.That(p.PerPage >= p.Count, Is.True, "PerPage should be the same or greater than the number of photos returned.");
        }

        [Test]
        public void PeopleGetPhotosOfAuthRequired()
        {
            Should.Throw<SignatureRequiredException>(async () => await Instance.PeopleGetPhotosOfAsync());
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetPhotosOfMe()
        {
            PeoplePhotoCollection p = await AuthInstance.PeopleGetPhotosOfAsync();

            Assert.That(p, Is.Not.Null, "PeoplePhotos should not be null.");
            Assert.That(p, Is.Not.Empty, "PeoplePhotos.Count should be greater than zero.");
            Assert.That(p.PerPage >= p.Count, Is.True, "PerPage should be the same or greater than the number of photos returned.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetPhotosBasicTest()
        {
            PhotoCollection photos = await AuthInstance.PeopleGetPhotosAsync();

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty, "Count should not be zero.");
            Assert.That(photos.Total > 1000, Is.True, "Total should be greater than 1000.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetPhotosFullParamTest()
        {
            PhotoCollection photos = await AuthInstance.PeopleGetPhotosAsync(TestData.TestUserId, SafetyLevel.Safe, new DateTime(2010, 1, 1),
                                                       new DateTime(2012, 1, 1), new DateTime(2010, 1, 1),
                                                       new DateTime(2012, 1, 1), ContentTypeSearch.All,
                                                       PrivacyFilter.PublicPhotos, PhotoSearchExtras.All, 1, 20);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Has.Count.EqualTo(20), "Count should be twenty.");
        }

        [Test]
        public async Task PeopleGetInfoBasicTestUnauth()
        {
            Flickr f = Instance;
            Person p = await f.PeopleGetInfoAsync(TestData.TestUserId);
            Assert.Multiple(() =>
            {
                Assert.That(p.UserName, Is.EqualTo("Sam Judson"));
                Assert.Multiple(() =>
            {
                Assert.That(p.RealName, Is.EqualTo("Sam Judson"));
                Assert.That(p.PathAlias, Is.EqualTo("samjudson"));
            });
                Assert.That(p.IsPro, Is.False, "IsPro should be true.");
                Assert.Multiple(() =>
                {
                    Assert.That(p.Location, Is.EqualTo("Newcastle, UK"));
                    Assert.Multiple(() =>
                {
                    Assert.That(p.TimeZoneOffset, Is.EqualTo("+00:00"));
                    Assert.That(p.TimeZoneLabel, Is.EqualTo("GMT: Dublin, Edinburgh, Lisbon, London"));
                    Assert.That(p.Description, Is.Not.Null, "Description should not be null.");
                });
                    Assert.That(p.Description.Length > 0, Is.True, "Description should not be empty");
                });
            });
        }

        [Test]
        public async Task PeopleGetInfoGenderNoAuthTest()
        {
            Flickr f = Instance;
            Person p = await f.PeopleGetInfoAsync("10973297@N00");

            Assert.That(p, Is.Not.Null, "Person object should be returned");
            Assert.That(p.Gender, Is.Null, "Gender should be null as not authenticated.");

            Assert.That(p.IsReverseContact, Is.Null, "IsReverseContact should not be null.");
            Assert.That(p.IsContact, Is.Null, "IsContact should be null.");
            Assert.That(p.IsIgnored, Is.Null, "IsIgnored should be null.");
            Assert.That(p.IsFriend, Is.Null, "IsFriend should be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetInfoGenderTest()
        {
            Flickr f = AuthInstance;
            Person p = await f.PeopleGetInfoAsync("10973297@N00");

            Assert.That(p, Is.Not.Null, "Person object should be returned");
            Assert.That(p.Gender, Is.EqualTo("F"), "Gender of M should be returned");
            Assert.Multiple(() =>
            {
                Assert.That(p.IsReverseContact, Is.Not.Null, "IsReverseContact should not be null.");
                Assert.Multiple(() =>
            {
                Assert.That(p.IsContact, Is.Not.Null, "IsContact should not be null.");
                Assert.That(p.IsIgnored, Is.Not.Null, "IsIgnored should not be null.");
                Assert.That(p.IsFriend, Is.Not.Null, "IsFriend should not be null.");

                Assert.That(p.PhotosSummary, Is.Not.Null, "PhotosSummary should not be null.");
            });
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetInfoBuddyIconTest()
        {
            Flickr f = AuthInstance;
            Person p = await f.PeopleGetInfoAsync(TestData.TestUserId);
            Assert.That(p.BuddyIconUrl.Contains(".staticflickr.com/"), Is.True, "Buddy icon doesn't contain correct details.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetInfoSelfTest()
        {
            Flickr f = AuthInstance;

            Person p = await f.PeopleGetInfoAsync(TestData.TestUserId);
            Assert.Multiple(() =>
            {
                Assert.That(p.MailboxSha1Hash, Is.Not.Null, "MailboxSha1Hash should not be null.");
                Assert.That(p.PhotosSummary, Is.Not.Null, "PhotosSummary should not be null.");
            });
            Assert.That(p.PhotosSummary.Views, Is.Not.EqualTo(0), "PhotosSummary.Views should not be zero.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetGroupsTest()
        {
            Flickr f = AuthInstance;

            var groups = await f.PeopleGetGroupsAsync(TestData.TestUserId);

            Assert.That(groups, Is.Not.Null);
            Assert.That(groups, Is.Not.Empty);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetLimitsTest()
        {
            var f = AuthInstance;

            var limits = await f.PeopleGetLimitsAsync();

            Assert.That(limits, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(limits.MaximumDisplayPixels, Is.EqualTo(0));
                Assert.That(limits.MaximumPhotoUpload, Is.EqualTo(209715200));
                Assert.That(limits.MaximumVideoUpload, Is.EqualTo(1073741824));
                Assert.That(limits.MaximumVideoDuration, Is.EqualTo(180));
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleFindByUsername()
        {
            Flickr f = AuthInstance;

            FoundUser user = await f.PeopleFindByUserNameAsync("Sam Judson");
            Assert.Multiple(() =>
            {
                Assert.That(user.UserId, Is.EqualTo("41888973@N00"));
                Assert.That(user.UserName, Is.EqualTo("Sam Judson"));
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleFindByEmail()
        {
            Flickr f = AuthInstance;

            FoundUser user = await f.PeopleFindByEmailAsync("samjudson@gmail.com");
            Assert.Multiple(() =>
            {
                Assert.That(user.UserId, Is.EqualTo("41888973@N00"));
                Assert.That(user.UserName, Is.EqualTo("Sam Judson"));
            });
        }

        [Test]
        public async Task PeopleGetPublicPhotosBasicTest()
        {
            var f = Instance;
            var photos = await f.PeopleGetPublicPhotosAsync(TestData.TestUserId, 1, 100, SafetyLevel.None, PhotoSearchExtras.OriginalDimensions);

            Assert.That(photos, Is.Not.Null);
            Assert.That(photos, Is.Not.Empty);

            foreach (var photo in photos)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(photo.PhotoId, Is.Not.Null, "PhotoId should not be null.");
                    Assert.That(photo.UserId, Is.EqualTo(TestData.TestUserId));
                    Assert.That(photo.OriginalWidth, Is.Not.EqualTo(0), "OriginalWidth should not be zero.");
                    Assert.That(photo.OriginalHeight, Is.Not.EqualTo(0), "OriginalHeight should not be zero.");
                });
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetPublicGroupsBasicTest()
        {
            Flickr f = AuthInstance;

            GroupInfoCollection groups = await f.PeopleGetPublicGroupsAsync(TestData.TestUserId);

            Assert.That(groups, Is.Not.Empty, "PublicGroupInfoCollection.Count should not be zero.");

            foreach (GroupInfo group in groups)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(group.GroupId, Is.Not.Null, "GroupId should not be null.");
                    Assert.That(group.GroupName, Is.Not.Null, "GroupName should not be null.");
                });
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PeopleGetUploadStatusBasicTest()
        {
            var u = await AuthInstance.PeopleGetUploadStatusAsync();

            Assert.That(u, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(u.UserId, Is.Not.Null);
                Assert.That(u.UserName, Is.Not.Null);
            });
            Assert.That(u.FileSizeMax, Is.Not.EqualTo(0));
        }

        [Test]
        public async Task PeopleGetInfoBlankDate()
        {
            var p = await Instance.PeopleGetInfoAsync("18387778@N00");
        }

        [Test]
        public async Task PeopleGetInfoZeroDate()
        {
            var p = await Instance.PeopleGetInfoAsync("47963952@N03");
        }

        [Test]
        public async Task PeopleGetInfoInternationalCharacters()
        {
            var p = await Instance.PeopleGetInfoAsync("24754141@N08");
            Assert.Multiple(() =>
            {
                Assert.That(p.UserId, Is.EqualTo("24754141@N08"), "UserId should match.");
                Assert.That(p.RealName, Is.EqualTo("Pierre Hsiu 脩丕政"), "RealName should match");
            });
        }
    }
}