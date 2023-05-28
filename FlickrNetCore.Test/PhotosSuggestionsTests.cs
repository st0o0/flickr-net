using FlickrNetCore;
using FlickrNetCore.Enums;
using FlickrNetCore.Test.TestUtilities;
using NUnit.Framework;

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
            var suggestions = await f.PhotosSuggestionsGetListAsync(photoId, SuggestionStatus.Pending);
            Assert.That(suggestions, Is.Not.Null, "SuggestionCollection should not be null.");

            foreach (var s in suggestions)
            {
                if (s.SuggestionId == null)
                {
                    Console.WriteLine(f.LastRequest);
                    Console.WriteLine(f.LastResponse);
                }
                Assert.That(s.SuggestionId, Is.Not.Null, "Suggestion ID should not be null.");
                await f.PhotosSuggestionsRemoveSuggestionAsync(s.SuggestionId);
            }

            // Add test suggestion
            await AddSuggestion(cancellationToken);

            // Get list of suggestions and check
            suggestions = await f.PhotosSuggestionsGetListAsync(photoId, SuggestionStatus.Pending);

            Assert.That(suggestions, Is.Not.Null, "SuggestionCollection should not be null.");
            Assert.That(suggestions, Is.Not.Empty, "Count should not be zero.");

            var suggestion = suggestions.First();
            Assert.Multiple(async () =>
            {
                Assert.That(suggestion.SuggestedByUserId, Is.EqualTo("41888973@N00"));
                Assert.Multiple(() =>
                {
                    Assert.That(suggestion.SuggestedByUserName, Is.EqualTo("Sam Judson"));
                    Assert.That(suggestion.Note, Is.EqualTo("I really think this is a good suggestion."));
                    Assert.That(suggestion.Latitude, Is.EqualTo(54.977), "Latitude should be the same.");
                });
                await f.PhotosSuggestionsRemoveSuggestionAsync(suggestion.SuggestionId);

                // Add test suggestion
                await AddSuggestion(cancellationToken);
                suggestion = (await f.PhotosSuggestionsGetListAsync(photoId, SuggestionStatus.Pending)).First();
                await f.PhotosSuggestionsApproveSuggestionAsync(suggestion.SuggestionId);
                await f.PhotosSuggestionsRemoveSuggestionAsync(suggestion.SuggestionId);
            });
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

            await f.PhotosSuggestionsSuggestLocationAsync(photoId, lat, lon, accuracy, woeId, placeId, note);
        }
    }
}