using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class PhotosetsOrderSets : BaseTest
    {
        [Test]
        public async Task PhotosetsOrderSetsStringTest(CancellationToken cancellationToken = default)
        {
            var mySets = await AuthInstance.PhotosetsGetListAsync(cancellationToken);

            await AuthInstance.PhotosetsOrderSetsAsync(string.Join(",", mySets.Select(myset => myset.PhotosetId).ToArray()), cancellationToken);
        }

        [Test]
        public async Task PhotosetsOrderSetsArrayTest(CancellationToken cancellationToken = default)
        {
            var mySets = await AuthInstance.PhotosetsGetListAsync(cancellationToken);

            await AuthInstance.PhotosetsOrderSetsAsync(mySets.Select(myset => myset.PhotosetId).ToArray(), cancellationToken);
        }
    }
}