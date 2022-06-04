using FlickrNet;
using FlickrNet.CollectionModels;
using FlickrNet.Enums;
using FlickrNet.Exceptions;
using FlickrNet.Models;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for OAuthTests
    /// </summary>
    [TestFixture]
    public class OAuthTests : BaseTest
    {
        [Test]
        [Ignore("Use this to generate a require token. Then add verifier to second test")]
        public async Task OAuthGetRequestTokenBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = TestData.GetSignedInstance();

            OAuthRequestToken requestToken = await f.OAuthGetRequestTokenAsync("oob", cancellationToken);

            Assert.IsNotNull(requestToken);
            Assert.IsNotNull(requestToken.Token, "Token should not be null.");
            Assert.IsNotNull(requestToken.TokenSecret, "TokenSecret should not be null.");

            System.Diagnostics.Process.Start(f.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Delete));

            Console.WriteLine("token = " + requestToken.Token);
            Console.WriteLine("token secret = " + requestToken.TokenSecret);

            TestData.RequestToken = requestToken.Token;
            TestData.RequestTokenSecret = requestToken.TokenSecret;
        }

        [Test]
        [Ignore("Use this to generate an access token. Substitute the verifier from above test prior to running")]
        public async Task OAuthGetAccessTokenBasicTest(CancellationToken cancellationToken = default)
        {
            Flickr f = TestData.GetSignedInstance();

            var requestToken = new OAuthRequestToken
            {
                Token = TestData.RequestToken,
                TokenSecret = TestData.RequestTokenSecret
            };
            string verifier = "846-116-116";

            OAuthAccessToken accessToken = await f.OAuthGetAccessTokenAsync(requestToken, verifier, cancellationToken);

            Console.WriteLine("access token = " + accessToken.Token);
            Console.WriteLine("access token secret = " + accessToken.TokenSecret);

            TestData.AccessToken = accessToken.Token;
            TestData.AccessTokenSecret = accessToken.TokenSecret;
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task OAuthPeopleGetPhotosBasicTest(CancellationToken cancellationToken = default)
        {
            PhotoCollection photos = await AuthInstance.PeopleGetPhotosAsync("me", cancellationToken);
        }

        [Test]
        public async Task OAuthInvalidAccessTokenTest(CancellationToken cancellationToken = default)
        {
            Instance.ApiSecret = "asdasd";

            Should.Throw<OAuthException>(async () => { await Instance.OAuthGetRequestTokenAsync("oob", cancellationToken); });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task OAuthCheckTokenTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            Auth a = await f.AuthOAuthCheckTokenAsync(cancellationToken);

            Assert.AreEqual(a.Token, f.OAuthAccessToken);
        }

        [Test]
        public void OAuthCheckEncoding()
        {
            // Test cases taken from OAuth spec
            // http://wiki.oauth.net/w/page/12238556/TestCases
            var test = new Dictionary<string, string>()
            {
                { "abcABC123", "abcABC123" },
                { "-._~", "-._~"},
                {"%", "%25"},
                { "+", "%2B"},
                { "&=*", "%26%3D%2A"},
                { "", ""},
                { "\u000A", "%0A"},
                { "\u0020", "%20"},
                { "\u007F", "%7F"},
                { "\u0080", "%C2%80"},
                { "\u3001", "%E3%80%81"},
                { "$()", "%24%28%29"}
            };

            foreach (var pair in test)
            {
                Assert.AreEqual(pair.Value, UtilityMethods.EscapeOAuthString(pair.Key));
            }
        }
    }
}