using FlickrNet.Flickrs.Results;
using FlickrNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS0618 // Type or member is obsolete

namespace FlickrNet
{
    public partial class Flickr
    {
        private async Task<FlickrResult<T>> GetResponseAsync<T>(Dictionary<string, string> parameters, CancellationToken cancellationToken = default) where T : IFlickrParsable, new()
        {
            CheckApiKey();

            parameters["api_key"] = ApiKey;

            // If performing one of the old 'flickr.auth' methods then use old authentication details.
            string method = parameters["method"];

            // If OAuth Token exists or no authentication required then use new OAuth
            if (!string.IsNullOrEmpty(OAuthAccessToken))
            {
                OAuthGetBasicParameters(parameters);
                if (!string.IsNullOrEmpty(OAuthAccessToken))
                {
                    parameters["oauth_token"] = OAuthAccessToken;
                }
            }
            else
            {
                parameters["oauth_token"] = OAuthAccessToken;
            }

            string url = CalculateUri(parameters, !string.IsNullOrEmpty(sharedSecret));

            lastRequest = url;

            FlickrResult<T> result = new();
            try
            {
                FlickrResult<byte[]> r = await FlickrResponder.GetDataResponseAsync(this, BaseUri.AbsoluteUri, parameters, cancellationToken);
                if (r.HasError)
                {
                    result.Error = r.Error;
                }
                else
                {
                    try
                    {
                        lastResponse = r.Result;

                        T t = new();
                        ((IFlickrParsable)t).Load(r.Result);
                        result.Result = t;
                        result.HasError = false;
                    }
                    catch (Exception ex)
                    {
                        result.Error = ex;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }
            return result;
        }
    }
}