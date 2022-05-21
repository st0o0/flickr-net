using FlickrNet.Exceptions;
using FlickrNet.Flickrs.Results;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNet
{
    public partial class Flickr
    {
        // TODO: not finished
        /// <summary>
        /// Get an <see cref="OAuthRequestToken"/> for the given callback URL.
        /// </summary>
        /// <remarks>Specify 'oob' as the callback url for no callback to be performed.</remarks>
        /// <param name="callbackUrl">The callback Uri, or 'oob' if no callback is to be performed.</param>
        /// <param name="cancellationToken"></param>
        public async Task<FlickrResult<OAuthRequestToken>> OAuthGetRequestTokenAsync(string callbackUrl, CancellationToken cancellationToken = default)
        {
            CheckApiKey();

            string url = "https://www.flickr.com/services/oauth/request_token";

            Dictionary<string, string> parameters = OAuthGetBasicParameters();

            parameters.Add("oauth_callback", callbackUrl);

            string sig = OAuthCalculateSignature("POST", url, parameters, null);

            parameters.Add("oauth_signature", sig);

            FlickrResult<OAuthRequestToken> result = new();
            var r = await FlickrResponder.GetDataResponseAsync(this, url, parameters, cancellationToken);
            if (r.Error != null)
            {
                if (r.Error is HttpRequestException httpRequest)
                {
                    if (httpRequest.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        OAuthException ex = new(r.Error);
                        result.Error = ex;
                    }
                    else
                    {
                        result.Error = r.Error;
                    }
                }
                else
                {
                    result.Error = r.Error;
                }
                return result;
            }
            result.Result = OAuthRequestToken.ParseResponse(r.Result);
            return result;
        }

        /// <summary>
        /// Returns an access token for the given request token, secret and authorization verifier.
        /// </summary>
        /// <param name="requestToken"></param>
        /// <param name="verifier"></param>
        /// <param name="callback"></param>
        public async Task<FlickrResult<OAuthAccessToken>> OAuthGetAccessTokenAsync(OAuthRequestToken requestToken, string verifier, CancellationToken cancellationToken = default)
        {
            return await OAuthGetAccessTokenAsync(requestToken.Token, requestToken.TokenSecret, verifier, cancellationToken);
        }

        /// <summary>
        /// For a given request token and verifier string return an access token.
        /// </summary>
        /// <param name="requestToken"></param>
        /// <param name="requestTokenSecret"></param>
        /// <param name="verifier"></param>
        /// <param name="callback"></param>
        public async Task<FlickrResult<OAuthAccessToken>> OAuthGetAccessTokenAsync(string requestToken, string requestTokenSecret, string verifier, CancellationToken cancellationToken = default)
        {
            CheckApiKey();

            string url = "https://www.flickr.com/services/oauth/access_token";

            Dictionary<string, string> parameters = OAuthGetBasicParameters();

            parameters.Add("oauth_verifier", verifier);
            parameters.Add("oauth_token", requestToken);

            string sig = OAuthCalculateSignature("POST", url, parameters, requestTokenSecret);

            parameters.Add("oauth_signature", sig);

            FlickrResponder.GetDataResponseAsync(this, url, parameters, (r) =>
                {
                    FlickrResult<OAuthAccessToken> result = new FlickrResult<OAuthAccessToken>();
                    if (r.Error != null)
                    {
                        if (r.Error is System.Net.WebException)
                        {
                            OAuthException ex = new(r.Error);
                            result.Error = ex;
                        }
                        else
                        {
                            result.Error = r.Error;
                        }

                        callback(result);
                        return;
                    }
                    result.Result = FlickrNet.OAuthAccessToken.ParseResponse(r.Result);
                    callback(result);
                });
        }
    }
}