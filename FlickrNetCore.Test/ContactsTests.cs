using FlickrNetCore;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for ContactsTests
    /// </summary>
    [TestFixture]
    public class ContactsTests : BaseTest
    {
        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetListTestBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;
            ContactCollection contacts = await f.ContactsGetListAsync(cancellationToken);

            Assert.IsNotNull(contacts);

            foreach (var contact in contacts)
            {
                Assert.IsNotNull(contact.UserId, "UserId should not be null.");
                Assert.IsNotNull(contact.UserName, "UserName should not be null.");
                Assert.IsNotNull(contact.BuddyIconUrl, "BuddyIconUrl should not be null.");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetListFullParamTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            ContactCollection contacts = await f.ContactsGetListAsync(null, 0, 0, cancellationToken);

            Assert.IsNotNull(contacts, "Contacts should not be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetListFilteredTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;
            ContactCollection contacts = await f.ContactsGetListAsync("friends", cancellationToken);

            Assert.IsNotNull(contacts);

            foreach (var contact in contacts)
            {
                Assert.IsNotNull(contact.UserId, "UserId should not be null.");
                Assert.IsNotNull(contact.UserName, "UserName should not be null.");
                Assert.IsNotNull(contact.BuddyIconUrl, "BuddyIconUrl should not be null.");
                Assert.IsNotNull(contact.IsFriend, "IsFriend should not be null.");
                Assert.IsTrue(contact.IsFriend.Value);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetListPagedTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;
            ContactCollection contacts = await f.ContactsGetListAsync(2, 20, cancellationToken);

            Assert.IsNotNull(contacts);
            Assert.AreEqual(2, contacts.Page);
            Assert.AreEqual(20, contacts.PerPage);
            Assert.AreEqual(20, contacts.Count);

            foreach (var contact in contacts)
            {
                Assert.IsNotNull(contact.UserId, "UserId should not be null.");
                Assert.IsNotNull(contact.UserName, "UserName should not be null.");
                Assert.IsNotNull(contact.BuddyIconUrl, "BuddyIconUrl should not be null.");
            }
        }

        [Test]
        public async Task ContactsGetPublicListTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            ContactCollection contacts = await f.ContactsGetPublicListAsync(TestData.TestUserId, cancellationToken);

            Assert.IsNotNull(contacts, "Contacts should not be null.");

            Assert.AreNotEqual(0, contacts.Total, "Total should not be zero.");
            Assert.AreNotEqual(0, contacts.PerPage, "PerPage should not be zero.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetRecentlyUpdatedTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            ContactCollection contacts = await f.ContactsGetListRecentlyUploadedAsync(DateTime.Now.AddDays(-1), null, cancellationToken);

            Assert.IsNotNull(contacts, "Contacts should not be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetTaggingSuggestions(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            var contacts = await f.ContactsGetTaggingSuggestionsAsync(cancellationToken);

            Assert.IsNotNull(contacts);
        }
    }
}