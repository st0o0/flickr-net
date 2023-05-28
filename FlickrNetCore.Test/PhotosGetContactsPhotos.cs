using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Enums;
using FlickrNetCore.Exceptions;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using Shouldly;

namespace FlickrNetTest
{
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class PhotosGetContactsPhotos : BaseTest
    {
        [Test]
        public void PhotosGetContactsPhotosSignatureRequiredTest()
        {
            Should.Throw<SignatureRequiredException>(async () => await Instance.PhotosGetContactsPhotosAsync());
        }

        [Test]
        public void PhotosGetContactsPhotosIncorrectCountTest()
        {
            Should.Throw<ArgumentOutOfRangeException>(async () => await AuthInstance.PhotosGetContactsPhotosAsync(51));
        }

        [Test]
        public async Task PhotosGetContactsPhotosBasicTest()
        {
            PhotoCollection photos = await AuthInstance.PhotosGetContactsPhotosAsync(10);

            photos.Count.ShouldBeInRange(9, 10, "Should return 9-10 photos");
        }

        [Test]
        public async Task PhotosGetContactsPhotosExtrasTest()
        {
            PhotoCollection photos = await AuthInstance.PhotosGetContactsPhotosAsync(10, false, false, false, PhotoSearchExtras.All);

            photos.Count.ShouldBeInRange(9, 10, "Should return 9-10 photos");

            foreach (Photo p in photos)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(p.OwnerName, Is.Not.Null, "OwnerName should not be null");
                    Assert.That(p.DateTaken, Is.Not.EqualTo(default(DateTime)), "DateTaken should not be default DateTime");
                });
            }
        }
    }
}