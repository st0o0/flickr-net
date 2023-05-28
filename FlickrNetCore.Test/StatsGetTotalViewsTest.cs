﻿using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

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
        public async Task StatsGetTotalViewsBasicTest()
        {
            StatViews views = await AuthInstance.StatsGetTotalViewsAsync();

            Assert.That(views, Is.Not.Null, "StatViews should not be null.");
            Assert.Multiple(() =>
            {
                Assert.That(views.TotalViews, Is.Not.EqualTo(0), "TotalViews should be greater than zero.");
                Assert.Multiple(() =>
            {
                Assert.That(views.PhotostreamViews, Is.Not.EqualTo(0), "PhotostreamViews should be greater than zero.");
                Assert.That(views.PhotoViews, Is.Not.EqualTo(0), "PhotoViews should be greater than zero.");
            });
            });
        }

        [Test]
        public async Task StatGetCsvFilesTest()
        {
            CsvFileCollection col = await AuthInstance.StatsGetCsvFilesAsync();

            Assert.That(col, Is.Not.Null, "CsvFileCollection should not be null.");

            Assert.That(col, Has.Count.GreaterThan(1), "Should be more than one CsvFile returned.");

            foreach (var file in col)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(file.Href, Is.Not.Null, "Href should not be null.");
                    Assert.That(file.Type, Is.Not.Null, "Type should not be null.");
                });
                Assert.That(file.Date, Is.Not.EqualTo(DateTime.MinValue));
            }
        }
    }
}