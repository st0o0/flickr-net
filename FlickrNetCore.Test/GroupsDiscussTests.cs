using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    [TestFixture]
    public class GroupsDiscussTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussRepliesAddTest(CancellationToken cancellationToken = default)
        {
            var topicId = "72157630982877126";
            var message = "Test message reply\n" + DateTime.Now.ToString("o");
            var newMessage = "New Message reply\n" + DateTime.Now.ToString("o");

            TopicReply reply = null;
            TopicReplyCollection topicReplies;
            try
            {
                await AuthInstance.GroupsDiscussRepliesAddAsync(topicId, message, cancellationToken);

                Thread.Sleep(1000);

                topicReplies = await AuthInstance.GroupsDiscussRepliesGetListAsync(topicId, 1, 100, cancellationToken);

                reply = topicReplies.FirstOrDefault(r => r.Message == message);

                Assert.IsNotNull(reply, "Cannot find matching message.");

                await AuthInstance.GroupsDiscussRepliesEditAsync(topicId, reply.ReplyId, newMessage, cancellationToken);

                var reply2 = await AuthInstance.GroupsDiscussRepliesGetInfoAsync(topicId, reply.ReplyId, cancellationToken);

                Assert.AreEqual(newMessage, reply2.Message, "Message should have been updated.");
            }
            finally
            {
                if (reply != null)
                {
                    await AuthInstance.GroupsDiscussRepliesDeleteAsync(topicId, reply.ReplyId, cancellationToken);
                    topicReplies = await AuthInstance.GroupsDiscussRepliesGetListAsync(topicId, 1, 100, cancellationToken);
                    var reply3 = topicReplies.FirstOrDefault(r => r.ReplyId == reply.ReplyId);
                    Assert.IsNull(reply3, "Reply should not exist anymore.");
                }
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussRepliesGetListTest(CancellationToken cancellationToken = default)
        {
            var topics = await AuthInstance.GroupsDiscussTopicsGetListAsync(TestData.GroupId, 1, 100, cancellationToken);

            Assert.IsNotNull(topics, "Topics should not be null.");

            Assert.AreNotEqual(0, topics.Count, "Should be more than one topics return.");

            var firstTopic = topics.First(t => t.RepliesCount > 0);

            var replies = await AuthInstance.GroupsDiscussRepliesGetListAsync(firstTopic.TopicId, 1, 10, cancellationToken);
            Assert.AreEqual(firstTopic.TopicId, replies.TopicId, "TopicId's should be the same.");
            Assert.AreEqual(firstTopic.Subject, replies.Subject, "Subject's should be the same.");
            Assert.AreEqual(firstTopic.Message, replies.Message, "Message's should be the same.");
            Assert.AreEqual(firstTopic.DateCreated, replies.DateCreated, "DateCreated's should be the same.");
            Assert.AreEqual(firstTopic.DateLastPost, replies.DateLastPost, "DateLastPost's should be the same.");

            Assert.IsNotNull(replies, "Replies should not be null.");

            var firstReply = replies.First();

            Assert.IsNotNull(firstReply.ReplyId, "ReplyId should not be null.");

            var reply = await AuthInstance.GroupsDiscussRepliesGetInfoAsync(firstTopic.TopicId, firstReply.ReplyId, cancellationToken);
            Assert.IsNotNull(reply, "Reply should not be null.");
            Assert.AreEqual(firstReply.Message, reply.Message, "TopicReply.Message should be the same.");
        }

        [Test]
        [Ignore("Got this working, now ignore as there is no way to delete topics!")]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussTopicsAddTest(CancellationToken cancellationToken = default)
        {
            var groupId = TestData.FlickrNetTestGroupId;

            var subject = "Test subject line: " + DateTime.Now.ToString("o");
            var message = "Subject message line.";

            await AuthInstance.GroupsDiscussTopicsAddAsync(groupId, subject, message, cancellationToken);

            var topics = await AuthInstance.GroupsDiscussTopicsGetListAsync(groupId, 1, 5, cancellationToken);

            var topic = topics.SingleOrDefault(t => t.Subject == subject);

            Assert.IsNotNull(topic, "Unable to find topic with matching subject line.");

            Assert.AreEqual(message, topic.Message, "Message should be the same.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussTopicsGetListTest(CancellationToken cancellationToken = default)
        {
            var topics = await AuthInstance.GroupsDiscussTopicsGetListAsync(TestData.GroupId, 1, 10, cancellationToken);

            Assert.IsNotNull(topics, "Topics should not be null.");

            Assert.AreEqual(TestData.GroupId, topics.GroupId, "GroupId should be the same.");
            Assert.AreNotEqual(0, topics.Count, "Should be more than one topics return.");
            Assert.AreEqual(10, topics.Count, "Count should be 10.");

            foreach (var topic in topics)
            {
                Assert.IsNotNull(topic.TopicId, "TopicId should not be null.");
                Assert.IsNotNull(topic.Subject, "Subject should not be null.");
                Assert.IsNotNull(topic.Message, "Message should not be null.");
            }

            var firstTopic = topics.First();

            var secondTopic = await AuthInstance.GroupsDiscussTopicsGetInfoAsync(firstTopic.TopicId, cancellationToken);
            Assert.AreEqual(firstTopic.TopicId, secondTopic.TopicId, "TopicId's should be the same.");
            Assert.AreEqual(firstTopic.Subject, secondTopic.Subject, "Subject's should be the same.");
            Assert.AreEqual(firstTopic.Message, secondTopic.Message, "Message's should be the same.");
            Assert.AreEqual(firstTopic.DateCreated, secondTopic.DateCreated, "DateCreated's should be the same.");
            Assert.AreEqual(firstTopic.DateLastPost, secondTopic.DateLastPost, "DateLastPost's should be the same.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussTopicsGetListEditableTest(CancellationToken cancellationToken = default)
        {
            var groupId = "51035612836@N01"; // Flickr API group

            var topics = await AuthInstance.GroupsDiscussTopicsGetListAsync(groupId, 1, 20, cancellationToken);

            Assert.AreNotEqual(0, topics.Count);

            foreach (var topic in topics)
            {
                Assert.IsTrue(topic.CanEdit, "CanEdit should be true.");
                if (!topic.IsLocked)
                {
                    Assert.IsTrue(topic.CanReply, "CanReply should be true.");
                }
                Assert.IsTrue(topic.CanDelete, "CanDelete should be true.");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussTopicsGetInfoStickyTest(CancellationToken cancellationToken = default)
        {
            const string topicId = "72157630982967152";
            var topic = await AuthInstance.GroupsDiscussTopicsGetInfoAsync(topicId, cancellationToken);

            Assert.IsTrue(topic.IsSticky, "This topic should be marked as sticky.");
            Assert.IsFalse(topic.IsLocked, "This topic should not be marked as locked.");

            // topic.CanReply should be true, but for some reason isn't, so we cannot test it.
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussTopicsGetInfoLockedTest(CancellationToken cancellationToken = default)
        {
            const string topicId = "72157630982969782";

            var topic = await AuthInstance.GroupsDiscussTopicsGetInfoAsync(topicId, cancellationToken);

            Assert.IsTrue(topic.IsLocked, "This topic should be marked as locked.");
            Assert.IsFalse(topic.IsSticky, "This topic should not be marked as sticky.");

            Assert.IsFalse(topic.CanReply, "CanReply should be false as the topic is locked.");
        }
    }
}