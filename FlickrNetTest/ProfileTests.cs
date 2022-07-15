using NUnit.Framework;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    [TestFixture]
    public class ProfileTests : BaseTest
    {
        [Test]
        public async Task GetDefaultProfile(CancellationToken cancellationToken = default)
        {
            var profile = await Instance.ProfileGetProfileAsync(TestData.TestUserId, cancellationToken);

            profile.UserId.ShouldBe(TestData.TestUserId);
        }
    }
}