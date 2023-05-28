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
        public void TagsGetListUserRawAuthenticationTest()
        {
            Flickr f = Instance;
            Should.Throw<SignatureRequiredException>(async () => await f.TagsGetListUserRawAsync());
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task TagsGetListUserRawBasicTest()
        {
            var tags = await AuthInstance.TagsGetListUserRawAsync();

            Assert.That(tags, Is.Not.Empty, "There should be one or more raw tags returned");

            foreach (RawTag tag in tags)
            {
                Assert.That(tag.CleanTag, Is.Not.Null, "Clean tag should not be null");
                Assert.Multiple(() =>
                {
                    Assert.That(tag.CleanTag.Length > 0, Is.True, "Clean tag should not be empty string");
                    Assert.That(tag.RawTags.Count > 0, Is.True, "Should be one or more raw tag for each clean tag");
                });
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task TagsGetListUserPopularBasicTest()
        {
            TagCollection tags = await AuthInstance.TagsGetListUserPopularAsync();

            Assert.That(tags, Is.Not.Null, "TagCollection should not be null.");
            Assert.That(tags, Is.Not.Empty, "TagCollection.Count should not be zero.");

            foreach (Tag tag in tags)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(tag.TagName, Is.Not.Null, "Tag.TagName should not be null.");
                    Assert.That(tag.Count, Is.Not.EqualTo(0), "Tag.Count should not be zero.");
                });
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task TagsGetListUserBasicTest()
        {
            TagCollection tags = await AuthInstance.TagsGetListUserAsync();

            Assert.That(tags, Is.Not.Null, "TagCollection should not be null.");
            Assert.That(tags, Is.Not.Empty, "TagCollection.Count should not be zero.");

            foreach (Tag tag in tags)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(tag.TagName, Is.Not.Null, "Tag.TagName should not be null.");
                    Assert.That(tag.Count, Is.EqualTo(0), "Tag.Count should be zero. Not ser for this method.");
                });
            }
        }

        [Test]
        public async Task TagsGetListPhotoBasicTest()
        {
            var tags = await Instance.TagsGetListPhotoAsync(TestData.PhotoId);

            Assert.That(tags, Is.Not.Null, "tags should not be null.");
            Assert.That(tags, Is.Not.Empty, "Length should be greater than zero.");

            foreach (var tag in tags)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(tag.TagId, Is.Not.Null, "TagId should not be null.");
                    Assert.Multiple(() =>
                {
                    Assert.That(tag.TagText, Is.Not.Null, "TagText should not be null.");
                    Assert.That(tag.Raw, Is.Not.Null, "Raw should not be null.");
                });
                });
            }
        }

        [Test]
        public async Task TagsGetClustersNewcastleTest()
        {
            var col = await Instance.TagsGetClustersAsync("newcastle");

            Assert.That(col, Is.Not.Null);

            Assert.That(col, Has.Count.EqualTo(4), "Count should be four.");
            Assert.That(col, Has.Count.EqualTo(col.TotalClusters));
            Assert.Multiple(() =>
            {
                Assert.That(col.SourceTag, Is.EqualTo("newcastle"));

                Assert.That(col[0].ClusterId, Is.EqualTo("water-ocean-clouds"));
            });
            foreach (var c in col)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(c.TotalTags, Is.Not.EqualTo(0), "TotalTags should not be zero.");
                    Assert.That(c.Tags, Is.Not.Null, "Tags should not be null.");
                });
                Assert.Multiple(() =>
                {
                    Assert.That(c.Tags, Has.Count.GreaterThanOrEqualTo(3));
                    Assert.That(c.ClusterId, Is.Not.Null);
                });
            }
        }

        [Test]
        public async Task TagsGetClusterPhotosNewcastleTest()
        {
            Flickr f = Instance;
            var col = await f.TagsGetClustersAsync("newcastle");

            foreach (var c in col)
            {
                var ps = await f.TagsGetClusterPhotosAsync(c);
                Assert.That(ps, Is.Not.Null);
                Assert.That(ps, Is.Not.Empty);
            }
        }

        [Test]
        public async Task TagsGetHotListTest()
        {
            var col = await Instance.TagsGetHotListAsync();

            Assert.That(col, Is.Not.Empty, "Count should not be zero.");

            foreach (var c in col)
            {
                Assert.That(c, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(c.Tag, Is.Not.Null);
                    Assert.That(c.Score, Is.Not.EqualTo(0));
                });
            }
        }

        [Test]
        public async Task TagsGetListUserTest()
        {
            var col = await Instance.TagsGetListUserAsync(TestData.TestUserId);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task TagsGetMostFrequentlyUsedTest()
        {
            Flickr f = AuthInstance;

            var tags = await f.TagsGetMostFrequentlyUsedAsync();

            Assert.That(tags, Is.Not.Null);

            Assert.That(tags, Is.Not.Empty);
        }
    }
}