using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PandaGetListTest
    /// </summary>
    [TestFixture]
    public class PandaTest : BaseTest
    {
        [Test]
        public async Task PandaGetListBasicTest(CancellationToken cancellationToken = default)
        {
            string[] pandas = await Instance.PandaGetListAsync(cancellationToken);

            Assert.IsNotNull(pandas, "Should return string array");
            Assert.IsTrue(pandas.Length > 0, "Should not return empty array");

            Assert.AreEqual("ling ling", pandas[0]);
            Assert.AreEqual("hsing hsing", pandas[1]);
            Assert.AreEqual("wang wang", pandas[2]);
        }

        [Test]
        public async Task PandaGetPhotosLingLingTest(CancellationToken cancellationToken = default)
        {
            var photos = await Instance.PandaGetPhotosAsync("ling ling", cancellationToken);

            Assert.IsNotNull(photos, "PandaPhotos should not be null.");
            Assert.AreEqual(photos.Count, photos.Total, "PandaPhotos.Count should equal PandaPhotos.Total.");
            Assert.AreEqual("ling ling", photos.PandaName, "PandaPhotos.Panda should be 'ling ling'");
        }
    }
}