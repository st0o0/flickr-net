using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using Shouldly;

namespace FlickrNetTest
{
    [TestFixture]
    public class ProfileTests : BaseTest
    {
        [Test]
        public async Task GetDefaultProfile(CancellationToken cancellationToken = default)
        {
            var profile = await Instance.ProfileGetProfileAsync(TestData.TestUserId);

            profile.UserId.ShouldBe(TestData.TestUserId);
        }
    }
}