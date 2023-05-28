﻿using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    public class CameraTests : BaseTest
    {
        [Test]
        public async Task ShouldReturnListOfCameraBrands()
        {
            var brands = await Instance.CamerasGetBrandsAsync(default);

            Assert.That(brands, Is.Not.Null);
            Assert.That(brands, Is.Not.Empty);
            Assert.Multiple(() =>
            {
                Assert.That(brands.Any(b => b.BrandId == "canon" && b.BrandName == "Canon"), Is.True);
                Assert.That(brands.Any(b => b.BrandId == "nikon" && b.BrandName == "Nikon"), Is.True);
            });
        }

        [Test]
        public async Task ShouldReturnListOfCanonCameraModels()
        {
            var models = await Instance.CamerasGetBrandModelsAsync("canon", default);

            Assert.That(models, Is.Not.Null);
            Assert.That(models, Is.Not.Empty);
            Assert.Multiple(() =>
            {
                Assert.That(models.Any(c => c.CameraId == "eos_5d_mark_ii" && c.CameraName == "Canon EOS 5D Mark II"), Is.True);
                Assert.That(models.Any(c => c.CameraId == "powershot_a620" && c.CameraName == "Canon PowerShot A620"), Is.True);
            });
        }
    }
}