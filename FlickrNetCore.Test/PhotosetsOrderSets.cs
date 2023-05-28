using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class PhotosetsOrderSets : BaseTest
    {
        [Test]
        public async Task PhotosetsOrderSetsStringTest()
        {
            var mySets = await AuthInstance.PhotosetsGetListAsync();

            await AuthInstance.PhotosetsOrderSetsAsync(string.Join(",", mySets.Select(myset => myset.PhotosetId).ToArray()));
        }

        [Test]
        public async Task PhotosetsOrderSetsArrayTest()
        {
            var mySets = await AuthInstance.PhotosetsGetListAsync();

            await AuthInstance.PhotosetsOrderSetsAsync(mySets.Select(myset => myset.PhotosetId).ToArray());
        }
    }
}