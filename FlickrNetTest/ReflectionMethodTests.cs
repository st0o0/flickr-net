using FlickrNet;
using FlickrNet.CollectionModels;
using FlickrNet.SearchOptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for ReflectionMethodTests
    /// </summary>
    [TestFixture]
    public class ReflectionMethodTests : BaseTest
    {
        [Test]
        public async Task ReflectionMethodsBasic(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            MethodCollection methodNames = await f.ReflectionGetMethodsAsync(cancellationToken);

            Assert.IsNotNull(methodNames, "Should not be null");
            Assert.AreNotEqual(0, methodNames.Count, "Should return some method names.");
            Assert.IsNotNull(methodNames[0], "First item should not be null");
        }

        [Test]
        public async Task ReflectionMethodsCheckWeSupport(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            MethodCollection methodNames = await f.ReflectionGetMethodsAsync(cancellationToken);

            Assert.IsNotNull(methodNames, "Should not be null");
            Assert.AreNotEqual(0, methodNames.Count, "Should return some method names.");
            Assert.IsNotNull(methodNames[0], "First item should not be null");

            Type type = typeof(Flickr);
            MethodInfo[] methods = type.GetMethods();

            int failCount = 0;

            foreach (string methodName in methodNames)
            {
                bool found = false;
                string trueName = methodName.Replace("flickr.", "").Replace(".", "").ToLower();
                foreach (MethodInfo info in methods)
                {
                    if (trueName == info.Name.ToLower())
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    failCount++;
                    Console.WriteLine("Method '" + methodName + "' not found in FlickrNet.Flickr.");
                }
            }

            if (failCount > 0)
            {
                Assert.Inconclusive("FailCount should be zero. Currently " + failCount + " unsupported methods found.");
            }
        }

        [Test]
        public async Task ReflectionMethodsCheckWeSupportAsync(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            MethodCollection methodNames = await f.ReflectionGetMethodsAsync(cancellationToken);

            Assert.IsNotNull(methodNames, "Should not be null");
            Assert.AreNotEqual(0, methodNames.Count, "Should return some method names.");
            Assert.IsNotNull(methodNames[0], "First item should not be null");

            Type type = typeof(Flickr);
            MethodInfo[] methods = type.GetMethods();

            int failCount = 0;

            foreach (string methodName in methodNames)
            {
                bool found = false;
                string trueName = methodName.Replace("flickr.", "").Replace(".", "").ToLower() + "async";
                foreach (MethodInfo info in methods)
                {
                    if (trueName == info.Name.ToLower())
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    failCount++;
                    Console.WriteLine("Async Method '" + methodName + "' not found in FlickrNet.Flickr.");
                }
            }

            if (failCount > 0)
            {
                Assert.Inconclusive("FailCount should be zero. Currently " + failCount + " unsupported methods found.");
            }
        }

        [Test]
        public async Task ReflectionGetMethodInfoSearchArgCheck(CancellationToken cancellationToken = default)
        {
            PropertyInfo[] properties = typeof(PhotoSearchOptions).GetProperties();

            Method flickrMethod = await Instance.ReflectionGetMethodInfoAsync("flickr.photos.search", cancellationToken);

            // These arguments are covered, but are named slightly differently from Flickr.
            Dictionary<string, string> exceptions = new()
            {
                { "license", "licenses" }, // Licenses
                { "sort", "sortorder" }, // SortOrder
                { "bbox", "boundarybox" }, // BoundaryBox
                { "lat", "latitude" }, // Latitude
                { "lon", "longitude" }, // Longitude
                { "media", "mediatype" }, // MediaType
                { "exifminfocallen", "exifminfocallength" }, // Focal Length
                { "exifmaxfocallen", "exifmaxfocallength" } // Focal Length
            };

            int numMissing = 0;

            foreach (MethodArgument argument in flickrMethod.Arguments)
            {
                if (argument.Name == "api_key") continue;

                bool found = false;

                string arg = argument.Name.Replace("_", "").ToLower();

                if (exceptions.ContainsKey(arg)) arg = exceptions[arg];

                foreach (PropertyInfo info in properties)
                {
                    string propName = info.Name.ToLower();
                    if (arg == propName)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    numMissing++;
                    Console.WriteLine("Argument    : " + argument.Name + " not found.");
                    Console.WriteLine("Description : " + argument.Description);
                }
            }

            Assert.AreEqual(0, numMissing, "Number of missing arguments should be zero.");
        }

        [Test]
        [Ignore("Test takes a long time")]
        public async Task ReflectionMethodsCheckWeSupportAndParametersMatch(CancellationToken cancellationToken = default)
        {
            List<string> exceptions = new()
            {
                "flickr.photos.getWithGeoData",
                "flickr.photos.getWithoutGeoData",
                "flickr.photos.search",
                "flickr.photos.getNotInSet",
                "flickr.photos.getUntagged"
            };

            Flickr f = Instance;

            MethodCollection methodNames = await f.ReflectionGetMethodsAsync(cancellationToken);

            Assert.IsNotNull(methodNames, "Should not be null");
            Assert.AreNotEqual(0, methodNames.Count, "Should return some method names.");
            Assert.IsNotNull(methodNames[0], "First item should not be null");

            Type type = typeof(Flickr);
            MethodInfo[] methods = type.GetMethods();

            int failCount = 0;

            foreach (string methodName in methodNames)
            {
                bool found = false;
                bool foundTrue = false;
                string trueName = methodName.Replace("flickr.", "").Replace(".", "").ToLower();
                foreach (MethodInfo info in methods)
                {
                    if (trueName == info.Name.ToLower())
                    {
                        found = true;
                        break;
                    }
                }
                // Check the number of arguments to see if we have a matching method.
                if (found && !exceptions.Contains(methodName))
                {
                    Method method = await f.ReflectionGetMethodInfoAsync(methodName, cancellationToken);
                    foreach (MethodInfo info in methods)
                    {
                        if (method.Arguments.Count - 1 == info.GetParameters().Length)
                        {
                            foundTrue = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    failCount++;
                    Console.WriteLine("Method '" + methodName + "' not found in FlickrNet.Flickr.");
                }
                if (found && !foundTrue)
                {
                    Console.WriteLine("Method '" + methodName + "' found but no matching method with all arguments.");
                }
            }

            Assert.AreEqual(0, failCount, "FailCount should be zero. Currently " + failCount + " unsupported methods found.");
        }

        [Test]
        public async Task ReflectionGetMethodInfoTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;
            Method method = await f.ReflectionGetMethodInfoAsync("flickr.reflection.getMethodInfo", cancellationToken);

            Assert.IsNotNull(method, "Method should not be null");
            Assert.AreEqual("flickr.reflection.getMethodInfo", method.Name, "Method name not set correctly");

            Assert.AreEqual(MethodPermission.None, method.RequiredPermissions);

            Assert.AreEqual(2, method.Arguments.Count, "There should be two arguments");
            Assert.AreEqual("api_key", method.Arguments[0].Name, "First argument should be api_key.");
            Assert.IsFalse(method.Arguments[0].IsOptional, "First argument should not be optional.");

            Assert.AreEqual(9, method.Errors.Count, "There should be 8 errors.");
            Assert.AreEqual(1, method.Errors[0].Code, "First error should have code of 1");
            Assert.AreEqual("Method not found", method.Errors[0].Message, "First error should have code of 1");
            Assert.AreEqual("The requested method was not found.", method.Errors[0].Description, "First error should have code of 1");
        }

        [Test]
        public async Task ReflectionGetMethodInfoFavContextArguments(CancellationToken cancellationToken = default)
        {
            var methodName = "flickr.favorites.getContext";
            var method = await Instance.ReflectionGetMethodInfoAsync(methodName, cancellationToken);

            Assert.AreEqual(3, method.Arguments.Count);
            Assert.AreEqual("The id of the photo to fetch the context for.", method.Arguments[1].Description);
            //Assert.IsNull(method.Arguments[4].Description);
        }

        private async Task GetExceptionList(CancellationToken cancellationToken = default)
        {
            var errors = new Dictionary<int, List<string>>();
            Flickr.CacheDisabled = true;

            Flickr f = Instance;
            var list = await f.ReflectionGetMethodsAsync(cancellationToken);
            foreach (var methodName in list)
            {
                Console.WriteLine("Method = " + methodName);
                var method = await f.ReflectionGetMethodInfoAsync(methodName, cancellationToken);

                foreach (var exception in method.Errors)
                {
                    if (!errors.ContainsKey(exception.Code))
                    {
                        errors[exception.Code] = new List<string>();
                    }

                    var l = errors[exception.Code];
                    if (!l.Contains(exception.Message))
                    {
                        l.Add(exception.Message);
                    }
                }
            }

            foreach (var pair in errors)
            {
                Console.WriteLine("Code,Message");
                foreach (string l in pair.Value)
                {
                    Console.WriteLine(pair.Key + ",\"" + l + "\"");
                }
                Console.WriteLine();
            }
        }
    }
}