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
        public void PhotosGetContactsPhotosSignatureRequiredTest(CancellationToken cancellationToken = default)
        {
            Should.Throw<SignatureRequiredException>(async () => await Instance.PhotosGetContactsPhotosAsync(cancellationToken));
        }

        [Test]
        public void PhotosGetContactsPhotosIncorrectCountTest(CancellationToken cancellationToken = default)
        {
            Should.Throw<ArgumentOutOfRangeException>(async () => await AuthInstance.PhotosGetContactsPhotosAsync(51, cancellationToken));
        }

        [Test]
        public async Task PhotosGetContactsPhotosBasicTest(CancellationToken cancellationToken = default)
        {
            PhotoCollection photos = await AuthInstance.PhotosGetContactsPhotosAsync(10, cancellationToken);

            photos.Count.ShouldBeInRange(9, 10, "Should return 9-10 photos");
        }

        [Test]
        public async Task PhotosGetContactsPhotosExtrasTest(CancellationToken cancellationToken = default)
        {
            PhotoCollection photos = await AuthInstance.PhotosGetContactsPhotosAsync(10, false, false, false, PhotoSearchExtras.All, cancellationToken);

            photos.Count.ShouldBeInRange(9, 10, "Should return 9-10 photos");

            foreach (Photo p in photos)
            {
                Assert.IsNotNull(p.OwnerName, "OwnerName should not be null");
                Assert.AreNotEqual(default(DateTime), p.DateTaken, "DateTaken should not be default DateTime");
            }
        }
    }
}