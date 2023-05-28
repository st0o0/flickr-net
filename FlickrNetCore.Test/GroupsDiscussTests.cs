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
        public async Task GroupsDiscussRepliesAddTest()
        {
            var topicId = "72157630982877126";
            var message = "Test message reply\n" + DateTime.Now.ToString("o");
            var newMessage = "New Message reply\n" + DateTime.Now.ToString("o");

            TopicReply reply = null;
            TopicReplyCollection topicReplies;
            try
            {
                await AuthInstance.GroupsDiscussRepliesAddAsync(topicId, message, default);

                Thread.Sleep(1000);

                topicReplies = await AuthInstance.GroupsDiscussRepliesGetListAsync(topicId, 1, 100, default);

                reply = topicReplies.FirstOrDefault(r => r.Message == message);

                Assert.That(reply, Is.Not.Null, "Cannot find matching message.");

                await AuthInstance.GroupsDiscussRepliesEditAsync(topicId, reply.ReplyId, newMessage, default);

                var reply2 = await AuthInstance.GroupsDiscussRepliesGetInfoAsync(topicId, reply.ReplyId, default);

                Assert.That(reply2.Message, Is.EqualTo(newMessage), "Message should have been updated.");
            }
            finally
            {
                if (reply != null)
                {
                    await AuthInstance.GroupsDiscussRepliesDeleteAsync(topicId, reply.ReplyId, default);
                    topicReplies = await AuthInstance.GroupsDiscussRepliesGetListAsync(topicId, 1, 100, default);
                    var reply3 = topicReplies.FirstOrDefault(r => r.ReplyId == reply.ReplyId);
                    Assert.That(reply3, Is.Null, "Reply should not exist anymore.");
                }
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussRepliesGetListTest()
        {
            var topics = await AuthInstance.GroupsDiscussTopicsGetListAsync(TestData.GroupId, 1, 100, default);

            Assert.That(topics, Is.Not.Null, "Topics should not be null.");

            Assert.That(topics, Is.Not.Empty, "Should be more than one topics return.");

            var firstTopic = topics.First(t => t.RepliesCount > 0);

            var replies = await AuthInstance.GroupsDiscussRepliesGetListAsync(firstTopic.TopicId, 1, 10, default);
            Assert.Multiple(async () =>
            {
                Assert.That(replies.TopicId, Is.EqualTo(firstTopic.TopicId), "TopicId's should be the same.");
                Assert.Multiple(() =>
                {
                    Assert.That(replies.Subject, Is.EqualTo(firstTopic.Subject), "Subject's should be the same.");
                    Assert.That(replies.Message, Is.EqualTo(firstTopic.Message), "Message's should be the same.");
                    Assert.That(replies.DateCreated, Is.EqualTo(firstTopic.DateCreated), "DateCreated's should be the same.");
                    Assert.That(replies.DateLastPost, Is.EqualTo(firstTopic.DateLastPost), "DateLastPost's should be the same.");

                    Assert.That(replies, Is.Not.Null, "Replies should not be null.");
                });
                var firstReply = replies.First();

                Assert.That(firstReply.ReplyId, Is.Not.Null, "ReplyId should not be null.");

                var reply = await AuthInstance.GroupsDiscussRepliesGetInfoAsync(firstTopic.TopicId, firstReply.ReplyId, default);
                Assert.That(reply, Is.Not.Null, "Reply should not be null.");
                Assert.That(reply.Message, Is.EqualTo(firstReply.Message), "TopicReply.Message should be the same.");
            });
        }

        [Test]
        [Ignore("Got this working, now ignore as there is no way to delete topics!")]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussTopicsAddTest()
        {
            var groupId = TestData.FlickrNetTestGroupId;

            var subject = "Test subject line: " + DateTime.Now.ToString("o");
            var message = "Subject message line.";

            await AuthInstance.GroupsDiscussTopicsAddAsync(groupId, subject, message, default);

            var topics = await AuthInstance.GroupsDiscussTopicsGetListAsync(groupId, 1, 5, default);

            var topic = topics.SingleOrDefault(t => t.Subject == subject);

            Assert.That(topic, Is.Not.Null, "Unable to find topic with matching subject line.");

            Assert.That(topic.Message, Is.EqualTo(message), "Message should be the same.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussTopicsGetListTest()
        {
            var topics = await AuthInstance.GroupsDiscussTopicsGetListAsync(TestData.GroupId, 1, 10, default);

            Assert.That(topics, Is.Not.Null, "Topics should not be null.");
            Assert.Multiple(() =>
            {
                Assert.That(topics.GroupId, Is.EqualTo(TestData.GroupId), "GroupId should be the same.");
                Assert.That(topics, Is.Not.Empty, "Should be more than one topics return.");
            });
            Assert.That(topics, Has.Count.EqualTo(10), "Count should be 10.");

            foreach (var topic in topics)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(topic.TopicId, Is.Not.Null, "TopicId should not be null.");
                    Assert.Multiple(() =>
                    {
                        Assert.That(topic.Subject, Is.Not.Null, "Subject should not be null.");
                        Assert.That(topic.Message, Is.Not.Null, "Message should not be null.");
                    });
                });
            }

            var firstTopic = topics.First();

            var secondTopic = await AuthInstance.GroupsDiscussTopicsGetInfoAsync(firstTopic.TopicId, default);
            Assert.Multiple(() =>
            {
                Assert.That(secondTopic.TopicId, Is.EqualTo(firstTopic.TopicId), "TopicId's should be the same.");
                Assert.Multiple(() =>
                {
                    Assert.That(secondTopic.Subject, Is.EqualTo(firstTopic.Subject), "Subject's should be the same.");
                    Assert.That(secondTopic.Message, Is.EqualTo(firstTopic.Message), "Message's should be the same.");
                    Assert.That(secondTopic.DateCreated, Is.EqualTo(firstTopic.DateCreated), "DateCreated's should be the same.");
                    Assert.That(secondTopic.DateLastPost, Is.EqualTo(firstTopic.DateLastPost), "DateLastPost's should be the same.");
                });
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussTopicsGetListEditableTest()
        {
            var groupId = "51035612836@N01"; // Flickr API group

            var topics = await AuthInstance.GroupsDiscussTopicsGetListAsync(groupId, 1, 20, default);

            Assert.That(topics, Is.Not.Empty);

            foreach (var topic in topics)
            {
                Assert.That(topic.CanEdit, Is.True, "CanEdit should be true.");
                if (!topic.IsLocked)
                {
                    Assert.That(topic.CanReply, Is.True, "CanReply should be true.");
                }
                Assert.That(topic.CanDelete, Is.True, "CanDelete should be true.");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussTopicsGetInfoStickyTest()
        {
            const string topicId = "72157630982967152";
            var topic = await AuthInstance.GroupsDiscussTopicsGetInfoAsync(topicId, default);
            Assert.Multiple(() =>
            {
                Assert.That(topic.IsSticky, Is.True, "This topic should be marked as sticky.");
                Assert.That(topic.IsLocked, Is.False, "This topic should not be marked as locked.");
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task GroupsDiscussTopicsGetInfoLockedTest()
        {
            const string topicId = "72157630982969782";

            var topic = await AuthInstance.GroupsDiscussTopicsGetInfoAsync(topicId, default);
            Assert.Multiple(() =>
            {
                Assert.That(topic.IsLocked, Is.True, "This topic should be marked as locked.");
                Assert.That(topic.IsSticky, Is.False, "This topic should not be marked as sticky.");

                Assert.That(topic.CanReply, Is.False, "CanReply should be false as the topic is locked.");
            });
        }
    }
}