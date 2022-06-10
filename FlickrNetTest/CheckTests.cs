using FlickrNet;
using FlickrNet.Exceptions;
using NUnit.Framework;
using Shouldly;

#pragma warning disable CS0618 // Type or member is obsolete

namespace FlickrNetTest
{
    [TestFixture]
    public class CheckTests : BaseTest
    {
        [Test]
        public void CheckApiKeyThrowsExceptionWhenNotPresent()
        {
            Flickr f = new("");

            Should.Throw<ApiKeyRequiredException>(() => f.CheckApiKey());
        }

        [Test]
        public void CheckApiKeyDoesNotThrowWhenPresent()
        {
            Flickr f = new("")
            {
                ApiKey = "X"
            };

            Should.NotThrow(() => f.CheckApiKey());
        }

        [Test]
        public void CheckSignatureKeyThrowsExceptionWhenSecretNotPresent()
        {
            Flickr f = new("")
            {
                ApiKey = "X"
            };
            Should.Throw<SignatureRequiredException>(() => f.CheckSigned());
        }

        [Test]
        public void CheckSignatureKeyDoesntThrowWhenSecretPresent()
        {
            Flickr f = new("")
            {
                ApiKey = "X",
                ApiSecret = "Y"
            };

            Should.NotThrow(() => f.CheckSigned());
        }

        [Test]
        public void CheckRequestAuthenticationThrowsExceptionWhenNothingPresent()
        {
            Flickr f = new("")
            {
                ApiKey = "X",
                ApiSecret = "Y"
            };

            Should.Throw<AuthenticationRequiredException>(() => f.CheckRequiresAuthentication());
        }

        [Test]
        public void CheckRequestAuthenticationDoesNotThrowWhenAuthTokenPresent()
        {
            Flickr f = new("")
            {
                ApiKey = "X",
                ApiSecret = "Y",
            };

            Assert.DoesNotThrow(f.CheckRequiresAuthentication);
        }

        [Test]
        public void CheckRequestAuthenticationDoesNotThrowWhenOAuthTokenPresent()
        {
            Flickr f = new("")
            {
                ApiKey = "X",
                ApiSecret = "Y",

                OAuthAccessToken = "Z1",
                OAuthAccessTokenSecret = "Z2"
            };

            Assert.DoesNotThrow(f.CheckRequiresAuthentication);
        }
    }
}