using FlickrNet.CollectionModels;
using FlickrNet.Models;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    [TestFixture]
    public class MachinetagsTests : BaseTest
    {
        [Test]
        public async Task MachinetagsGetNamespacesBasicTest(CancellationToken cancellationToken = default)
        {
            NamespaceCollection col = await Instance.MachineTagsGetNamespacesAsync(cancellationToken);

            Assert.IsTrue(col.Count > 10, "Should be greater than 10 namespaces.");

            foreach (var n in col)
            {
                Assert.IsNotNull(n.NamespaceName, "NamespaceName should not be null.");
                Assert.AreNotEqual(0, n.Predicates, "Predicates should not be zero.");
                Assert.AreNotEqual(0, n.Usage, "Usage should not be zero.");
            }
        }

        [Test]
        public async Task MachinetagsGetPredicatesBasicTest(CancellationToken cancellationToken = default)
        {
            var col = await Instance.MachineTagsGetPredicatesAsync(cancellationToken);

            Assert.IsTrue(col.Count > 10, "Should be greater than 10 namespaces.");

            foreach (var n in col)
            {
                Assert.IsNotNull(n.PredicateName, "PredicateName should not be null.");
                Assert.AreNotEqual(0, n.Namespaces, "Namespaces should not be zero.");
                Assert.AreNotEqual(0, n.Usage, "Usage should not be zero.");
            }
        }

        [Test]
        public async Task MachinetagsGetPairsBasicTest(CancellationToken cancellationToken = default)
        {
            var pairs = await Instance.MachineTagsGetPairsAsync(null, null, 0, 0, cancellationToken);
            Assert.IsNotNull(pairs);

            Assert.AreNotEqual(0, pairs.Count, "Count should not be zero.");

            foreach (Pair p in pairs)
            {
                Assert.IsNotNull(p.NamespaceName, "NamespaceName should not be null.");
                Assert.IsNotNull(p.PairName, "PairName should not be null.");
                Assert.IsNotNull(p.PredicateName, "PredicateName should not be null.");
                Assert.AreNotEqual(0, p.Usage, "Usage should be greater than zero.");
            }
        }

        [Test]
        public async Task MachinetagsGetPairsNamespaceTest(CancellationToken cancellationToken = default)
        {
            var pairs = await Instance.MachineTagsGetPairsAsync("dc", null, 0, 0, cancellationToken);
            Assert.IsNotNull(pairs);

            Assert.AreNotEqual(0, pairs.Count, "Count should not be zero.");

            foreach (Pair p in pairs)
            {
                Assert.AreEqual("dc", p.NamespaceName, "NamespaceName should be 'dc'.");
                Assert.IsNotNull(p.PairName, "PairName should not be null.");
                Assert.IsTrue(p.PairName.StartsWith("dc:", StringComparison.Ordinal), "PairName should start with 'dc:'.");
                Assert.IsNotNull(p.PredicateName, "PredicateName should not be null.");
                Assert.AreNotEqual(0, p.Usage, "Usage should be greater than zero.");
            }
        }

        [Test]
        public async Task MachinetagsGetPairsPredicateTest(CancellationToken cancellationToken = default)
        {
            var pairs = await Instance.MachineTagsGetPairsAsync(null, "author", 0, 0, cancellationToken);
            Assert.IsNotNull(pairs);

            Assert.AreNotEqual(0, pairs.Count, "Count should not be zero.");

            foreach (Pair p in pairs)
            {
                Assert.AreEqual("author", p.PredicateName, "PredicateName should be 'dc'.");
                Assert.IsNotNull(p.PairName, "PairName should not be null.");
                Assert.IsTrue(p.PairName.EndsWith(":author", StringComparison.Ordinal), "PairName should end with ':author'.");
                Assert.IsNotNull(p.NamespaceName, "NamespaceName should not be null.");
                Assert.AreNotEqual(0, p.Usage, "Usage should be greater than zero.");
            }
        }

        [Test]
        public async Task MachinetagsGetPairsDcAuthorTest(CancellationToken cancellationToken = default)
        {
            var pairs = await Instance.MachineTagsGetPairsAsync("dc", "author", 0, 0, cancellationToken);
            Assert.IsNotNull(pairs);

            Assert.AreEqual(1, pairs.Count, "Count should be 1.");

            foreach (Pair p in pairs)
            {
                Assert.AreEqual("author", p.PredicateName, "PredicateName should be 'author'.");
                Assert.AreEqual("dc", p.NamespaceName, "NamespaceName should be 'dc'.");
                Assert.AreEqual("dc:author", p.PairName, "PairName should be 'dc:author'.");
                Assert.AreNotEqual(0, p.Usage, "Usage should be greater than zero.");
            }
        }

        [Test]
        public async Task MachinetagsGetValuesTest(CancellationToken cancellationToken = default)
        {
            var items = await Instance.MachineTagsGetValuesAsync("dc", "author", cancellationToken);
            Assert.IsNotNull(items);

            Assert.AreNotEqual(0, items.Count, "Count should be not be zero.");
            Assert.AreEqual("dc", items.NamespaceName);
            Assert.AreEqual("author", items.PredicateName);

            foreach (var item in items)
            {
                Assert.AreEqual("author", item.PredicateName, "PredicateName should be 'author'.");
                Assert.AreEqual("dc", item.NamespaceName, "NamespaceName should be 'dc'.");
                Assert.IsNotNull(item.ValueText, "ValueText should not be null.");
                Assert.AreNotEqual(0, item.Usage, "Usage should be greater than zero.");
            }
        }

        [Test]
        [Ignore("This method is throwing a Not Available error at the moment.")]
        public async Task MachinetagsGetRecentValuesTest(CancellationToken cancellationToken = default)
        {
            var items = await Instance.MachineTagsGetRecentValuesAsync(DateTime.Now.AddHours(-5), cancellationToken);
            Assert.IsNotNull(items);

            Assert.AreNotEqual(0, items.Count, "Count should be not be zero.");

            foreach (var item in items)
            {
                Assert.IsNotNull(item.NamespaceName, "NamespaceName should not be null.");
                Assert.IsNotNull(item.PredicateName, "PredicateName should not be null.");
                Assert.IsNotNull(item.ValueText, "ValueText should not be null.");
                Assert.IsNotNull(item.DateFirstAdded, "DateFirstAdded should not be null.");
                Assert.IsNotNull(item.DateLastUsed, "DateLastUsed should not be null.");
                Assert.AreNotEqual(0, item.Usage, "Usage should be greater than zero.");
            }
        }
    }
}