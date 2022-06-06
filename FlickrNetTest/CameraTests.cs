using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    [TestFixture]
    public class CameraTests : BaseTest
    {
        [Test]
        public async Task ShouldReturnListOfCameraBrands(CancellationToken cancellationToken = default)
        {
            var brands = await Instance.CamerasGetBrandsAsync(cancellationToken);

            Assert.IsNotNull((brands));
            Assert.AreNotEqual(0, brands.Count);

            Assert.IsTrue(brands.Any(b => b.CameraId == "canon" && b.CameraName == "Canon"));
            Assert.IsTrue(brands.Any(b => b.CameraId == "nikon" && b.CameraName == "Nikon"));
        }

        [Test]
        public async Task ShouldReturnListOfCanonCameraModels(CancellationToken cancellationToken = default)
        {
            var models = await Instance.CamerasGetBrandModelsAsync("canon", cancellationToken);

            Assert.IsNotNull((models));
            Assert.AreNotEqual(0, models.Count);

            Assert.IsTrue(models.Any(c => c.CameraId == "eos_5d_mark_ii" && c.CameraName == "Canon EOS 5D Mark II"));
            Assert.IsTrue(models.Any(c => c.CameraId == "powershot_a620" && c.CameraName == "Canon PowerShot A620"));
        }
    }
}