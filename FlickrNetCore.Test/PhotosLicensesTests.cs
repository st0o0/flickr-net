using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    public class PhotosLicensesTests : BaseTest
    {
        [Test]
        public async Task PhotosLicensesGetInfoBasicTest(CancellationToken cancellationToken = default)
        {
            LicenseCollection col = await Instance.PhotosLicensesGetInfoAsync(cancellationToken);

            foreach (License lic in col)
            {
                if (!Enum.IsDefined(typeof(LicenseType), lic.LicenseId))
                {
                    Assert.Fail("License with ID " + (int)lic.LicenseId + ", " + lic.LicenseName + " dooes not exist.");
                }
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosLicensesSetLicenseTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;
            string photoId = "7176125763";

            var photoInfo = await f.PhotosGetInfoAsync(photoId, cancellationToken); // Rainbow Rose
            var origLicense = photoInfo.License;

            var newLicense = origLicense == LicenseType.AttributionCC ? LicenseType.AttributionNoDerivativesCC : LicenseType.AttributionCC;
            await f.PhotosLicensesSetLicenseAsync(photoId, newLicense, cancellationToken);

            var newPhotoInfo = await f.PhotosGetInfoAsync(photoId, cancellationToken);

            Assert.AreEqual(newLicense, newPhotoInfo.License, "License has not changed");

            // Reset license
            await f.PhotosLicensesSetLicenseAsync(photoId, origLicense, cancellationToken);
        }
    }
}