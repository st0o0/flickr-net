using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for CollectionGetTreeTest
    /// </summary>
    [TestFixture]
    public class CollectionTests : BaseTest
    {
        [Test()]
        [Category("AccessTokenRequired")]
        public async Task CollectionGetInfoBasicTest()
        {
            string id = "78188-72157618817175751";

            Flickr f = AuthInstance;

            CollectionInfo info = await f.CollectionsGetInfoAsync(id, default);
            Assert.Multiple(() =>
            {
                Assert.That(info.CollectionId, Is.EqualTo(id), "CollectionId should be correct.");
                Assert.Multiple(() =>
            {
                Assert.That(info.ChildCount, Is.EqualTo(1), "ChildCount should be 1.");
                Assert.That(info.Title, Is.EqualTo("Global Collection"), "Title should be 'Global Collection'.");
                Assert.That(info.Description, Is.EqualTo("My global collection."), "Description should be set correctly.");
                Assert.That(info.Server, Is.EqualTo("3629"), "Server should be 3629.");

                Assert.That(info.IconPhotos, Has.Count.EqualTo(12), "IconPhotos.Length should be 12.");
            });
                Assert.That(info.IconPhotos[0].Title, Is.EqualTo("Tires"), "The first IconPhoto Title should be 'Tires'.");
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task CollectionGetTreeRootTest()
        {
            Flickr f = AuthInstance;
            CollectionCollection tree = await f.CollectionsGetTreeAsync(default);

            Assert.That(tree, Is.Not.Null, "CollectionList should not be null.");
            Assert.That(tree, Is.Not.Empty, "CollectionList.Count should not be zero.");

            foreach (Collection coll in tree)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(coll.CollectionId, Is.Not.Null, "CollectionId should not be null.");
                    Assert.Multiple(() =>
                {
                    Assert.That(coll.Title, Is.Not.Null, "Title should not be null.");
                    Assert.That(coll.Description, Is.Not.Null, "Description should not be null.");
                    Assert.That(coll.IconSmall, Is.Not.Null, "IconSmall should not be null.");
                    Assert.That(coll.IconLarge, Is.Not.Null, "IconLarge should not be null.");
                });
                    Assert.That(coll.Sets.Count + coll.Collections.Count, Is.Not.EqualTo(0), "Should be either some sets or some collections.");

                    foreach (CollectionSet set in coll.Sets)
                    {
                        Assert.That(set.SetId, Is.Not.Null, "SetId should not be null.");
                    }
                });
            }
        }

        [Test]
        public async Task CollectionGetTreeRootForSpecificUser()
        {
            Flickr f = Instance;
            CollectionCollection tree = await f.CollectionsGetTreeAsync(null, TestData.TestUserId);

            Assert.That(tree, Is.Not.Null, "CollectionList should not be null.");
            Assert.That(tree, Is.Not.Empty, "CollectionList.Count should not be zero.");

            foreach (Collection coll in tree)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(coll.CollectionId, Is.Not.Null, "CollectionId should not be null.");
                    Assert.Multiple(() =>
                    {
                        Assert.That(coll.Title, Is.Not.Null, "Title should not be null.");
                        Assert.That(coll.Description, Is.Not.Null, "Description should not be null.");
                        Assert.That(coll.IconSmall, Is.Not.Null, "IconSmall should not be null.");
                        Assert.That(coll.IconLarge, Is.Not.Null, "IconLarge should not be null.");
                    });
                    Assert.That(coll.Sets.Count + coll.Collections.Count, Is.Not.EqualTo(0), "Should be either some sets or some collections.");

                    foreach (CollectionSet set in coll.Sets)
                    {
                        Assert.That(set.SetId, Is.Not.Null, "SetId should not be null.");
                    }
                });
            }
        }

        [Test]
        public async Task CollectionGetSubTreeForSpecificUser()
        {
            string id = "78188-72157618817175751";
            Flickr f = Instance;
            CollectionCollection tree = await f.CollectionsGetTreeAsync(id, TestData.TestUserId);

            Assert.That(tree, Is.Not.Null, "CollectionList should not be null.");
            Assert.That(tree, Is.Not.Empty, "CollectionList.Count should not be zero.");

            foreach (Collection coll in tree)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(coll.CollectionId, Is.Not.Null, "CollectionId should not be null.");
                    Assert.Multiple(() =>
                {
                    Assert.That(coll.Title, Is.Not.Null, "Title should not be null.");
                    Assert.That(coll.Description, Is.Not.Null, "Description should not be null.");
                    Assert.That(coll.IconSmall, Is.Not.Null, "IconSmall should not be null.");
                    Assert.That(coll.IconLarge, Is.Not.Null, "IconLarge should not be null.");
                });
                    Assert.That(coll.Sets.Count + coll.Collections.Count, Is.Not.EqualTo(0), "Should be either some sets or some collections.");

                    foreach (CollectionSet set in coll.Sets)
                    {
                        Assert.That(set.SetId, Is.Not.Null, "SetId should not be null.");
                    }
                });
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task CollectionsEditMetaTest()
        {
            string id = "78188-72157618817175751";

            Flickr.CacheDisabled = true;
            Flickr f = AuthInstance;

            CollectionInfo info = await f.CollectionsGetInfoAsync(id);

            await f.CollectionsEditMetaAsync(id, info.Title, info.Description + "TEST");

            var info2 = await f.CollectionsGetInfoAsync(id);

            Assert.That(info2.Description, Is.Not.EqualTo(info.Description));

            // Revert description
            await f.CollectionsEditMetaAsync(id, info.Title, info.Description);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task CollectionsEmptyCollection()
        {
            Flickr f = AuthInstance;

            // Get global collection
            CollectionCollection collections = await f.CollectionsGetTreeAsync("78188-72157618817175751", null);

            Assert.That(collections, Is.Not.Null);
            Assert.That(collections.Count > 0, Is.True, "Global collection should be greater than zero.");

            var col = collections[0];
            Assert.Multiple(() =>
            {
                Assert.That(col.Title, Is.EqualTo("Global Collection"), "Global Collection title should be correct.");

                Assert.That(col.Collections, Is.Not.Null, "Child collections property should not be null.");
            });
            Assert.That(col.Collections.Count > 0, Is.True, "Global collection should have child collections.");

            var subsol = col.Collections[0];

            Assert.That(subsol.Collections, Is.Not.Null, "Child collection Collections property should ne null.");
            Assert.That(subsol.Collections.Count, Is.EqualTo(0), "Child collection should not have and sub collections.");
        }
    }
}