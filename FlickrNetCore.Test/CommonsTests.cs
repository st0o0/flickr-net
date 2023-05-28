using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for CommonsTests
    /// </summary>
    [TestFixture]
    public class CommonsTests : BaseTest
    {
        [Test]
        public async Task CommonsGetInstitutions()
        {
            InstitutionCollection insts = await Instance.CommonsGetInstitutionsAsync(default);

            Assert.That(insts, Is.Not.Null);
            Assert.That(insts, Has.Count.GreaterThan(5));

            foreach (var i in insts)
            {
                Assert.That(i, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(i.InstitutionId, Is.Not.Null);
                    Assert.That(i.InstitutionName, Is.Not.Null);
                });
            }
        }
    }
}