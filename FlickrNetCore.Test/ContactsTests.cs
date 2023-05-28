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
        public async Task ContactsGetListTestBasicTest()
        {
            Flickr f = AuthInstance;
            ContactCollection contacts = await f.ContactsGetListAsync(default);

            Assert.That(contacts, Is.Not.Null);

            foreach (var contact in contacts)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(contact.UserId, Is.Not.Null, "UserId should not be null.");
                    Assert.Multiple(() =>
                {
                    Assert.That(contact.UserName, Is.Not.Null, "UserName should not be null.");
                    Assert.That(contact.BuddyIconUrl, Is.Not.Null, "BuddyIconUrl should not be null.");
                });
                });
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetListFullParamTest()
        {
            Flickr f = AuthInstance;

            ContactCollection contacts = await f.ContactsGetListAsync(null, 0, 0, default);

            Assert.That(contacts, Is.Not.Null, "Contacts should not be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetListFilteredTest()
        {
            Flickr f = AuthInstance;
            ContactCollection contacts = await f.ContactsGetListAsync("friends", default);

            Assert.That(contacts, Is.Not.Null);

            foreach (var contact in contacts)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(contact.UserId, Is.Not.Null, "UserId should not be null.");
                    Assert.That(contact.IsFriend.Value, Is.True);
                    Assert.Multiple(() =>
                    {
                        Assert.That(contact.UserName, Is.Not.Null, "UserName should not be null.");
                        Assert.That(contact.BuddyIconUrl, Is.Not.Null, "BuddyIconUrl should not be null.");
                        Assert.That(contact.IsFriend, Is.Not.Null, "IsFriend should not be null.");
                    });
                });
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetListPagedTest()
        {
            Flickr f = AuthInstance;
            ContactCollection contacts = await f.ContactsGetListAsync(2, 20, default);

            Assert.That(contacts, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(contacts.Page, Is.EqualTo(2));
                Assert.That(contacts.PerPage, Is.EqualTo(20));
                Assert.That(contacts, Has.Count.EqualTo(20));
            });
            foreach (var contact in contacts)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(contact.UserId, Is.Not.Null, "UserId should not be null.");
                    Assert.Multiple(() =>
                    {
                        Assert.That(contact.UserName, Is.Not.Null, "UserName should not be null.");
                        Assert.That(contact.BuddyIconUrl, Is.Not.Null, "BuddyIconUrl should not be null.");
                    });
                });
            }
        }

        [Test]
        public async Task ContactsGetPublicListTest()
        {
            Flickr f = Instance;

            ContactCollection contacts = await f.ContactsGetPublicListAsync(TestData.TestUserId, default);

            Assert.That(contacts, Is.Not.Null, "Contacts should not be null.");
            Assert.Multiple(() =>
            {
                Assert.That(contacts.Total, Is.Not.EqualTo(0), "Total should not be zero.");
                Assert.That(contacts.PerPage, Is.Not.EqualTo(0), "PerPage should not be zero.");
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetRecentlyUpdatedTest()
        {
            Flickr f = AuthInstance;

            ContactCollection contacts = await f.ContactsGetListRecentlyUploadedAsync(DateTime.Now.AddDays(-1), null, default);

            Assert.That(contacts, Is.Not.Null, "Contacts should not be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task ContactsGetTaggingSuggestions()
        {
            Flickr f = AuthInstance;

            var contacts = await f.ContactsGetTaggingSuggestionsAsync(default);

            Assert.That(contacts, Is.Not.Null);
        }
    }
}