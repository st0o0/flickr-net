using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    public class PushTests : BaseTest
    {
        [Test]
        public async Task GetTopicsTest(CancellationToken cancellationToken = default)
        {
            var f = Instance;

            var topics = await f.PushGetTopicsAsync(cancellationToken);

            Assert.IsNotNull(topics);
            Assert.AreNotEqual(0, topics.Length, "Should return greater than zero topics.");

            Assert.IsTrue(topics.Contains("contacts_photos"), "Should include \"contacts_photos\".");
            Assert.IsTrue(topics.Contains("contacts_faves"), "Should include \"contacts_faves\".");
            Assert.IsTrue(topics.Contains("geotagged"), "Should include \"geotagged\".");
            Assert.IsTrue(topics.Contains("airports"), "Should include \"airports\".");
        }

        [Test]
        [Category("AccessTokenRequired")]
        [Ignore("Wackylabs called is broken.")]
        public async Task SubscribeUnsubscribeTest(CancellationToken cancellationToken = default)
        {
            var callback = "http://www.wackylabs.net/dev/push/test.php";
            var topic = "contacts_photos";
            var lease = 0;
            var verify = "sync";

            var f = AuthInstance;
            await f.PushSubscribeAsync(topic, callback, verify, null, lease, null, null, 0, 0, 0, RadiusUnit.None, GeoAccuracy.None, null, null, cancellationToken);

            var subscriptions = await f.PushGetSubscriptionsAsync(cancellationToken);

            bool found = false;

            foreach (var sub in subscriptions)
            {
                if (sub.Topic == topic && sub.Callback == callback)
                {
                    found = true;
                    break;
                }
            }

            Assert.IsTrue(found, "Should have found subscription.");

            await f.PushUnsubscribeAsync(topic, callback, verify, null, cancellationToken);
        }

        [Test]
        [Category("AccessTokenRequired")]
        [Ignore("Wackylabs called is broken.")]
        public async Task SubscribeTwiceUnsubscribeTest(CancellationToken cancellationToken = default)
        {
            var callback1 = "http://www.wackylabs.net/dev/push/test.php?id=4";
            var callback2 = "http://www.wackylabs.net/dev/push/test.php?id=5";
            var topic = "contacts_photos";
            var lease = 0;
            var verify = "sync";

            var f = AuthInstance;
            await f.PushSubscribeAsync(topic, callback1, verify, null, lease, null, null, 0, 0, 0, RadiusUnit.None, GeoAccuracy.None, null, null, cancellationToken);
            await f.PushSubscribeAsync(topic, callback2, verify, null, lease, null, null, 0, 0, 0, RadiusUnit.None, GeoAccuracy.None, null, null, cancellationToken);

            var subscriptions = await f.PushGetSubscriptionsAsync(cancellationToken);

            try
            {
                Assert.IsTrue(subscriptions.Count > 1, "Should be more than one subscription.");
            }
            finally
            {
                await f.PushUnsubscribeAsync(topic, callback1, verify, null, cancellationToken);
                await f.PushUnsubscribeAsync(topic, callback2, verify, null, cancellationToken);
            }
        }
    }
}