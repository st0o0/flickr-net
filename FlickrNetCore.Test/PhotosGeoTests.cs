﻿using FlickrNetCore.Classes;
using FlickrNetCore.Classes.Interfaces;
using FlickrNetCore.Enums;
using FlickrNetCore.SearchOptions;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PhotosGeoTests
    /// </summary>
    [TestFixture]
    public class PhotosGeoTests : BaseTest
    {
        [Test]
        public void PhotoInfoParseFull()
        {
            const string xml = "<photo id=\"7519320006\">"
                             + "<location latitude=\"54.971831\" longitude=\"-1.612683\" accuracy=\"16\" context=\"0\" place_id=\"Ke8IzXlQV79yxA\" woeid=\"15532\">"
                             + "<neighbourhood place_id=\"Ke8IzXlQV79yxA\" woeid=\"15532\">Central</neighbourhood>"
                             + "<locality place_id=\"DW0IUrFTUrO0FQ\" woeid=\"20928\">Gateshead</locality>"
                             + "<county place_id=\"myqh27pQULzLWcg7Kg\" woeid=\"12602156\">Tyne and Wear</county>"
                             + "<region place_id=\"2eIY2QFTVr_DwWZNLg\" woeid=\"24554868\">England</region>"
                             + "<country place_id=\"cnffEpdTUb5v258BBA\" woeid=\"23424975\">United Kingdom</country>"
                             + "</location>"
                             + "</photo>";

            var sr = new System.IO.StringReader(xml);
            var xr = new System.Xml.XmlTextReader(sr);
            xr.Read();

            var info = new PhotoInfo();
            ((IFlickrParsable)info).Load(xr);

            Assert.AreEqual("7519320006", info.PhotoId);
            Assert.IsNotNull(info.Location);
            Assert.AreEqual((GeoAccuracy)16, info.Location.Accuracy);

            Assert.IsNotNull(info.Location.Country);
            Assert.AreEqual("cnffEpdTUb5v258BBA", info.Location.Country.PlaceId);
        }

        [Test]
        public void PhotoInfoLocationParseShortTest()
        {
            const string xml = "<photo id=\"7519320006\">"
                             + "<location latitude=\"-23.32\" longitude=\"-34.2\" accuracy=\"10\" context=\"1\" />"
                             + "</photo>";

            var sr = new System.IO.StringReader(xml);
            var xr = new System.Xml.XmlTextReader(sr);
            xr.Read();

            var info = new PhotoInfo();
            ((IFlickrParsable)info).Load(xr);

            Assert.AreEqual("7519320006", info.PhotoId);
            Assert.IsNotNull(info.Location);
            Assert.AreEqual((GeoAccuracy)10, info.Location.Accuracy);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosForLocationReturnsPhotos(CancellationToken cancellationToken = default)
        {
            var photos = await Instance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = true, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo, PerPage = 10 }, cancellationToken);

            var geoPhoto = photos.First();

            var geoPhotos = await AuthInstance.PhotosGeoPhotosForLocationAsync(geoPhoto.Latitude, geoPhoto.Longitude, GeoAccuracy.Street, PhotoSearchExtras.None, 100, 1, cancellationToken);

            Assert.IsTrue(geoPhotos.Select(p => p.PhotoId).Contains(geoPhoto.PhotoId));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetGetLocationTest(CancellationToken cancellationToken = default)
        {
            var photos = await AuthInstance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = true, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo }, cancellationToken);

            var photo = photos.First();

            Console.WriteLine(photo.PhotoId);

            var location = await AuthInstance.PhotosGeoGetLocationAsync(photo.PhotoId, cancellationToken);

            Assert.AreEqual(photo.Longitude, location.Longitude, "Longitudes should match exactly.");
            Assert.AreEqual(photo.Latitude, location.Latitude, "Latitudes should match exactly.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetGetLocationNullTest(CancellationToken cancellationToken = default)
        {
            var photos = await AuthInstance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = false, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo }, cancellationToken);

            var photo = photos.First();

            var location = await AuthInstance.PhotosGeoGetLocationAsync(photo.PhotoId, cancellationToken);

            Assert.IsNull(location, "Location should be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        [Ignore("Flickr not returning place id correctly.")]
        public async Task PhotosGetCorrectLocationTest(CancellationToken cancellationToken = default)
        {
            var photo = (await AuthInstance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = true, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo }, cancellationToken)).First();

            await AuthInstance.PhotosGeoCorrectLocationAsync(photo.PhotoId, photo.PlaceId, null, cancellationToken);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGeoSetContextTest(CancellationToken cancellationToken = default)
        {
            var photo = (await AuthInstance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = true, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo }, cancellationToken)).First();

            Assert.IsTrue(photo.GeoContext.HasValue, "GeoContext should be set.");

            var origContext = photo.GeoContext.Value;

            var newContext = origContext == GeoContext.Indoors ? GeoContext.Outdoors : GeoContext.Indoors;

            try
            {
                await AuthInstance.PhotosGeoSetContextAsync(photo.PhotoId, newContext, cancellationToken);
            }
            finally
            {
                await AuthInstance.PhotosGeoSetContextAsync(photo.PhotoId, origContext, cancellationToken);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        [Ignore("Flickr returning 'Sorry, the Flickr API service is not currently available' error.")]
        public async Task PhotosGeoSetLocationTest(CancellationToken cancellationToken = default)
        {
            var photo = (await AuthInstance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = true, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo }, cancellationToken)).First();

            if (photo.GeoContext == null)
            {
                Assert.Fail("GeoContext should not be null");
            }

            var origGeo = new { photo.Latitude, photo.Longitude, photo.Accuracy, Context = photo.GeoContext.Value };
            var newGeo = new { Latitude = -23.32, Longitude = -34.2, Accuracy = GeoAccuracy.Level10, Context = GeoContext.Indoors };

            try
            {
                await AuthInstance.PhotosGeoSetLocationAsync(photo.PhotoId, newGeo.Latitude, newGeo.Longitude, newGeo.Accuracy, cancellationToken);

                var location = await AuthInstance.PhotosGeoGetLocationAsync(photo.PhotoId, cancellationToken);
                Assert.AreEqual(newGeo.Latitude, location.Latitude, "New Latitude should be set.");
                Assert.AreEqual(newGeo.Longitude, location.Longitude, "New Longitude should be set.");
                Assert.AreEqual(newGeo.Context, location.Context, "New Context should be set.");
                Assert.AreEqual(newGeo.Accuracy, location.Accuracy, "New Accuracy should be set.");
            }
            finally
            {
                await AuthInstance.PhotosGeoSetLocationAsync(photo.PhotoId, origGeo.Latitude, origGeo.Longitude, origGeo.Accuracy, cancellationToken);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGeoPhotosForLocationBasicTest(CancellationToken cancellationToken = default)
        {
            var o = new PhotoSearchOptions
            {
                UserId = TestData.TestUserId,
                HasGeo = true,
                PerPage = 1,
                Extras = PhotoSearchExtras.Geo
            };

            var photos = await AuthInstance.PhotosSearchAsync(o, cancellationToken);
            var photo = photos[0];

            var photos2 = await AuthInstance.PhotosGeoPhotosForLocationAsync(photo.Latitude, photo.Longitude, photo.Accuracy, PhotoSearchExtras.All, 0, 0, cancellationToken);

            Assert.IsNotNull(photos2, "PhotosGeoPhotosForLocation should not return null.");
            Assert.IsTrue(photos2.Count > 0, "Should return one or more photos.");

            foreach (var p in photos2)
            {
                Assert.IsNotNull(p.PhotoId);
                Assert.AreNotEqual(0, p.Longitude);
                Assert.AreNotEqual(0, p.Latitude);
            }
        }
    }
}