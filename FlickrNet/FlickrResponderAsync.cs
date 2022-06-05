using FlickrNet.Enums;
using FlickrNet.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNet
{
    public static partial class FlickrResponder
    {
        /// <summary>
        /// Gets a data response for the given base url and parameters,
        /// either using OAuth or not depending on which parameters were passed in.
        /// </summary>
        /// <param name="flickr">The current instance of the <see cref="Flickr"/> class.</param>
        /// <param name="baseUrl">The base url to be called.</param>
        /// <param name="parameters">A dictionary of parameters.</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetDataResponseAsync(Flickr flickr, string baseUrl, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            bool oAuth = parameters.ContainsKey("oauth_consumer_key");

            if (oAuth)
            {
                return await GetDataResponseOAuthAsync(flickr, baseUrl, parameters, cancellationToken);
            }
            else
            {
                return await GetDataResponseNormalAsync(flickr, baseUrl, parameters, cancellationToken);
            }
        }

        private static async Task<byte[]> GetDataResponseNormalAsync(Flickr flickr, string baseUrl, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            string method = flickr.CurrentService == SupportedService.Zooomr ? "GET" : "POST";

            string data = string.Empty;

            foreach (KeyValuePair<string, string> k in parameters)
            {
                data += k.Key + "=" + UtilityMethods.EscapeDataString(k.Value) + "&";
            }

            if (method == "GET" && data.Length > 2000)
            {
                method = "POST";
            }

            if (method == "GET")
            {
                return await DownloadDataAsync(method, baseUrl + "?" + data, null, null, null, cancellationToken);
            }
            else
            {
                return await DownloadDataAsync(method, baseUrl, data, PostContentType, null, cancellationToken);
            }
        }

        private static async Task<byte[]> GetDataResponseOAuthAsync(Flickr flickr, string baseUrl, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            const string method = "POST";

            // Remove api key if it exists.
            if (parameters.ContainsKey("api_key"))
            {
                parameters.Remove("api_key");
            }

            if (parameters.ContainsKey("api_sig"))
            {
                parameters.Remove("api_sig");
            }

            // If OAuth Access Token is set then add token and generate signature.
            if (!string.IsNullOrEmpty(flickr.OAuthAccessToken) && !parameters.ContainsKey("oauth_token"))
            {
                parameters.Add("oauth_token", flickr.OAuthAccessToken);
            }
            if (!string.IsNullOrEmpty(flickr.OAuthAccessTokenSecret) && !parameters.ContainsKey("oauth_signature"))
            {
                string sig = flickr.OAuthCalculateSignature(method, baseUrl, parameters, flickr.OAuthAccessTokenSecret);
                parameters.Add("oauth_signature", sig);
            }

            // Calculate post data, content header and auth header
            string data = OAuthCalculatePostData(parameters);
            string authHeader = OAuthCalculateAuthHeader(parameters);

            // Download data.
            try
            {
                return await DownloadDataAsync(method, baseUrl, data, PostContentType, authHeader, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                if (ex == null)
                {
                    throw;
                }

                if (ex.StatusCode != HttpStatusCode.BadRequest && ex.StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw;
                }

                throw new OAuthException(ex.Message, ex);
            }
        }

        private static async Task<byte[]> DownloadDataAsync(string method, string baseUrl, string data, string contentType, string authHeader, CancellationToken cancellationToken = default)
        {
            using HttpClient client = new();
            HttpRequestMessage message = new()
            {
                RequestUri = new Uri(baseUrl)
            };

            if (!string.IsNullOrEmpty(authHeader))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", authHeader.Replace("OAuth ", ""));
            }

            if (method == "POST")
            {
                message.Method = HttpMethod.Post;
                message.Content = new StringContent(data);
                if (!string.IsNullOrEmpty(contentType))
                {
                    message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                }
            }
            else
            {
                message.Method = HttpMethod.Get;
            }
            HttpResponseMessage response = await client.SendAsync(message, cancellationToken);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                throw;
            }

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }
    }
}