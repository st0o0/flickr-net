using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for GroupsBrowseTests
    /// </summary>
    [TestFixture]
    public class GroupsTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsBrowseBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;
            GroupCategory cat = await f.GroupsBrowseAsync(cancellationToken);

            Assert.IsNotNull(cat, "GroupCategory should not be null.");
            Assert.AreEqual("/", cat.CategoryName, "CategoryName should be '/'.");
            Assert.AreEqual("/", cat.Path, "Path should be '/'.");
            Assert.AreEqual("", cat.PathIds, "PathIds should be empty string.");
            Assert.AreEqual(0, cat.Subcategories.Count, "No sub categories should be returned.");
            Assert.AreEqual(0, cat.Groups.Count, "No groups should be returned.");
        }

        [Test]
        public async Task GroupsSearchBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            GroupSearchResultCollection results = await f.GroupsSearchAsync("Buses", cancellationToken);

            Assert.IsNotNull(results, "GroupSearchResults should not be null.");
            Assert.AreNotEqual(0, results.Count, "Count should not be zero.");
            Assert.AreNotEqual(0, results.Total, "Total should not be zero.");
            Assert.AreNotEqual(0, results.PerPage, "PerPage should not be zero.");
            Assert.AreEqual(1, results.Page, "Page should be 1.");

            foreach (GroupSearchResult result in results)
            {
                Assert.IsNotNull(result.GroupId, "GroupId should not be null.");
                Assert.IsNotNull(result.GroupName, "GroupName should not be null.");
            }
        }

        [Test]
        public async Task GroupsGetInfoBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            GroupFullInfo info = await f.GroupsGetInfoAsync(TestData.GroupId, cancellationToken);

            Assert.IsNotNull(info, "GroupFullInfo should not be null");
            Assert.AreEqual(TestData.GroupId, info.GroupId);
            Assert.AreEqual("The Lake District UK", info.GroupName);

            Assert.AreEqual("5128", info.IconServer);
            Assert.AreEqual("6", info.IconFarm);

            Assert.AreEqual("https://farm6.staticflickr.com/5128/buddyicons/53837206@N00.jpg", info.GroupIconUrl);

            Assert.AreEqual(2, info.ThrottleInfo.Count);
            Assert.AreEqual(GroupThrottleMode.PerDay, info.ThrottleInfo.Mode);

            Assert.IsTrue(info.Restrictions.PhotosAccepted, "PhotosAccepted should be true.");
            Assert.IsFalse(info.Restrictions.VideosAccepted, "VideosAccepted should be false.");
        }

        [Test]
        public async Task GroupsGetInfoNoGroupIconTest(CancellationToken cancellationToken = default)
        {
            string groupId = "562176@N20";
            Flickr f = Instance;

            GroupFullInfo info = await f.GroupsGetInfoAsync(groupId, cancellationToken);

            Assert.IsNotNull(info, "GroupFullInfo should not be null");
            Assert.AreEqual("0", info.IconServer, "Icon Server should be zero");
            Assert.AreEqual("https://www.flickr.com/images/buddyicon.jpg", info.GroupIconUrl);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsMembersGetListBasicTest(CancellationToken cancellationToken = default)
        {
            var ms = await AuthInstance.GroupsMembersGetListAsync(TestData.GroupId, cancellationToken);

            Assert.IsNotNull(ms);
            Assert.AreNotEqual(0, ms.Count, "Count should not be zero.");
            Assert.AreNotEqual(0, ms.Total, "Total should not be zero.");
            Assert.AreEqual(1, ms.Page, "Page should be one.");
            Assert.AreNotEqual(0, ms.PerPage, "PerPage should not be zero.");
            Assert.AreNotEqual(0, ms.Pages, "Pages should not be zero.");
        }
    }
}