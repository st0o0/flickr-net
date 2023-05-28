using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PandaGetListTest
    /// </summary>
    [TestFixture]
    public class PandaTest : BaseTest
    {
        [Test]
        public async Task PandaGetListBasicTest()
        {
            string[] pandas = await Instance.PandaGetListAsync();

            Assert.That(pandas, Is.Not.Null, "Should return string array");
            Assert.That(pandas.Length > 0, Is.True, "Should not return empty array");
            Assert.Multiple(() =>
            {
                Assert.That(pandas[0], Is.EqualTo("ling ling"));
                Assert.Multiple(() =>
                {
                    Assert.That(pandas[1], Is.EqualTo("hsing hsing"));
                    Assert.That(pandas[2], Is.EqualTo("wang wang"));
                });
            });
        }

        [Test]
        public async Task PandaGetPhotosLingLingTest()
        {
            var photos = await Instance.PandaGetPhotosAsync("ling ling");

            Assert.That(photos, Is.Not.Null, "PandaPhotos should not be null.");
            Assert.Multiple(() =>
            {
                Assert.That(photos.Total, Is.EqualTo(photos.Count), "PandaPhotos.Count should equal PandaPhotos.Total.");
                Assert.That(photos.PandaName, Is.EqualTo("ling ling"), "PandaPhotos.Panda should be 'ling ling'");
            });
        }
    }
}