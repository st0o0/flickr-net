using NUnit.Framework;
using Shouldly;
using System;

namespace FlickrNetTest
{
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class PhotosGetContactsPhotos : BaseTest
    {
        [Test]
        public void PhotosGetContactsPhotosSignatureRequiredTest()
        {
            Should.Throw<SignatureRequiredException>(() => Instance.PhotosGetContactsPhotos());
        }

        [Test]
        public void PhotosGetContactsPhotosIncorrectCountTest()
        {
            Should.Throw<ArgumentOutOfRangeException>(() => AuthInstance.PhotosGetContactsPhotos(51));
        }

        [Test]
        public void PhotosGetContactsPhotosBasicTest()
        {
            PhotoCollection photos = AuthInstance.PhotosGetContactsPhotos(10);

            photos.Count.ShouldBeInRange(9, 10, "Should return 9-10 photos");

        }

        [Test]
        public void PhotosGetContactsPhotosExtrasTest()
        {
            PhotoCollection photos = AuthInstance.PhotosGetContactsPhotos(10, false, false, false, PhotoSearchExtras.All);

            photos.Count.ShouldBeInRange(9, 10, "Should return 9-10 photos");

            foreach (Photo p in photos)
            {
                Assert.IsNotNull(p.OwnerName, "OwnerName should not be null");
                Assert.AreNotEqual(default(DateTime), p.DateTaken, "DateTaken should not be default DateTime");
            }
        }
    }
}
