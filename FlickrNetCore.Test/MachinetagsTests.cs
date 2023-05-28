using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    public class MachinetagsTests : BaseTest
    {
        [Test]
        public async Task MachinetagsGetNamespacesBasicTest()
        {
            NamespaceCollection col = await Instance.MachineTagsGetNamespacesAsync();

            Assert.That(col, Has.Count.GreaterThan(10), "Should be greater than 10 namespaces.");

            foreach (var n in col)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(n.NamespaceName, Is.Not.Null, "NamespaceName should not be null.");
                    Assert.Multiple(() =>
                    {
                        Assert.That(n.Predicates, Is.Not.EqualTo(0), "Predicates should not be zero.");
                        Assert.That(n.Usage, Is.Not.EqualTo(0), "Usage should not be zero.");
                    });
                });
            }
        }

        [Test]
        public async Task MachinetagsGetPredicatesBasicTest()
        {
            var col = await Instance.MachineTagsGetPredicatesAsync();

            Assert.That(col, Has.Count.GreaterThan(10), "Should be greater than 10 namespaces.");

            foreach (var n in col)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(n.PredicateName, Is.Not.Null, "PredicateName should not be null.");
                    Assert.Multiple(() =>
                    {
                        Assert.That(n.Namespaces, Is.Not.EqualTo(0), "Namespaces should not be zero.");
                        Assert.That(n.Usage, Is.Not.EqualTo(0), "Usage should not be zero.");
                    });
                });
            }
        }

        [Test]
        public async Task MachinetagsGetPairsBasicTest()
        {
            var pairs = await Instance.MachineTagsGetPairsAsync(null, null, 0, 0);
            Assert.That(pairs, Is.Not.Null);

            Assert.That(pairs, Is.Not.Empty, "Count should not be zero.");

            foreach (Pair p in pairs)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(p.NamespaceName, Is.Not.Null, "NamespaceName should not be null.");
                    Assert.Multiple(() =>
                    {
                        Assert.That(p.PairName, Is.Not.Null, "PairName should not be null.");
                        Assert.That(p.PredicateName, Is.Not.Null, "PredicateName should not be null.");
                    });
                    Assert.That(p.Usage, Is.Not.EqualTo(0), "Usage should be greater than zero.");
                });
            }
        }

        [Test]
        public async Task MachinetagsGetPairsNamespaceTest()
        {
            var pairs = await Instance.MachineTagsGetPairsAsync("dc", null, 0, 0);
            Assert.That(pairs, Is.Not.Null);

            Assert.That(pairs, Is.Not.Empty, "Count should not be zero.");

            foreach (Pair p in pairs)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(p.NamespaceName, Is.EqualTo("dc"), "NamespaceName should be 'dc'.");
                    Assert.That(p.PairName, Is.Not.Null, "PairName should not be null.");
                });
                Assert.Multiple(() =>
                {
                    Assert.That(p.PairName.StartsWith("dc:", StringComparison.Ordinal), Is.True, "PairName should start with 'dc:'.");
                    Assert.That(p.PredicateName, Is.Not.Null, "PredicateName should not be null.");
                    Assert.That(p.Usage, Is.Not.EqualTo(0), "Usage should be greater than zero.");
                });
            }
        }

        [Test]
        public async Task MachinetagsGetPairsPredicateTest()
        {
            var pairs = await Instance.MachineTagsGetPairsAsync(null, "author", 0, 0);
            Assert.That(pairs, Is.Not.Null);

            Assert.That(pairs, Is.Not.Empty, "Count should not be zero.");

            foreach (Pair p in pairs)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(p.PredicateName, Is.EqualTo("author"), "PredicateName should be 'dc'.");
                    Assert.That(p.PairName, Is.Not.Null, "PairName should not be null.");
                });
                Assert.Multiple(() =>
                {
                    Assert.That(p.PairName.EndsWith(":author", StringComparison.Ordinal), Is.True, "PairName should end with ':author'.");
                    Assert.That(p.NamespaceName, Is.Not.Null, "NamespaceName should not be null.");
                    Assert.That(p.Usage, Is.Not.EqualTo(0), "Usage should be greater than zero.");
                });
            }
        }

        [Test]
        public async Task MachinetagsGetPairsDcAuthorTest()
        {
            var pairs = await Instance.MachineTagsGetPairsAsync("dc", "author", 0, 0);
            Assert.That(pairs, Is.Not.Null);

            Assert.That(pairs, Has.Count.EqualTo(1), "Count should be 1.");

            foreach (Pair p in pairs)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(p.PredicateName, Is.EqualTo("author"), "PredicateName should be 'author'.");
                    Assert.That(p.NamespaceName, Is.EqualTo("dc"), "NamespaceName should be 'dc'.");
                    Assert.That(p.PairName, Is.EqualTo("dc:author"), "PairName should be 'dc:author'.");
                    Assert.That(p.Usage, Is.Not.EqualTo(0), "Usage should be greater than zero.");
                });
            }
        }

        [Test]
        public async Task MachinetagsGetValuesTest()
        {
            var items = await Instance.MachineTagsGetValuesAsync("dc", "author");
            Assert.That(items, Is.Not.Null);

            Assert.That(items, Is.Not.Empty, "Count should be not be zero.");
            Assert.Multiple(() =>
            {
                Assert.That(items.NamespaceName, Is.EqualTo("dc"));
                Assert.That(items.PredicateName, Is.EqualTo("author"));
            });
            foreach (var item in items)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(item.PredicateName, Is.EqualTo("author"), "PredicateName should be 'author'.");
                    Assert.That(item.NamespaceName, Is.EqualTo("dc"), "NamespaceName should be 'dc'.");
                    Assert.That(item.ValueText, Is.Not.Null, "ValueText should not be null.");
                    Assert.That(item.Usage, Is.Not.EqualTo(0), "Usage should be greater than zero.");
                });
            }
        }

        [Test]
        [Ignore("This method is throwing a Not Available error at the moment.")]
        public async Task MachinetagsGetRecentValuesTest()
        {
            var items = await Instance.MachineTagsGetRecentValuesAsync(DateTime.Now.AddHours(-5));
            Assert.That(items, Is.Not.Null);

            Assert.That(items, Is.Not.Empty, "Count should be not be zero.");

            foreach (var item in items)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(item.NamespaceName, Is.Not.Null, "NamespaceName should not be null.");
                    Assert.Multiple(() =>
                    {
                        Assert.That(item.PredicateName, Is.Not.Null, "PredicateName should not be null.");
                        Assert.That(item.ValueText, Is.Not.Null, "ValueText should not be null.");
                        Assert.That(item.DateFirstAdded, Is.Not.Null, "DateFirstAdded should not be null.");
                        Assert.That(item.DateLastUsed, Is.Not.Null, "DateLastUsed should not be null.");
                    });
                    Assert.That(item.Usage, Is.Not.EqualTo(0), "Usage should be greater than zero.");
                });
            }
        }
    }
}