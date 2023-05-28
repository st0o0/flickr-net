using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for TestTests
    /// </summary>
    [TestFixture]
    public class TestTests : BaseTest
    {
        [Test]
        public async Task TestGenericGroupSearch()
        {
            Flickr f = Instance;

            var parameters = new Dictionary<string, string>
            {
                { "text", "Flowers" }
            };

            UnknownResponse response = await f.TestGenericAsync("flickr.groups.search", parameters);

            Assert.That(response, Is.Not.Null, "UnknownResponse should not be null.");
            Assert.That(response.ResponseXml, Is.Not.Null, "ResponseXml should not be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task TestGenericTestNull()
        {
            Flickr f = AuthInstance;

            UnknownResponse response = await f.TestGenericAsync("flickr.test.null", null);

            Assert.That(response, Is.Not.Null, "UnknownResponse should not be null.");
            Assert.That(response.ResponseXml, Is.Not.Null, "ResponseXml should not be null.");
        }

        [Test]
        public async Task TestEcho()
        {
            Flickr f = Instance;
            var parameters = new Dictionary<string, string>
            {
                { "test1", "testvalue" }
            };

            Dictionary<string, string> returns = await f.TestEchoAsync(parameters);

            Assert.That(returns, Is.Not.Null);

            // Was 3, now 11 because of extra oauth parameter used by default.
            Assert.That(returns, Has.Count.EqualTo(11));
            Assert.Multiple(() =>
            {
                Assert.That(returns["method"], Is.EqualTo("flickr.test.echo"));
                Assert.That(returns["test1"], Is.EqualTo("testvalue"));
            });
        }
    }
}