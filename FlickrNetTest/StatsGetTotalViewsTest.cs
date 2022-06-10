using FlickrNet.CollectionModels;
using FlickrNet.Models;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for StatsGetTotalViewsTest
    /// </summary>
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class StatsGetTotalViewsTest : BaseTest
    {
        [Test]
        public async Task StatsGetTotalViewsBasicTest(CancellationToken cancellationToken = default)
        {
            StatViews views = await AuthInstance.StatsGetTotalViewsAsync(cancellationToken);

            Assert.IsNotNull(views, "StatViews should not be null.");
            Assert.AreNotEqual(0, views.TotalViews, "TotalViews should be greater than zero.");
            Assert.AreNotEqual(0, views.PhotostreamViews, "PhotostreamViews should be greater than zero.");
            Assert.AreNotEqual(0, views.PhotoViews, "PhotoViews should be greater than zero.");
        }

        [Test]
        public async Task StatGetCsvFilesTest(CancellationToken cancellationToken = default)
        {
            CsvFileCollection col = await AuthInstance.StatsGetCsvFilesAsync(cancellationToken);

            Assert.IsNotNull(col, "CsvFileCollection should not be null.");

            Assert.IsTrue(col.Count > 1, "Should be more than one CsvFile returned.");

            foreach (var file in col)
            {
                Assert.IsNotNull(file.Href, "Href should not be null.");
                Assert.IsNotNull(file.Type, "Type should not be null.");
                Assert.AreNotEqual(DateTime.MinValue, file.Date);
            }
        }
    }
}