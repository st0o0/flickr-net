using System;

using NUnit.Framework;
using FlickrNet;
using System.IO;
using Shouldly;
using FlickrNet.Enums;
using System.Threading;
using System.Threading.Tasks;
using FlickrNet.Exceptions;
using FlickrNet.CollectionModels;
using FlickrNet.Models;

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
        public async Task GroupsPoolsAddBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            byte[] imageBytes = TestData.TestImageBytes;
            var s = new MemoryStream(imageBytes);
            s.Position = 0;

            string title = "Test Title";
            string desc = "Test Description\nSecond Line";
            string tags = "testtag1,testtag2";
            string photoId = await f.UploadPictureAsync(s, "Test.jpg", title, desc, tags, false, false, false, ContentType.Other, SafetyLevel.Safe, HiddenFromSearch.Visible, cancellationToken: cancellationToken);

            try
            {
                await f.GroupsPoolsAddAsync(photoId, TestData.FlickrNetTestGroupId, cancellationToken);
            }
            finally
            {
                await f.PhotoDeleteAsync(photoId, cancellationToken);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public void GroupsPoolsAddNotAuthTestTest(CancellationToken cancellationToken = default)
        {
            string photoId = "12345";

            Should.Throw<SignatureRequiredException>(async () => await Instance.GroupsPoolsAddAsync(photoId, TestData.FlickrNetTestGroupId, cancellationToken));
        }

        [Test]
        public async Task GroupsPoolGetPhotosFullParamTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            PhotoCollection photos = await f.GroupsPoolsGetPhotosAsync(TestData.GroupId, null, TestData.TestUserId, PhotoSearchExtras.All, 1, 20, cancellationToken);

            Assert.IsNotNull(photos, "Photos should not be null");
            Assert.IsTrue(photos.Count > 0, "Should be more than 0 photos returned");
            Assert.AreEqual(20, photos.PerPage);
            Assert.AreEqual(1, photos.Page);

            foreach (Photo p in photos)
            {
                Assert.AreNotEqual(default(DateTime), p.DateAddedToGroup, "DateAddedToGroup should not be default value");
                Assert.IsTrue(p.DateAddedToGroup < DateTime.Now, "DateAddedToGroup should be in the past");
            }
        }

        [Test]
        public async Task GroupsPoolGetPhotosDateAddedTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            PhotoCollection photos = await f.GroupsPoolsGetPhotosAsync(TestData.GroupId, cancellationToken);

            Assert.IsNotNull(photos, "Photos should not be null");
            Assert.IsTrue(photos.Count > 0, "Should be more than 0 photos returned");

            foreach (Photo p in photos)
            {
                Assert.AreNotEqual(default(DateTime), p.DateAddedToGroup, "DateAddedToGroup should not be default value");
                Assert.IsTrue(p.DateAddedToGroup < DateTime.Now, "DateAddedToGroup should be in the past");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsPoolsGetGroupsBasicTest(CancellationToken cancellationToken = default)
        {
            MemberGroupInfoCollection groups = await AuthInstance.GroupsPoolsGetGroupsAsync(cancellationToken);

            Assert.IsNotNull(groups, "MemberGroupInfoCollection should not be null.");
            Assert.AreNotEqual(0, groups.Count, "MemberGroupInfoCollection.Count should not be zero.");
            Assert.IsTrue(groups.Count > 1, "Count should be greater than one.");

            Assert.AreEqual(400, groups.PerPage, "PerPage should be 400.");
            Assert.AreEqual(1, groups.Page, "Page should be 1.");
            Assert.IsTrue(groups.Total > 1, "Total chould be greater than one");
        }
    }
}