using FlickrNet;
using FlickrNet.Enums;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNetTest
{
    [TestFixture]
    public class PhotosSuggestionsTests : BaseTest
    {
        private string photoId = "6282363572";

        [SetUp]
        public void TestInitialize()
        {
            Flickr.CacheDisabled = true;
        }

        [Test]
        [Category("AccessTokenRequired")]
        [Ignore("Throws a 500 exception for some reason.")]
        public async Task GetListTest(CancellationToken cancellationToken = default)
        {
            var f = AuthInstance;

            // Remove any pending suggestions
            var suggestions = await f.PhotosSuggestionsGetListAsync(photoId, SuggestionStatus.Pending, cancellationToken);
            Assert.IsNotNull(suggestions, "SuggestionCollection should not be null.");

            foreach (var s in suggestions)
            {
                if (s.SuggestionId == null)
                {
                    Console.WriteLine(f.LastRequest);
                    Console.WriteLine(f.LastResponse);
                }
                Assert.IsNotNull(s.SuggestionId, "Suggestion ID should not be null.");
                await f.PhotosSuggestionsRemoveSuggestionAsync(s.SuggestionId, cancellationToken);
            }

            // Add test suggestion
            await AddSuggestion(cancellationToken);

            // Get list of suggestions and check
            suggestions = await f.PhotosSuggestionsGetListAsync(photoId, SuggestionStatus.Pending, cancellationToken);

            Assert.IsNotNull(suggestions, "SuggestionCollection should not be null.");
            Assert.AreNotEqual(0, suggestions.Count, "Count should not be zero.");

            var suggestion = suggestions.First();

            Assert.AreEqual("41888973@N00", suggestion.SuggestedByUserId);
            Assert.AreEqual("Sam Judson", suggestion.SuggestedByUserName);
            Assert.AreEqual("I really think this is a good suggestion.", suggestion.Note);
            Assert.AreEqual(54.977, suggestion.Latitude, "Latitude should be the same.");

            await f.PhotosSuggestionsRemoveSuggestionAsync(suggestion.SuggestionId, cancellationToken);

            // Add test suggestion
            await AddSuggestion(cancellationToken);
            suggestion = (await f.PhotosSuggestionsGetListAsync(photoId, SuggestionStatus.Pending, cancellationToken)).First();
            await f.PhotosSuggestionsApproveSuggestionAsync(suggestion.SuggestionId, cancellationToken);
            await f.PhotosSuggestionsRemoveSuggestionAsync(suggestion.SuggestionId, cancellationToken);
        }

        public async Task AddSuggestion(CancellationToken cancellationToken = default)
        {
            var f = AuthInstance;

            var lat = 54.977;
            var lon = -1.612;
            var accuracy = GeoAccuracy.Street;
            var woeId = "30079";
            var placeId = "X9sTR3BSUrqorQ";
            var note = "I really think this is a good suggestion.";

            await f.PhotosSuggestionsSuggestLocationAsync(photoId, lat, lon, accuracy, woeId, placeId, note, cancellationToken);
        }
    }
}