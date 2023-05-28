using FlickrNetCore;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.SearchOptions;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using System.Reflection;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for ReflectionMethodTests
    /// </summary>
    [TestFixture]
    public class ReflectionMethodTests : BaseTest
    {
        [Test]
        public async Task ReflectionMethodsBasic()
        {
            Flickr f = Instance;

            MethodCollection methodNames = await f.ReflectionGetMethodsAsync();

            Assert.That(methodNames, Is.Not.Null, "Should not be null");
            Assert.That(methodNames, Is.Not.Empty, "Should return some method names.");
            Assert.That(methodNames[0], Is.Not.Null, "First item should not be null");
        }

        [Test]
        public async Task ReflectionMethodsCheckWeSupport()
        {
            Flickr f = Instance;

            MethodCollection methodNames = await f.ReflectionGetMethodsAsync();

            Assert.That(methodNames, Is.Not.Null, "Should not be null");
            Assert.That(methodNames, Is.Not.Empty, "Should return some method names.");
            Assert.That(methodNames[0], Is.Not.Null, "First item should not be null");

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
        public async Task ReflectionMethodsCheckWeSupportAsync()
        {
            Flickr f = Instance;

            MethodCollection methodNames = await f.ReflectionGetMethodsAsync();

            Assert.That(methodNames, Is.Not.Null, "Should not be null");
            Assert.That(methodNames, Is.Not.Empty, "Should return some method names.");
            Assert.That(methodNames[0], Is.Not.Null, "First item should not be null");

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
        public async Task ReflectionGetMethodInfoSearchArgCheck()
        {
            PropertyInfo[] properties = typeof(PhotoSearchOptions).GetProperties();

            Method flickrMethod = await Instance.ReflectionGetMethodInfoAsync("flickr.photos.search");

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

            Assert.That(numMissing, Is.EqualTo(0), "Number of missing arguments should be zero.");
        }

        [Test]
        [Ignore("Test takes a long time")]
        public async Task ReflectionMethodsCheckWeSupportAndParametersMatch()
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

            MethodCollection methodNames = await f.ReflectionGetMethodsAsync();

            Assert.That(methodNames, Is.Not.Null, "Should not be null");
            Assert.That(methodNames, Is.Not.Empty, "Should return some method names.");
            Assert.That(methodNames[0], Is.Not.Null, "First item should not be null");

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
                    Method method = await f.ReflectionGetMethodInfoAsync(methodName);
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

            Assert.That(failCount, Is.EqualTo(0), "FailCount should be zero. Currently " + failCount + " unsupported methods found.");
        }

        [Test]
        public async Task ReflectionGetMethodInfoTest()
        {
            Flickr f = Instance;
            Method method = await f.ReflectionGetMethodInfoAsync("flickr.reflection.getMethodInfo");

            Assert.That(method, Is.Not.Null, "Method should not be null");
            Assert.Multiple(() =>
            {
                Assert.That(method.Name, Is.EqualTo("flickr.reflection.getMethodInfo"), "Method name not set correctly");
                Assert.Multiple(() =>
                    {
                        Assert.That(method.RequiredPermissions, Is.EqualTo(MethodPermission.None));

                        Assert.That(method.Arguments, Has.Count.EqualTo(2), "There should be two arguments");
                    });
                Assert.That(method.Arguments[0].Name, Is.EqualTo("api_key"), "First argument should be api_key.");
                Assert.That(method.Arguments[0].IsOptional, Is.False, "First argument should not be optional.");

                Assert.That(method.Errors, Has.Count.EqualTo(9), "There should be 8 errors.");
                Assert.Multiple(() =>
                {
                    Assert.That(method.Errors[0].Code, Is.EqualTo(1), "First error should have code of 1");
                    Assert.Multiple(() =>
                {
                    Assert.That(method.Errors[0].Message, Is.EqualTo("Method not found"), "First error should have code of 1");
                    Assert.That(method.Errors[0].Description, Is.EqualTo("The requested method was not found."), "First error should have code of 1");
                });
                });
            });
        }

        [Test]
        public async Task ReflectionGetMethodInfoFavContextArguments()
        {
            var methodName = "flickr.favorites.getContext";
            var method = await Instance.ReflectionGetMethodInfoAsync(methodName);

            Assert.That(method.Arguments, Has.Count.EqualTo(3));
            Assert.That(method.Arguments[1].Description, Is.EqualTo("The id of the photo to fetch the context for."));
            //Assert.IsNull(method.Arguments[4].Description);
        }

        private async Task GetExceptionList()
        {
            var errors = new Dictionary<int, List<string>>();
            Flickr.CacheDisabled = true;

            Flickr f = Instance;
            var list = await f.ReflectionGetMethodsAsync();
            foreach (var methodName in list)
            {
                Console.WriteLine("Method = " + methodName);
                var method = await f.ReflectionGetMethodInfoAsync(methodName);

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