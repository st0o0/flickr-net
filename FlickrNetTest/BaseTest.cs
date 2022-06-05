using System;
using System.Collections.Generic;
using FlickrNet;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace FlickrNetTest
{
    [TestFixture]
    public class BaseTest
    {
        private Flickr _instance;
        private Flickr _authInstance;
        private Dictionary<string, string> _errorLog;

        private static int _testCount;

        protected Flickr Instance
        {
            get => _instance ??= TestData.GetInstance();
        }

        protected Flickr AuthInstance
        {
            get => _authInstance ??= TestData.GetAuthInstance();
        }

        protected bool InstanceUsed
        {
            get => _instance != null;
        }

        protected bool AuthInstanceUsed
        {
            get => _authInstance != null;
        }

        [SetUp]
        public void InitialiseLoggingAndFlickr()
        {
            _instance = null;
            _authInstance = null;
            _errorLog = new Dictionary<string, string>();
            _testCount += 1;
        }

        protected void LogOnError(string key, string information)
        {
            _errorLog.Add(key, information);
        }

        [TearDown]
        public void ErrorLogging()
        {
            if ((_testCount % 10) > 0)
            {
                System.Threading.Thread.Sleep(200);
            }

            if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed)
            {
                return;
            }

            if (InstanceUsed)
            {
                Console.WriteLine("LastRequest: " + _instance.LastRequest);
                Console.WriteLine("LastResponse: " + _instance.LastResponse);
            }
            if (AuthInstanceUsed)
            {
                Console.WriteLine("LastRequest (Auth): " + _authInstance.LastRequest);
                Console.WriteLine("LastResponse (Auth): " + _authInstance.LastResponse);
            }

            foreach (var line in _errorLog)
            {
                Console.WriteLine(line.Key + ": " + line.Value);
            }
        }
    }
}