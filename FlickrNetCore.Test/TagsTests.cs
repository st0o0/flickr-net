using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Exceptions;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using Shouldly;

namespace FlickrNetTest
{
    [TestFixture]
    public class TagsTests : BaseTest
    {
        public TagsTests()
        {
            Flickr.CacheDisabled = true;
        }

        [Test]
        public void TagsGetListUserRawAuthenticationTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;
            Should.Throw<SignatureRequiredException>(async () => await f.TagsGetListUserRawAsync(cancellationToken));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task TagsGetListUserRawBasicTest(CancellationToken cancellationToken = default)
        {
            var tags = await AuthInstance.TagsGetListUserRawAsync(cancellationToken);

            Assert.AreNotEqual(0, tags.Count, "There should be one or more raw tags returned");

            foreach (RawTag tag in tags)
            {
                Assert.IsNotNull(tag.CleanTag, "Clean tag should not be null");
                Assert.IsTrue(tag.CleanTag.Length > 0, "Clean tag should not be empty string");
                Assert.IsTrue(tag.RawTags.Count > 0, "Should be one or more raw tag for each clean tag");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task TagsGetListUserPopularBasicTest(CancellationToken cancellationToken = default)
        {
            TagCollection tags = await AuthInstance.TagsGetListUserPopularAsync(cancellationToken);

            Assert.IsNotNull(tags, "TagCollection should not be null.");
            Assert.AreNotEqual(0, tags.Count, "TagCollection.Count should not be zero.");

            foreach (Tag tag in tags)
            {
                Assert.IsNotNull(tag.TagName, "Tag.TagName should not be null.");
                Assert.AreNotEqual(0, tag.Count, "Tag.Count should not be zero.");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task TagsGetListUserBasicTest(CancellationToken cancellationToken = default)
        {
            TagCollection tags = await AuthInstance.TagsGetListUserAsync(cancellationToken);

            Assert.IsNotNull(tags, "TagCollection should not be null.");
            Assert.AreNotEqual(0, tags.Count, "TagCollection.Count should not be zero.");

            foreach (Tag tag in tags)
            {
                Assert.IsNotNull(tag.TagName, "Tag.TagName should not be null.");
                Assert.AreEqual(0, tag.Count, "Tag.Count should be zero. Not ser for this method.");
            }
        }

        [Test]
        public async Task TagsGetListPhotoBasicTest(CancellationToken cancellationToken = default)
        {
            var tags = await Instance.TagsGetListPhotoAsync(TestData.PhotoId, cancellationToken);

            Assert.IsNotNull(tags, "tags should not be null.");
            Assert.AreNotEqual(0, tags.Count, "Length should be greater than zero.");

            foreach (var tag in tags)
            {
                Assert.IsNotNull(tag.TagId, "TagId should not be null.");
                Assert.IsNotNull(tag.TagText, "TagText should not be null.");
                Assert.IsNotNull(tag.Raw, "Raw should not be null.");
                Assert.IsNotNull(tag.IsMachineTag, "IsMachineTag should not be null.");
            }
        }

        [Test]
        public async Task TagsGetClustersNewcastleTest(CancellationToken cancellationToken = default)
        {
            var col = await Instance.TagsGetClustersAsync("newcastle", cancellationToken);

            Assert.IsNotNull(col);

            Assert.AreEqual(4, col.Count, "Count should be four.");
            Assert.AreEqual(col.TotalClusters, col.Count);
            Assert.AreEqual("newcastle", col.SourceTag);

            Assert.AreEqual("water-ocean-clouds", col[0].ClusterId);

            foreach (var c in col)
            {
                Assert.AreNotEqual(0, c.TotalTags, "TotalTags should not be zero.");
                Assert.IsNotNull(c.Tags, "Tags should not be null.");
                Assert.IsTrue(c.Tags.Count >= 3);
                Assert.IsNotNull(c.ClusterId);
            }
        }

        [Test]
        public async Task TagsGetClusterPhotosNewcastleTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;
            var col = await f.TagsGetClustersAsync("newcastle", cancellationToken);

            foreach (var c in col)
            {
                var ps = await f.TagsGetClusterPhotosAsync(c, cancellationToken);
                Assert.IsNotNull(ps);
                Assert.AreNotEqual(0, ps.Count);
            }
        }

        [Test]
        public async Task TagsGetHotListTest(CancellationToken cancellationToken = default)
        {
            var col = await Instance.TagsGetHotListAsync(cancellationToken);

            Assert.AreNotEqual(0, col.Count, "Count should not be zero.");

            foreach (var c in col)
            {
                Assert.IsNotNull(c);
                Assert.IsNotNull(c.Tag);
                Assert.AreNotEqual(0, c.Score);
            }
        }

        [Test]
        public async Task TagsGetListUserTest(CancellationToken cancellationToken = default)
        {
            var col = await Instance.TagsGetListUserAsync(TestData.TestUserId, cancellationToken);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task TagsGetMostFrequentlyUsedTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var tags = await f.TagsGetMostFrequentlyUsedAsync(cancellationToken);

            Assert.IsNotNull(tags);

            Assert.AreNotEqual(0, tags.Count);
        }
    }
}