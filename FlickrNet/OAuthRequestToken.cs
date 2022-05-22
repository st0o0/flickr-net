﻿using System.Collections.Generic;

namespace FlickrNet
{
    /// <summary>
    /// Class containing details of the OAUth request token returned by Flickr.
    /// </summary>
    public class OAuthRequestToken
    {
        /// <summary>
        /// The request token string.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The request token secret.
        /// </summary>
        public string TokenSecret { get; set; }

        /// <summary>
        /// Parses a URL parameter encoded string and returns a new <see cref="OAuthRequestToken"/>
        /// </summary>
        /// <param name="response">A URL parameter encoded string, e.g. "oauth_token=ABC&amp;oauth_token_secret=DEF".</param>
        /// <returns></returns>
        public static OAuthRequestToken ParseResponse(byte[] response)
        {
            Dictionary<string, string> parameters = UtilityMethods.StringToDictionary(response);
            OAuthRequestToken token = new();
            token.Token = parameters["oauth_token"];
            token.TokenSecret = parameters["oauth_token_secret"];
            return token;
        }
    }
}