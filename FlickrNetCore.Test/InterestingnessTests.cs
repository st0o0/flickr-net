using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    public class InterestingnessTests : BaseTest
    {
        [Test]
        public async Task InterestingnessGetListTestsBasicTest()
        {
            DateTime date = DateTime.Today.AddDays(-2);
            DateTime datePlusOne = date.AddDays(1);

            PhotoCollection photos = await Instance.InterestingnessGetListAsync(date, PhotoSearchExtras.All, 1, 100);

            Assert.That(photos, Is.Not.Null, "Photos should not be null.");

            Assert.That(photos.Count > 50 && photos.Count <= 100, Is.True, "Count should be at least 50, but not more than 100.");
        }
    }
}