using FlickrNetCore.Classes;
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
            Assert.Multiple(() =>
            {
                Assert.That(info.PhotoId, Is.EqualTo("7519320006"));
                Assert.That(info.Location, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(info.Location.Accuracy, Is.EqualTo((GeoAccuracy)16));

                Assert.That(info.Location.Country, Is.Not.Null);
            });
            Assert.That(info.Location.Country.PlaceId, Is.EqualTo("cnffEpdTUb5v258BBA"));
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
            Assert.Multiple(() =>
            {
                Assert.That(info.PhotoId, Is.EqualTo("7519320006"));
                Assert.That(info.Location, Is.Not.Null);
            });
            Assert.That(info.Location.Accuracy, Is.EqualTo((GeoAccuracy)10));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosForLocationReturnsPhotos()
        {
            var photos = await Instance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = true, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo, PerPage = 10 });

            var geoPhoto = photos.First();

            var geoPhotos = await AuthInstance.PhotosGeoPhotosForLocationAsync(geoPhoto.Latitude, geoPhoto.Longitude, GeoAccuracy.Street, PhotoSearchExtras.None, 100, 1);

            Assert.That(geoPhotos.Select(p => p.PhotoId).Contains(geoPhoto.PhotoId), Is.True);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetGetLocationTest()
        {
            var photos = await AuthInstance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = true, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo });

            var photo = photos.First();

            Console.WriteLine(photo.PhotoId);

            var location = await AuthInstance.PhotosGeoGetLocationAsync(photo.PhotoId);
            Assert.Multiple(() =>
            {
                Assert.That(location.Longitude, Is.EqualTo(photo.Longitude), "Longitudes should match exactly.");
                Assert.That(location.Latitude, Is.EqualTo(photo.Latitude), "Latitudes should match exactly.");
            });
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGetGetLocationNullTest()
        {
            var photos = await AuthInstance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = false, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo });

            var photo = photos.First();

            var location = await AuthInstance.PhotosGeoGetLocationAsync(photo.PhotoId);

            Assert.That(location, Is.Null, "Location should be null.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        [Ignore("Flickr not returning place id correctly.")]
        public async Task PhotosGetCorrectLocationTest()
        {
            var photo = (await AuthInstance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = true, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo })).First();

            await AuthInstance.PhotosGeoCorrectLocationAsync(photo.PhotoId, photo.PlaceId, null);
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGeoSetContextTest()
        {
            var photo = (await AuthInstance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = true, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo })).First();

            Assert.That(photo.GeoContext.HasValue, Is.True, "GeoContext should be set.");

            var origContext = photo.GeoContext.Value;

            var newContext = origContext == GeoContext.Indoors ? GeoContext.Outdoors : GeoContext.Indoors;

            try
            {
                await AuthInstance.PhotosGeoSetContextAsync(photo.PhotoId, newContext);
            }
            finally
            {
                await AuthInstance.PhotosGeoSetContextAsync(photo.PhotoId, origContext);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        [Ignore("Flickr returning 'Sorry, the Flickr API service is not currently available' error.")]
        public async Task PhotosGeoSetLocationTest()
        {
            var photo = (await AuthInstance.PhotosSearchAsync(new PhotoSearchOptions { HasGeo = true, UserId = TestData.TestUserId, Extras = PhotoSearchExtras.Geo })).First();

            if (photo.GeoContext == null)
            {
                Assert.Fail("GeoContext should not be null");
            }

            var origGeo = new { photo.Latitude, photo.Longitude, photo.Accuracy, Context = photo.GeoContext.Value };
            var newGeo = new { Latitude = -23.32, Longitude = -34.2, Accuracy = GeoAccuracy.Level10, Context = GeoContext.Indoors };

            try
            {
                await AuthInstance.PhotosGeoSetLocationAsync(photo.PhotoId, newGeo.Latitude, newGeo.Longitude, newGeo.Accuracy);

                var location = await AuthInstance.PhotosGeoGetLocationAsync(photo.PhotoId);
                Assert.Multiple(() =>
                {
                    Assert.That(location.Latitude, Is.EqualTo(newGeo.Latitude), "New Latitude should be set.");
                    Assert.Multiple(() =>
                {
                    Assert.That(location.Longitude, Is.EqualTo(newGeo.Longitude), "New Longitude should be set.");
                    Assert.That(location.Context, Is.EqualTo(newGeo.Context), "New Context should be set.");
                    Assert.That(location.Accuracy, Is.EqualTo(newGeo.Accuracy), "New Accuracy should be set.");
                });
                });
            }
            finally
            {
                await AuthInstance.PhotosGeoSetLocationAsync(photo.PhotoId, origGeo.Latitude, origGeo.Longitude, origGeo.Accuracy);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PhotosGeoPhotosForLocationBasicTest()
        {
            var o = new PhotoSearchOptions
            {
                UserId = TestData.TestUserId,
                HasGeo = true,
                PerPage = 1,
                Extras = PhotoSearchExtras.Geo
            };

            var photos = await AuthInstance.PhotosSearchAsync(o);
            var photo = photos[0];

            var photos2 = await AuthInstance.PhotosGeoPhotosForLocationAsync(photo.Latitude, photo.Longitude, photo.Accuracy, PhotoSearchExtras.All, 0, 0);

            Assert.That(photos2, Is.Not.Null, "PhotosGeoPhotosForLocation should not return null.");
            Assert.That(photos2.Count > 0, Is.True, "Should return one or more photos.");

            foreach (var p in photos2)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(p.PhotoId, Is.Not.Null);
                    Assert.Multiple(() =>
                {
                    Assert.That(p.Longitude, Is.Not.EqualTo(0));
                    Assert.That(p.Latitude, Is.Not.EqualTo(0));
                });
                });
            }
        }
    }
}