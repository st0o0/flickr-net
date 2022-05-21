using FlickrNet.CollectionModels;
using FlickrNet.Flickrs.Results;
using System;
using System.Collections.Generic;

namespace FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// Gets an array of supported method names for Flickr.
        /// </summary>
        /// <remarks>
        /// Note: Not all methods might be supported by the FlickrNet Library.</remarks>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        public void ReflectionGetMethodsAsync(Action<FlickrResult<MethodCollection>> callback)
        {
            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.reflection.getMethods" }
            };

            GetResponseAsync<MethodCollection>(parameters, callback);
        }

        /// <summary>
        /// Gets the method details for a given method.
        /// </summary>
        /// <param name="methodName">The name of the method to retrieve.</param>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        public void ReflectionGetMethodInfoAsync(string methodName, Action<FlickrResult<Method>> callback)
        {
            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.reflection.getMethodInfo" },
                { "api_key", apiKey },
                { "method_name", methodName }
            };

            GetResponseAsync<Method>(parameters, callback);
        }
    }
}