using FlickrNet.Enums;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PrefsTest
    /// </summary>
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class PrefsTests : BaseTest
    {
        [Test]
        public async Task PrefsGetContentTypeTest(CancellationToken cancellationToken = default)
        {
            var s = await AuthInstance.PrefsGetContentTypeAsync(cancellationToken);

            Assert.IsNotNull(s);
            Assert.AreNotEqual(ContentType.None, s);
        }

        [Test]
        public async Task PrefsGetGeoPermsTest(CancellationToken cancellationToken = default)
        {
            var p = await AuthInstance.PrefsGetGeoPermsAsync(cancellationToken);

            Assert.IsNotNull(p);
            Assert.IsTrue(p.ImportGeoExif);
            Assert.AreEqual(GeoPermissionType.Public, p.GeoPermissions);
        }

        [Test]
        public async Task PrefsGetHiddenTest(CancellationToken cancellationToken = default)
        {
            var s = await AuthInstance.PrefsGetHiddenAsync(cancellationToken);

            Assert.IsNotNull(s);
            Assert.AreNotEqual(HiddenFromSearch.None, s);
        }

        [Test]
        public async Task PrefsGetPrivacyTest(CancellationToken cancellationToken = default)
        {
            var p = await AuthInstance.PrefsGetPrivacyAsync(cancellationToken);

            Assert.IsNotNull(p);
            Assert.AreEqual(PrivacyFilter.PublicPhotos, p);
        }

        [Test]
        public async Task PrefsGetSafetyLevelTest(CancellationToken cancellationToken = default)
        {
            var s = await AuthInstance.PrefsGetSafetyLevelAsync(cancellationToken);

            Assert.IsNotNull(s);
            Assert.AreEqual(SafetyLevel.Safe, s);
        }
    }
}