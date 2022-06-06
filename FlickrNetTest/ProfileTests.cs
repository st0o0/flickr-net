using NUnit.Framework;
using Shouldly;

namespace FlickrNetTest
{
    [TestFixture]
    public class ProfileTests : BaseTest
    {
        [Test]
        public void GetDefaultProfile()
        {
            var profile = Instance.ProfileGetProfile(TestData.TestUserId);

            profile.UserId.ShouldBe(TestData.TestUserId);
        }
    }
}
