using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    public class PushTests : BaseTest
    {
        [Test]
        public async Task GetTopicsTest()
        {
            var f = Instance;

            var topics = await f.PushGetTopicsAsync();

            Assert.That(topics, Is.Not.Null);
            Assert.That(topics, Is.Not.Empty, "Should return greater than zero topics.");
            Assert.Multiple(() =>
            {
                Assert.That(topics.Contains("contacts_photos"), Is.True, "Should include \"contacts_photos\".");
                Assert.Multiple(() =>
            {
                Assert.That(topics.Contains("contacts_faves"), Is.True, "Should include \"contacts_faves\".");
                Assert.That(topics.Contains("geotagged"), Is.True, "Should include \"geotagged\".");
                Assert.That(topics.Contains("airports"), Is.True, "Should include \"airports\".");
            });
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        [Ignore("Wackylabs called is broken.")]
        public async Task SubscribeUnsubscribeTest()
        {
            var callback = "http://www.wackylabs.net/dev/push/test.php";
            var topic = "contacts_photos";
            var lease = 0;
            var verify = "sync";

            var f = AuthInstance;
            await f.PushSubscribeAsync(topic, callback, verify, null, lease, null, null, 0, 0, 0, RadiusUnit.None, GeoAccuracy.None, null, null);

            var subscriptions = await f.PushGetSubscriptionsAsync();

            bool found = false;

            foreach (var sub in subscriptions)
            {
                if (sub.Topic == topic && sub.Callback == callback)
                {
                    found = true;
                    break;
                }
            }

            Assert.That(found, Is.True, "Should have found subscription.");

            await f.PushUnsubscribeAsync(topic, callback, verify, null);
        }

        [Test]
        [Category("AccessTokenRequired")]
        [Ignore("Wackylabs called is broken.")]
        public async Task SubscribeTwiceUnsubscribeTest()
        {
            var callback1 = "http://www.wackylabs.net/dev/push/test.php?id=4";
            var callback2 = "http://www.wackylabs.net/dev/push/test.php?id=5";
            var topic = "contacts_photos";
            var lease = 0;
            var verify = "sync";

            var f = AuthInstance;
            await f.PushSubscribeAsync(topic, callback1, verify, null, lease, null, null, 0, 0, 0, RadiusUnit.None, GeoAccuracy.None, null, null);
            await f.PushSubscribeAsync(topic, callback2, verify, null, lease, null, null, 0, 0, 0, RadiusUnit.None, GeoAccuracy.None, null, null);

            var subscriptions = await f.PushGetSubscriptionsAsync();

            try
            {
                Assert.That(subscriptions.Count > 1, Is.True, "Should be more than one subscription.");
            }
            finally
            {
                await f.PushUnsubscribeAsync(topic, callback1, verify, null);
                await f.PushUnsubscribeAsync(topic, callback2, verify, null);
            }
        }
    }
}