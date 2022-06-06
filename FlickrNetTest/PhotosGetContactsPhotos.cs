using FlickrNet.CollectionModels;
using FlickrNet.Enums;
using FlickrNet.Exceptions;
using FlickrNet.Models;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class PhotosGetContactsPhotos : BaseTest
    {
        [Test]
        public async Task PhotosGetContactsPhotosSignatureRequiredTest(CancellationToken cancellationToken = default)
        {
            Should.Throw<SignatureRequiredException>(async () => await Instance.PhotosGetContactsPhotosAsync(cancellationToken));
        }

        [Test]
        public async Task PhotosGetContactsPhotosIncorrectCountTest(CancellationToken cancellationToken = default)
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