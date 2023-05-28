﻿using FlickrNetCore.Common;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using System.Xml;

namespace FlickrNetTest.Internals
{
    /// <summary>
    ///This is a test class for FlickrConfigurationSettingsTest and is intended
    ///to contain all FlickrConfigurationSettingsTest Unit Tests
    ///</summary>
    [TestFixture]
    public class FlickrConfigurationSettingsTest : BaseTest
    {
        /// <summary>
        ///A test for FlickrConfigurationSettings Constructor
        ///</summary>
        [Test]
        public void FlickrConfigurationSettingsConstructorTest()
        {
            const string xml = "<flickrNet apiKey=\"apikey\" secret=\"secret\" token=\"thetoken\" " +
                               "cacheDisabled=\"true\" cacheSize=\"1024\" cacheTimeout=\"01:00:00\" " +
                               "cacheLocation=\"testlocation\" service=\"flickr\">"
                               + "<proxy ipaddress=\"localhost\" port=\"8800\" username=\"testusername\" " +
                               "password=\"testpassword\" domain=\"testdomain\"/>"
                               + "</flickrNet>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var configNode = doc.SelectSingleNode("flickrNet");
            var target = new FlickrConfigurationSettings(configNode);

            Assert.That(target.ApiKey, Is.EqualTo("apikey"));
            Assert.That(target.SharedSecret, Is.EqualTo("secret"));
            Assert.That(target.ApiToken, Is.EqualTo("thetoken"));
            Assert.That(target.CacheDisabled, Is.True);
            Assert.That(target.CacheSize, Is.EqualTo(1024));
            Assert.That(target.CacheTimeout, Is.EqualTo(new TimeSpan(1, 0, 0)));
            Assert.That(target.CacheLocation, Is.EqualTo("testlocation"));
        }
    }
}
