using FlickrNet;
using System;

namespace FlickrNetTest
{
    public static class TestData
    {
        public const string ApiKey = "dbc316af64fb77dae9140de64262da0a";
        public const string SharedSecret = "0781969a058a2745";

        // https://www.flickr.com/photos/samjudson/3547139066 - Apple Store
        public const string PhotoId = "3547139066";

        // https://www.flickr.com/photos/samjudson/5890800 - Grey Street
        public const string FavouritedPhotoId = "5890800";

        // FLOWERS
        public const string GroupId = "53837206@N00";

        // Test user is Sam Judson (i.e. Me)
        public const string TestUserId = "41888973@N00";

        public const string TestImageBase64 = "/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAYEBQYFBAYGBQYHBwYIChAKCgkJChQODwwQFxQYGBcUFhYaHSUfGhsjHBYWICwgIyYnKSopGR8tMC0oMCUoKSj/2wBDAQcHBwoIChMKChMoGhYaKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCj/wAARCAAgACADASIAAhEBAxEB/8QAGQAAAwEBAQAAAAAAAAAAAAAAAQYHCAME/8QAMRAAAQQBAgMFBQkAAAAAAAAAAQIDBBEFABITFCEGBzFR0UFVcZOUFSMkUmGEkaHw/8QAFgEBAQEAAAAAAAAAAAAAAAAAAgEE/8QAHBEAAgMBAAMAAAAAAAAAAAAAAQIAAwQSERNB/9oADAMBAAIRAxEAPwDT/Os2QOKaJSSlpZFg0eoGjzrX5Xvkr9NSzvhyGTgsYj7KnSYhW/J4nAcKNwChV141Z/nSTBzHaByuLmckf3C/XW2nC1qdgiY7NYrfgiaI55nye+Sv00OeY3JFuAqISLaUOpND2eeoajOy2R+IzM8nyElwn+jpp7A5heSyDqOZmPIRsJ461KF8RFEWT+upbjetSx+RV6ldgoh70HorK8VzkSZKQVytqYoBUDvT1Ng9NIgyEELKHMXlwk9UANXYoePT46ucvEGUoCSzj5CUrWpHGZ3lO42av/dNec9mo/u/D/SDRr1vWoVfkNuX2MW8yJSMth4rZW7i8qlF7bKBQ8vZpz7uH2Hsm4Y8OTFoJsP1avvEeFaeD2Yje78P9KNdouDEVwKjMY9i1JKiyzsJAINdPhpW7HsXkiSrJ6268z//2Q==";

        public static byte[] TestImageBytes
        {
            get
            {
                return Convert.FromBase64String(TestImageBase64);
            }
        }

        public const string FlickrNetTestGroupId = "1368041@N20";

        public static string AuthToken
        {
            get { return GetEnvironmentVariable("AuthToken"); }
            set { SetEnvironmentVariable("AuthToken", value); }
        }

        public static string RequestToken
        {
            get { return GetEnvironmentVariable("RequestToken"); }
            set { SetEnvironmentVariable("RequestToken", value); }
        }

        public static string RequestTokenSecret
        {
            get { return GetEnvironmentVariable("RequestTokenSecret"); }
            set { SetEnvironmentVariable("RequestTokenSecret", value); }
        }

        public static string AccessToken
        {
            get { return GetEnvironmentVariable("AccessToken"); }
            set { SetEnvironmentVariable("AccessToken", value); }
        }

        public static string AccessTokenSecret
        {
            get { return GetEnvironmentVariable("AccessTokenSecret"); }
            set { SetEnvironmentVariable("AccessTokenSecret", value); }
        }

        private static void SetEnvironmentVariable(string name, string value)
        {
            Environment.SetEnvironmentVariable("FLICKR_TEST_" + name.ToUpper(), value);
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable("FLICKR_TEST_" + name.ToUpper());
        }

        public static Flickr GetInstance()
        {
            return new Flickr(ApiKey) { InstanceCacheDisabled = true };
        }

        public static Flickr GetSignedInstance()
        {
            return new Flickr(ApiKey, SharedSecret) { InstanceCacheDisabled = true };
        }

        public static Flickr GetAuthInstance()
        {
            return new Flickr(ApiKey, SharedSecret)
            {
                InstanceCacheDisabled = true,
                OAuthAccessToken = AccessToken,
                OAuthAccessTokenSecret = AccessTokenSecret
            };
        }
    }
}