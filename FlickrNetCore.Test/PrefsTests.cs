using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

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

            Assert.That(s, Is.Not.Null);
            Assert.That(s, Is.Not.EqualTo(ContentType.None));
        }

        [Test]
        public async Task PrefsGetGeoPermsTest(CancellationToken cancellationToken = default)
        {
            var p = await AuthInstance.PrefsGetGeoPermsAsync(cancellationToken);

            Assert.That(p, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(p.ImportGeoExif, Is.True);
                Assert.That(p.GeoPermissions, Is.EqualTo(GeoPermissionType.Public));
            });
        }

        [Test]
        public async Task PrefsGetHiddenTest(CancellationToken cancellationToken = default)
        {
            var s = await AuthInstance.PrefsGetHiddenAsync(cancellationToken);

            Assert.That(s, Is.Not.Null);
            Assert.That(s, Is.Not.EqualTo(HiddenFromSearch.None));
        }

        [Test]
        public async Task PrefsGetPrivacyTest(CancellationToken cancellationToken = default)
        {
            var p = await AuthInstance.PrefsGetPrivacyAsync(cancellationToken);

            Assert.That(p, Is.Not.Null);
            Assert.That(p, Is.EqualTo(PrivacyFilter.PublicPhotos));
        }

        [Test]
        public async Task PrefsGetSafetyLevelTest(CancellationToken cancellationToken = default)
        {
            var s = await AuthInstance.PrefsGetSafetyLevelAsync(cancellationToken);

            Assert.That(s, Is.Not.Null);
            Assert.That(s, Is.EqualTo(SafetyLevel.Safe));
        }
    }
}