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
    /// Summary description for GroupsPoolsGetGroupsTests
    /// </summary>
    [TestFixture]
    public class GroupsPoolsTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsPoolsAddBasicTest()
        {
            Flickr f = AuthInstance;

            byte[] imageBytes = TestData.TestImageBytes;
            var s = new MemoryStream(imageBytes)
            {
                Position = 0
            };

            string title = "Test Title";
            string desc = "Test Description\nSecond Line";
            string tags = "testtag1,testtag2";
            string photoId = await f.UploadPictureAsync(s, "Test.jpg", title, desc, tags, false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.Visible, default);

            try
            {
                await f.GroupsPoolsAddAsync(photoId, TestData.FlickrNetTestGroupId);
            }
            finally
            {
                await f.PhotoDeleteAsync(photoId);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public void GroupsPoolsAddNotAuthTestTest()
        {
            string photoId = "12345";

            Should.Throw<SignatureRequiredException>(async () => await Instance.GroupsPoolsAddAsync(photoId, TestData.FlickrNetTestGroupId));
        }

        [Test]
        public async Task GroupsPoolGetPhotosFullParamTest()
        {
            Flickr f = Instance;

            PhotoCollection photos = await f.GroupsPoolsGetPhotosAsync(TestData.GroupId, null, TestData.TestUserId, PhotoSearchExtras.All, 1, 20);

            Assert.That(photos, Is.Not.Null, "Photos should not be null");
            Assert.That(photos, Is.Not.Empty, "Should be more than 0 photos returned");
            Assert.Multiple(() =>
            {
                Assert.That(photos.PerPage, Is.EqualTo(20));
                Assert.That(photos.Page, Is.EqualTo(1));
            });
            foreach (Photo p in photos)
            {
                Assert.That(p.DateAddedToGroup, Is.Not.EqualTo(default(DateTime)), "DateAddedToGroup should not be default value");
                Assert.That(p.DateAddedToGroup, Is.LessThan(DateTime.Now), "DateAddedToGroup should be in the past");
            }
        }

        [Test]
        public async Task GroupsPoolGetPhotosDateAddedTest()
        {
            Flickr f = Instance;

            PhotoCollection photos = await f.GroupsPoolsGetPhotosAsync(TestData.GroupId);

            Assert.That(photos, Is.Not.Null, "Photos should not be null");
            Assert.That(photos, Is.Not.Empty, "Should be more than 0 photos returned");

            foreach (Photo p in photos)
            {
                Assert.That(p.DateAddedToGroup, Is.Not.EqualTo(default(DateTime)), "DateAddedToGroup should not be default value");
                Assert.That(p.DateAddedToGroup, Is.LessThan(DateTime.Now), "DateAddedToGroup should be in the past");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsPoolsGetGroupsBasicTest()
        {
            MemberGroupInfoCollection groups = await AuthInstance.GroupsPoolsGetGroupsAsync();

            Assert.That(groups, Is.Not.Null, "MemberGroupInfoCollection should not be null.");
            Assert.That(groups, Is.Not.Empty, "MemberGroupInfoCollection.Count should not be zero.");
            Assert.That(groups, Has.Count.GreaterThan(1), "Count should be greater than one.");
            Assert.Multiple(() =>
            {
                Assert.That(groups.PerPage, Is.EqualTo(400), "PerPage should be 400.");
                Assert.That(groups.Page, Is.EqualTo(1), "Page should be 1.");
            });
            Assert.That(groups.Total, Is.GreaterThan(1), "Total chould be greater than one");
        }
    }
}