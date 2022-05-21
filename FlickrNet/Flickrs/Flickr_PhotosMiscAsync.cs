using FlickrNet.CollectionModels;
using FlickrNet.Flickrs.Results;
using FlickrNet.Models;
using System;
using System.Collections.Generic;

namespace FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// Rotates a photo on Flickr.
        /// </summary>
        /// <remarks>
        /// Does not rotate the original photo.
        /// </remarks>
        /// <param name="photoId">The ID of the photo.</param>
        /// <param name="degrees">The number of degrees to rotate by. Valid values are 90, 180 and 270.</param>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        public void PhotosTransformRotateAsync(string photoId, int degrees, Action<FlickrResult<NoResponse>> callback)
        {
            if (photoId == null)
            {
                throw new ArgumentNullException(nameof(photoId));
            }

            if (degrees != 90 && degrees != 180 && degrees != 270)
            {
                throw new ArgumentException("Must be 90, 180 or 270", nameof(degrees));
            }

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.photos.transform.rotate" },
                { "photo_id", photoId },
                { "degrees", degrees.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) }
            };

            GetResponseAsync<NoResponse>(parameters, callback);
        }

        /// <summary>
        /// Checks the status of one or more asynchronous photo upload tickets.
        /// </summary>
        /// <param name="tickets">A list of ticket ids</param>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        public void PhotosUploadCheckTicketsAsync(string[] tickets, Action<FlickrResult<TicketCollection>> callback)
        {
            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.photos.upload.checkTickets" },
                { "tickets", string.Join(",", tickets) }
            };

            GetResponseAsync<TicketCollection>(parameters, callback);
        }
    }
}