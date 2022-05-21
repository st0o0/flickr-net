using FlickrNet.CollectionModels;
using FlickrNet.Enums;
using FlickrNet.Flickrs.Results;
using FlickrNet.Models;
using System;
using System.Collections.Generic;

namespace FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// Approve a location suggestion for a photo.
        /// </summary>
        /// <param name="suggestionId">The suggestion to approve.</param>
        /// <param name="callback"></param>
        public void PhotosSuggestionsApproveSuggestionAsync(string suggestionId, Action<FlickrResult<NoResponse>> callback)
        {
            CheckRequiresAuthentication();

            if (string.IsNullOrEmpty(suggestionId))
            {
                throw new ArgumentNullException(nameof(suggestionId));
            }

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.photos.suggestions.approveSuggestion" },
                { "suggestion_id", suggestionId }
            };

            GetResponseAsync<NoResponse>(parameters, callback);
        }

        /// <summary>
        /// Get a list of suggestions for a photo.
        /// </summary>
        /// <param name="photoId">The photo id of the photo to list the suggestions for.</param>
        /// <param name="status">The type of status to filter by.</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public void PhotosSuggestionsGetListAsync(string photoId, SuggestionStatus status, Action<FlickrResult<SuggestionCollection>> callback)
        {
            CheckRequiresAuthentication();

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.photos.suggestions.getList" },
                { "photo_id", photoId },
                { "status_id", status.ToString("d") }
            };

            GetResponseAsync<SuggestionCollection>(parameters, callback);
        }

        /// <summary>
        /// Rejects a suggestion made for a location on a photo. Currently doesn't appear to actually work. Just use <see cref="Flickr.PhotosSuggestionsRemoveSuggestion"/> instead.
        /// </summary>
        /// <param name="suggestionId">The ID of the suggestion to remove.</param>
        /// <param name="callback"></param>
        public void PhotosSuggestionsRejectSuggestionAsync(string suggestionId, Action<FlickrResult<NoResponse>> callback)
        {
            CheckRequiresAuthentication();

            if (string.IsNullOrEmpty(suggestionId))
            {
                throw new ArgumentNullException(nameof(suggestionId));
            }

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.photos.suggestions.rejectSuggestion" },
                { "suggestion_id", suggestionId }
            };

            GetResponseAsync<NoResponse>(parameters, callback);
        }

        /// <summary>
        /// Remove a location suggestion from a photo.
        /// </summary>
        /// <param name="suggestionId">The suggestion to remove.</param>
        /// <param name="callback"></param>
        public void PhotosSuggestionsRemoveSuggestionAsync(string suggestionId, Action<FlickrResult<NoResponse>> callback)
        {
            CheckRequiresAuthentication();

            if (string.IsNullOrEmpty(suggestionId))
            {
                throw new ArgumentNullException(nameof(suggestionId));
            }

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.photos.suggestions.removeSuggestion" },
                { "suggestion_id", suggestionId }
            };

            GetResponseAsync<NoResponse>(parameters, callback);
        }

        /// <summary>
        /// Suggest a particular location for a photo.
        /// </summary>
        /// <param name="photoId">The id of the photo.</param>
        /// <param name="latitude">The latitude of the location to suggest.</param>
        /// <param name="longitude">The longitude of the location to suggest.</param>
        /// <param name="accuracy">The accuracy of the location to suggest.</param>
        /// <param name="woeId">The WOE ID of the location to suggest.</param>
        /// <param name="placeId">The Flickr place id of the location to suggest.</param>
        /// <param name="note">A note to add to the suggestion.</param>
        /// <param name="callback"></param>
        public void PhotosSuggestionsSuggestLocationAsync(string photoId, double latitude, double longitude,
                                                          GeoAccuracy accuracy, string woeId, string placeId,
                                                          string note, Action<FlickrResult<NoResponse>> callback)
        {
            CheckRequiresAuthentication();

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.photos.suggestions.suggestLocation" },
                { "photo_id", photoId },
                { "lat", latitude.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "lon", longitude.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "accuracy", accuracy.ToString("D") },
                { "place_id", placeId },
                { "woe_id", woeId },
                { "note", note }
            };

            GetResponseAsync<NoResponse>(parameters, callback);
        }
    }
}