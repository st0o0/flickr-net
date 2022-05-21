﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FlickrNet
{
    /// <summary>
    /// Flickr library interaction with the web goes in here.
    /// </summary>
    public static partial class FlickrResponder
    {
        private const string PostContentType = "application/x-www-form-urlencoded";

        /// <summary>
        /// Returns the string for the Authorisation header to be used for OAuth authentication.
        /// Parameters other than OAuth ones are ignored.
        /// </summary>
        /// <param name="parameters">OAuth and other parameters.</param>
        /// <returns></returns>
        public static string OAuthCalculateAuthHeader(Dictionary<string, string> parameters)
        {
            // Silverlight < 5 doesn't support modification of the Authorization header, so all data must be sent in post body.

            StringBuilder sb = new("OAuth ");
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                if (pair.Key.StartsWith("oauth", StringComparison.Ordinal))
                {
                    sb.Append(pair.Key + "=\"" + Uri.EscapeDataString(pair.Value) + "\",");
                }
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        /// <summary>
        /// Calculates for form encoded POST data to be included in the body of an OAuth call.
        /// </summary>
        /// <remarks>This will include all non-OAuth parameters. The OAuth parameter will be included in the Authentication header.</remarks>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string OAuthCalculatePostData(Dictionary<string, string> parameters)
        {
            string data = string.Empty;
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                // Silverlight < 5 doesn't support modification of the Authorization header, so all data must be sent in post body.

                if (!pair.Key.StartsWith("oauth", StringComparison.Ordinal))
                {
                    data += pair.Key + "=" + UtilityMethods.EscapeDataString(pair.Value) + "&";
                }
            }
            return data;
        }
    }
}