using FlickrNet;
using FlickrNet.CollectionModels;
using FlickrNet.Models;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for CollectionGetTreeTest
    /// </summary>
    [TestFixture]
    public class CollectionTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task CollectionGetInfoBasicTest(CancellationToken cancellation = default)
        {
            string id = "78188-72157618817175751";

            Flickr f = AuthInstance;

            CollectionInfo info = await f.CollectionsGetInfoAsync(id, cancellation);

            Assert.AreEqual(id, info.CollectionId, "CollectionId should be correct.");
            Assert.AreEqual(1, info.ChildCount, "ChildCount should be 1.");
            Assert.AreEqual("Global Collection", info.Title, "Title should be 'Global Collection'.");
            Assert.AreEqual("My global collection.", info.Description, "Description should be set correctly.");
            Assert.AreEqual("3629", info.Server, "Server should be 3629.");

            Assert.AreEqual(12, info.IconPhotos.Count, "IconPhotos.Length should be 12.");

            Assert.AreEqual("Tires", info.IconPhotos[0].Title, "The first IconPhoto Title should be 'Tires'.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task CollectionGetTreeRootTest(CancellationToken cancellation = default)
        {
            Flickr f = AuthInstance;
            CollectionCollection tree = await f.CollectionsGetTreeAsync(cancellation);

            Assert.IsNotNull(tree, "CollectionList should not be null.");
            Assert.AreNotEqual(0, tree.Count, "CollectionList.Count should not be zero.");

            foreach (Collection coll in tree)
            {
                Assert.IsNotNull(coll.CollectionId, "CollectionId should not be null.");
                Assert.IsNotNull(coll.Title, "Title should not be null.");
                Assert.IsNotNull(coll.Description, "Description should not be null.");
                Assert.IsNotNull(coll.IconSmall, "IconSmall should not be null.");
                Assert.IsNotNull(coll.IconLarge, "IconLarge should not be null.");

                Assert.AreNotEqual(0, coll.Sets.Count + coll.Collections.Count, "Should be either some sets or some collections.");

                foreach (CollectionSet set in coll.Sets)
                {
                    Assert.IsNotNull(set.SetId, "SetId should not be null.");
                }
            }
        }

        [Test]
        public async Task CollectionGetTreeRootForSpecificUser(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;
            CollectionCollection tree = await f.CollectionsGetTreeAsync(null, TestData.TestUserId, cancellationToken);

            Assert.IsNotNull(tree, "CollectionList should not be null.");
            Assert.AreNotEqual(0, tree.Count, "CollectionList.Count should not be zero.");

            foreach (Collection coll in tree)
            {
                Assert.IsNotNull(coll.CollectionId, "CollectionId should not be null.");
                Assert.IsNotNull(coll.Title, "Title should not be null.");
                Assert.IsNotNull(coll.Description, "Description should not be null.");
                Assert.IsNotNull(coll.IconSmall, "IconSmall should not be null.");
                Assert.IsNotNull(coll.IconLarge, "IconLarge should not be null.");

                Assert.AreNotEqual(0, coll.Sets.Count + coll.Collections.Count, "Should be either some sets or some collections.");

                foreach (CollectionSet set in coll.Sets)
                {
                    Assert.IsNotNull(set.SetId, "SetId should not be null.");
                }
            }
        }

        [Test]
        public async Task CollectionGetSubTreeForSpecificUser(CancellationToken cancellationToken = default)
        {
            string id = "78188-72157618817175751";
            Flickr f = Instance;
            CollectionCollection tree = await f.CollectionsGetTreeAsync(id, TestData.TestUserId, cancellationToken);

            Assert.IsNotNull(tree, "CollectionList should not be null.");
            Assert.AreNotEqual(0, tree.Count, "CollectionList.Count should not be zero.");

            foreach (Collection coll in tree)
            {
                Assert.IsNotNull(coll.CollectionId, "CollectionId should not be null.");
                Assert.IsNotNull(coll.Title, "Title should not be null.");
                Assert.IsNotNull(coll.Description, "Description should not be null.");
                Assert.IsNotNull(coll.IconSmall, "IconSmall should not be null.");
                Assert.IsNotNull(coll.IconLarge, "IconLarge should not be null.");

                Assert.AreNotEqual(0, coll.Sets.Count + coll.Collections.Count, "Should be either some sets or some collections.");

                foreach (CollectionSet set in coll.Sets)
                {
                    Assert.IsNotNull(set.SetId, "SetId should not be null.");
                }
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task CollectionsEditMetaTest(CancellationToken cancellationToken = default)
        {
            string id = "78188-72157618817175751";

            Flickr.CacheDisabled = true;
            Flickr f = AuthInstance;

            CollectionInfo info = await f.CollectionsGetInfoAsync(id, cancellationToken);

            await f.CollectionsEditMetaAsync(id, info.Title, info.Description + "TEST", cancellationToken);

            var info2 = await f.CollectionsGetInfoAsync(id, cancellationToken);

            Assert.AreNotEqual(info.Description, info2.Description);

            // Revert description
            await f.CollectionsEditMetaAsync(id, info.Title, info.Description, cancellationToken);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task CollectionsEmptyCollection(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            // Get global collection
            CollectionCollection collections = await f.CollectionsGetTreeAsync("78188-72157618817175751", null, cancellationToken);

            Assert.IsNotNull(collections);
            Assert.IsTrue(collections.Count > 0, "Global collection should be greater than zero.");

            var col = collections[0];

            Assert.AreEqual("Global Collection", col.Title, "Global Collection title should be correct.");

            Assert.IsNotNull(col.Collections, "Child collections property should not be null.");
            Assert.IsTrue(col.Collections.Count > 0, "Global collection should have child collections.");

            var subsol = col.Collections[0];

            Assert.IsNotNull(subsol.Collections, "Child collection Collections property should ne null.");
            Assert.AreEqual(0, subsol.Collections.Count, "Child collection should not have and sub collections.");
        }
    }
}