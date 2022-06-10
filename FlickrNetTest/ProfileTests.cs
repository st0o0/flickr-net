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
            // TODO: LUL
            //var profile = Instance.ProfileGetProfile(TestData.TestUserId);

            //profile.UserId.ShouldBe(TestData.TestUserId);
        }
    }
}