using FlickrNetCore;
using FlickrNetCore.Classes;
using FlickrNetCore.CollectionClasses;
using FlickrNetCore.Common;
using FlickrNetCore.Enums;
using FlickrNetCore.Exceptions;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;
using Shouldly;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PlacesForUserTests
    /// </summary>
    [TestFixture]
    [Ignore("Place functionality is not working very well at the moment!")]
    public class PlacesTests : BaseTest
    {
        [Test]
        public async Task PlacesFindBasicTest(CancellationToken cancellationToken = default)
        {
            var places = await Instance.PlacesFindAsync("Newcastle", cancellationToken);

            Assert.IsNotNull(places);
            Assert.AreNotEqual(0, places.Count);
        }

        [Test]
        public async Task PlacesFindNewcastleTest(CancellationToken cancellationToken = default)
        {
            var places = await Instance.PlacesFindAsync("Newcastle upon Tyne", cancellationToken);

            Assert.IsNotNull(places);
            Assert.AreEqual(1, places.Count);
        }

        [Test]
        public async Task PlacesFindByLatLongNewcastleTest(CancellationToken cancellationToken = default)
        {
            double lat = 54.977;
            double lon = -1.612;

            var place = await Instance.PlacesFindByLatLonAsync(lat, lon, cancellationToken);

            Assert.IsNotNull(place);
            Assert.AreEqual("Haymarket, Newcastle upon Tyne, England, GB, United Kingdom", place.Description);
        }

        [Test]
        public void PlacesPlacesForUserAuthenticationRequiredTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;
            Should.Throw<SignatureRequiredException>(async () => await f.PlacesPlacesForUserAsync(cancellationToken));
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PlacesPlacesForUserHasContinentsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;
            PlaceCollection places = await f.PlacesPlacesForUserAsync(cancellationToken);

            foreach (Place place in places)
            {
                Assert.IsNotNull(place.PlaceId, "PlaceId should not be null.");
                Assert.IsNotNull(place.WoeId, "WoeId should not be null.");
                Assert.IsNotNull(place.Description, "Description should not be null.");
                Assert.AreEqual(PlaceType.Continent, place.PlaceType, "PlaceType should be continent.");
            }

            Assert.AreEqual("6dCBhRRTVrJiB5xOrg", places[0].PlaceId);
            Assert.AreEqual("Europe", places[0].Description);
            Assert.AreEqual("l5geY0lTVrLoNkLgeQ", places[1].PlaceId);
            Assert.AreEqual("North America", places[1].Description);
        }

        [Test, Ignore("Not currently returning any records for some reason.")]
        public async Task PlacesGetChildrenWithPhotosPublicPlaceIdTest(CancellationToken cancellationToken = default)
        {
            string placeId = "6dCBhRRTVrJiB5xOrg"; // Europe
            Flickr f = Instance;

            var places = await f.PlacesGetChildrenWithPhotosPublicAsync(placeId, null, cancellationToken);
            Console.WriteLine(f.LastRequest);
            Console.WriteLine(f.LastResponse);

            Assert.IsNotNull(places);
            Assert.AreNotEqual(0, places.Count);

            foreach (var place in places)
            {
                Assert.AreEqual(PlaceType.Country, place.PlaceType);
            }
        }

        [Test, Ignore("Not currently returning any records for some reason.")]
        public async Task PlacesGetChildrenWithPhotosPublicWoeIdTest(CancellationToken cancellationToken = default)
        {
            string woeId = "24865675"; // Europe

            var places = await Instance.PlacesGetChildrenWithPhotosPublicAsync(null, woeId, cancellationToken);
            Assert.IsNotNull(places);
            Assert.AreNotEqual(0, places.Count);

            foreach (var place in places)
            {
                Assert.AreEqual(PlaceType.Country, place.PlaceType);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PlacesPlacesForUserContinentHasRegionsTest(CancellationToken cancellationToken = default)
        {
            Flickr f = AuthInstance;

            // Test place ID of '6dCBhRRTVrJiB5xOrg' is Europe
            PlaceCollection p = await f.PlacesPlacesForUserAsync(PlaceType.Region, null, "6dCBhRRTVrJiB5xOrg", cancellationToken);

            foreach (Place place in p)
            {
                Assert.IsNotNull(place.PlaceId, "PlaceId should not be null.");
                Assert.IsNotNull(place.WoeId, "WoeId should not be null.");
                Assert.IsNotNull(place.Description, "Description should not be null.");
                Assert.IsNotNull(place.PlaceUrl, "PlaceUrl should not be null");
                Assert.AreEqual(PlaceType.Region, place.PlaceType, "PlaceType should be Region.");
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PlacesPlacesForContactsBasicTest(CancellationToken cancellationToken = default)
        {
            var f = AuthInstance;
            var places = await f.PlacesPlacesForContactsAsync(PlaceType.Country, null, null, 0, ContactSearch.AllContacts, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, cancellationToken);

            Assert.IsNotNull(places);

            Assert.AreNotEqual(0, places.Count);

            foreach (var place in places)
            {
                Assert.AreEqual(PlaceType.Country, place.PlaceType);
                Assert.IsNotNull(place.PlaceId);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PlacesPlacesForContactsFullParamTest(CancellationToken cancellationToken = default)
        {
            DateTime lastYear = DateTime.Today.AddYears(-1);
            DateTime today = DateTime.Today;

            var f = AuthInstance;
            var places = await f.PlacesPlacesForContactsAsync(PlaceType.Country, null, null, 1, ContactSearch.AllContacts, lastYear, today, lastYear, today, cancellationToken);

            Console.WriteLine(f.LastRequest);

            Assert.IsNotNull(places);

            Assert.AreNotEqual(0, places.Count);

            foreach (var place in places)
            {
                Assert.AreEqual(PlaceType.Country, place.PlaceType);
                Assert.IsNotNull(place.PlaceId);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PlacesPlacesForTagsBasicTest(CancellationToken cancellationToken = default)
        {
            var f = AuthInstance;
            var places = await f.PlacesPlacesForTagsAsync(PlaceType.Country, null, null, 0, new string[] { "newcastle" },
                                               TagMode.AllTags, null, MachineTagMode.None, DateTime.MinValue,
                                               DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, cancellationToken);

            Assert.IsNotNull(places);

            Assert.AreNotEqual(0, places.Count);

            foreach (var place in places)
            {
                Assert.AreEqual(PlaceType.Country, place.PlaceType);
                Assert.IsNotNull(place.PlaceId);
            }
        }

        [Test]
        [Category("AccessTokenRequired")]
        public async Task PlacesPlacesForTagsFullParamTest(CancellationToken cancellationToken = default)
        {
            var f = AuthInstance;
            var places = await f.PlacesPlacesForTagsAsync(PlaceType.Country, null, null, 0, new string[] { "newcastle" },
                                               TagMode.AllTags, new string[] { "dc:author=*" }, MachineTagMode.AllTags,
                                               DateTime.Today.AddYears(-10), DateTime.Today,
                                               DateTime.Today.AddYears(-10), DateTime.Today, cancellationToken);

            Assert.IsNotNull(places);
        }

        [Test]
        public async Task PlacesGetInfoBasicTest(CancellationToken cancellationToken = default)
        {
            var f = Instance;
            var placeId = "X9sTR3BSUrqorQ";
            PlaceInfo p = await f.PlacesGetInfoAsync(placeId, null, cancellationToken);

            Console.WriteLine(f.LastResponse);

            Assert.IsNotNull(p);
            Assert.AreEqual(placeId, p.PlaceId);
            Assert.AreEqual("30079", p.WoeId);
            Assert.AreEqual(PlaceType.Locality, p.PlaceType);
            Assert.AreEqual("Newcastle upon Tyne, England, United Kingdom", p.Description);

            Assert.AreEqual("X9sTR3BSUrqorQ", p.Locality.PlaceId);
            Assert.AreEqual("myqh27pQULzLWcg7Kg", p.County.PlaceId);
            Assert.AreEqual("2eIY2QFTVr_DwWZNLg", p.Region.PlaceId);
            Assert.AreEqual("cnffEpdTUb5v258BBA", p.Country.PlaceId);

            Assert.IsTrue(p.HasShapeData);
            Assert.IsNotNull(p.ShapeData);
            Assert.AreEqual(0.00015, p.ShapeData.Alpha);
            Assert.AreEqual(1, p.ShapeData.PolyLines.Count);
            Assert.AreEqual(89, p.ShapeData.PolyLines[0].Count);
            Assert.AreEqual(55.030498504639, p.ShapeData.PolyLines[0][88].X);
            Assert.AreEqual(-1.6404060125351, p.ShapeData.PolyLines[0][88].Y);
        }

        [Test]
        public async Task PlacesGetInfoByUrlBasicTest(CancellationToken cancellationToken = default)
        {
            var f = Instance;
            var placeId = "X9sTR3BSUrqorQ";
            PlaceInfo p1 = await f.PlacesGetInfoAsync(placeId, null, cancellationToken);
            PlaceInfo p2 = await f.PlacesGetInfoByUrlAsync(p1.PlaceUrl, cancellationToken);

            Assert.IsNotNull(p2);
            Assert.AreEqual(p1.PlaceId, p2.PlaceId);
            Assert.AreEqual(p1.WoeId, p2.WoeId);
            Assert.AreEqual(p1.PlaceType, p2.PlaceType);
            Assert.AreEqual(p1.Description, p2.Description);

            Assert.IsNotNull(p2.PlaceFlickrUrl);
        }

        [Test]
        public async Task PlacesGetTopPlacesListTest(CancellationToken cancellationToken = default)
        {
            var f = Instance;
            var places = await f.PlacesGetTopPlacesListAsync(PlaceType.Continent, cancellationToken);

            Assert.IsNotNull(places);
            Assert.AreNotEqual(0, places.Count);

            foreach (var p in places)
            {
                Assert.AreEqual(PlaceType.Continent, p.PlaceType);
                Assert.IsNotNull(p.PlaceId);
                Assert.IsNotNull(p.WoeId);
            }
        }

        [Test]
        public async Task PlacesGetShapeHistoryTest(CancellationToken cancellationToken = default)
        {
            var placeId = "X9sTR3BSUrqorQ";
            var f = Instance;
            var col = await f.PlacesGetShapeHistoryAsync(placeId, null, cancellationToken);

            Assert.IsNotNull(col, "ShapeDataCollection should not be null.");
            Assert.AreEqual(7, col.Count, "Count should be six.");
            Assert.AreEqual(placeId, col.PlaceId);

            Assert.AreEqual(1, col[1].PolyLines.Count, "The second shape should have one polylines.");
        }

        [Test]
        public async Task PlacesGetTagsForPlace(CancellationToken cancellationToken = default)
        {
            var placeId = "X9sTR3BSUrqorQ";
            var f = Instance;
            var col = await f.PlacesTagsForPlaceAsync(placeId, null, cancellationToken);

            Assert.IsNotNull(col, "TagCollection should not be null.");
            Assert.AreEqual(100, col.Count, "Count should be one hundred.");

            foreach (var t in col)
            {
                Assert.AreNotEqual(0, t.Count, "Count should be greater than zero.");
                Assert.IsNotNull(t.TagName, "TagName should not be null.");
            }
        }

        [Test]
        public async Task PlacesGetPlaceTypes(CancellationToken cancellationToken = default)
        {
            var pts = await Instance.PlacesGetPlaceTypesAsync(cancellationToken);
            Assert.IsNotNull(pts);
            Assert.IsTrue(pts.Count > 1, "Count should be greater than one. Count = " + pts.Count + ".");

            foreach (var kp in pts)
            {
                Assert.AreNotEqual(0, kp.Id, "Key should not be zero.");
                Assert.IsNotNull(kp.Name, "Value should not be null.");

                Assert.IsTrue(Enum.IsDefined(typeof(PlaceType), kp.Id), "PlaceType with ID " + kp.Id + " and Value '" + kp.Name + "' not defined in PlaceType enum.");
                var type = (PlaceType)kp.Id;
                Assert.AreEqual(type.ToString("G").ToLower(), kp.Name, "Name of enum should match.");
            }
        }

        [Test]
        public async Task PlacesPlacesForBoundingBoxUsaTest(CancellationToken cancellationToken = default)
        {
            Flickr f = Instance;

            var places = await f.PlacesPlacesForBoundingBoxAsync(PlaceType.County, null, null, BoundaryBox.UKNewcastle, cancellationToken);

            Assert.IsNotNull(places);
            Assert.AreNotEqual(0, places.Count);

            foreach (var place in places)
            {
                Assert.IsNotNull(place.PlaceId);
                Assert.AreEqual(PlaceType.County, place.PlaceType);
            }
        }
    }
}