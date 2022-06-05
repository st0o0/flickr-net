using System.Collections.Generic;

using NUnit.Framework;
using FlickrNet;
using System.Threading.Tasks;
using System.Threading;
using FlickrNet.Models;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for TestTests
    /// </summary>
    [TestFixture]
    public class TestTests : BaseTest
    {
        [Test]
        public async Task TestGenericGroupSearch(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            var parameters = new Dictionary<string, string>
            {
                { "text", "Flowers" }
            };

            UnknownResponse response = await f.TestGenericAsync("flickr.groups.search", parameters, cancellationToken);

            Assert.IsNotNull(response, "UnknownResponse should not be null.");
            Assert.IsNotNull(response.ResponseXml, "ResponseXml should not be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task TestGenericTestNull(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            UnknownResponse response = await f.TestGenericAsync("flickr.test.null", null, cancellationToken);

            Assert.IsNotNull(response, "UnknownResponse should not be null.");
            Assert.IsNotNull(response.ResponseXml, "ResponseXml should not be null.");
        }

        [Test]
        public async Task TestEcho(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;
            var parameters = new Dictionary<string, string>
            {
                { "test1", "testvalue" }
            };

            Dictionary<string, string> returns = await f.TestEchoAsync(parameters, cancellationToken);

            Assert.IsNotNull(returns);

            // Was 3, now 11 because of extra oauth parameter used by default.
            Assert.AreEqual(11, returns.Count);

            Assert.AreEqual("flickr.test.echo", returns["method"]);
            Assert.AreEqual("testvalue", returns["test1"]);
        }
    }
}