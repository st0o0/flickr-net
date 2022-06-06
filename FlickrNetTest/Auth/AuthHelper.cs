using FlickrNet;
using NUnit.Framework;
using Shouldly;
using System;

#pragma warning disable CS0618 // Type or member is obsolete

namespace FlickrNetTest
{
    [TestFixture]
    public class AuthHelper
    {
        [Test]
        public void CheckEnvironmentVariableForAccessToken()
        {
            var value = Environment.GetEnvironmentVariable("FLICKR_TEST_ACCESSTOKEN");
            value.ShouldNotBeNullOrEmpty();
        }

        /// <summary>
        /// This method will authenticate the current user, and then store the AuthToken in the 
        /// </summary>
        [Test]
        [Ignore("Use this to generate a new aut token if required")]
        public void AuthHelperMethod()
        {
            Flickr f = TestData.GetOldSignedInstance();

            string frob = f.AuthGetFrob();

            Assert.IsNotNull(frob, "Frob should not be null.");

            string url = f.AuthCalcUrl(frob, AuthLevel.Delete);

            Assert.IsNotNull(url, "url should not be null.");

            System.Diagnostics.Process.Start(url);

            // Auth flickr in next 30 seconds

            System.Threading.Thread.Sleep(1000 * 30);

            Auth auth = f.AuthGetToken(frob);

            TestData.AuthToken = auth.Token;

            Console.WriteLine(TestData.AuthToken);
        }
    }
}
