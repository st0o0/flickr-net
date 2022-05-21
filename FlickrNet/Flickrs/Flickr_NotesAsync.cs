using FlickrNet.Flickrs.Results;
using FlickrNet.Models;
using System;
using System.Collections.Generic;

namespace FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// Add a note to a picture.
        /// </summary>
        /// <param name="photoId">The photo id to add the note to.</param>
        /// <param name="noteX">The X co-ordinate of the upper left corner of the note.</param>
        /// <param name="noteY">The Y co-ordinate of the upper left corner of the note.</param>
        /// <param name="noteWidth">The width of the note.</param>
        /// <param name="noteHeight">The height of the note.</param>
        /// <param name="noteText">The text in the note.</param>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        public void PhotosNotesAddAsync(string photoId, int noteX, int noteY, int noteWidth, int noteHeight, string noteText, Action<FlickrResult<string>> callback)
        {
            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.photos.notes.add" },
                { "photo_id", photoId },
                { "note_x", noteX.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "note_y", noteY.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "note_w", noteWidth.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "note_h", noteHeight.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "note_text", noteText }
            };

            GetResponseAsync<UnknownResponse>(
                parameters,
                r =>
                {
                    FlickrResult<string> result = new FlickrResult<string>
                    {
                        HasError = r.HasError
                    };
                    if (r.HasError)
                    {
                        result.Error = r.Error;
                    }
                    else
                    {
                        result.Result = r.Result.GetAttributeValue("*", "id");
                    }
                    callback(result);
                });
        }

        /// <summary>
        /// Edit and update a note.
        /// </summary>
        /// <param name="noteId">The ID of the note to update.</param>
        /// <param name="noteX">The X co-ordinate of the upper left corner of the note.</param>
        /// <param name="noteY">The Y co-ordinate of the upper left corner of the note.</param>
        /// <param name="noteWidth">The width of the note.</param>
        /// <param name="noteHeight">The height of the note.</param>
        /// <param name="noteText">The new text in the note.</param>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        public void PhotosNotesEditAsync(string noteId, int noteX, int noteY, int noteWidth, int noteHeight, string noteText, Action<FlickrResult<NoResponse>> callback)
        {
            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.photos.notes.edit" },
                { "note_id", noteId },
                { "note_x", noteX.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "note_y", noteY.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "note_w", noteWidth.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "note_h", noteHeight.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "note_text", noteText }
            };

            GetResponseAsync<NoResponse>(parameters, callback);
        }

        /// <summary>
        /// Delete an existing note.
        /// </summary>
        /// <param name="noteId">The ID of the note.</param>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        public void PhotosNotesDeleteAsync(string noteId, Action<FlickrResult<NoResponse>> callback)
        {
            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.photos.notes.delete" },
                { "note_id", noteId }
            };

            GetResponseAsync<NoResponse>(parameters, callback);
        }
    }
}