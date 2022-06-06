using FlickrNet;
using FlickrNet.CollectionModels;
using FlickrNet.Models;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for BlogTests
    /// </summary>
    [TestFixture]
    public class BlogTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task BlogsGetListTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            BlogCollection blogs = await f.BlogsGetListAsync(cancellationToken);

            Assert.IsNotNull(blogs, "Blogs should not be null.");

            foreach (Blog blog in blogs)
            {
                Assert.IsNotNull(blog.BlogId, "BlogId should not be null.");
                Assert.IsNotNull(blog.NeedsPassword, "NeedsPassword should not be null.");
                Assert.IsNotNull(blog.BlogName, "BlogName should not be null.");
                Assert.IsNotNull(blog.BlogUrl, "BlogUrl should not be null.");
                Assert.IsNotNull(blog.Service, "Service should not be null.");
            }
        }

        [Test]
        public async Task BlogGetServicesTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            BlogServiceCollection services = await f.BlogsGetServicesAsync(cancellationToken);

            Assert.IsNotNull(services, "BlogServices should not be null.");
            Assert.AreNotEqual(0, services.Count, "BlogServices.Count should not be zero.");

            foreach (BlogService service in services)
            {
                Assert.IsNotNull(service.Id, "BlogService.Id should not be null.");
                Assert.IsNotNull(service.Name, "BlogService.Name should not be null.");
            }

            Assert.AreEqual("beta.blogger.com", services[0].Id, "First ID should be beta.blogger.com.");
            Assert.AreEqual("Blogger", services[0].Name, "First Name should be beta.blogger.com.");
        }
    }
}