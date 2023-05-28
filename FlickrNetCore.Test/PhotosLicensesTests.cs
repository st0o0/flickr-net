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
        public async Task PhotosLicensesGetInfoBasicTest()
        {
            LicenseCollection col = await Instance.PhotosLicensesGetInfoAsync();

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
        public async Task PhotosLicensesSetLicenseTest()
        {
            Flickr f = AuthInstance;
            string photoId = "7176125763";

            var photoInfo = await f.PhotosGetInfoAsync(photoId); // Rainbow Rose
            var origLicense = photoInfo.License;

            var newLicense = origLicense == LicenseType.AttributionCC ? LicenseType.AttributionNoDerivativesCC : LicenseType.AttributionCC;
            await f.PhotosLicensesSetLicenseAsync(photoId, newLicense);

            var newPhotoInfo = await f.PhotosGetInfoAsync(photoId);

            Assert.That(newPhotoInfo.License, Is.EqualTo(newLicense), "License has not changed");

            // Reset license
            await f.PhotosLicensesSetLicenseAsync(photoId, origLicense);
        }
    }
}