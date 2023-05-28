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
        public async Task CommonsGetInstitutions(CancellationToken cancellationToken = default)
        {
            InstitutionCollection insts = await Instance.CommonsGetInstitutionsAsync(cancellationToken);

            Assert.IsNotNull(insts);
            Assert.IsTrue(insts.Count > 5);

            foreach (var i in insts)
            {
                Assert.IsNotNull(i);
                Assert.IsNotNull(i.InstitutionId);
                Assert.IsNotNull(i.InstitutionName);
            }
        }
    }
}