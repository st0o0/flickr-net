﻿using FlickrNet.CollectionModels;
using FlickrNet.Flickrs.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// Gets a list of camera brands.
        /// </summary>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        /// <returns></returns>
        public async Task<CameraCollection> CamerasGetBrandsAsync(CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> parameters = new() { { "method", "flickr.cameras.getBrands" } };
            var t = await GetResponseAsync<CameraCollection>(parameters, cancellationToken);
            if (t.HasError)
            {
                throw t.Error;
            }

            return t.Result;
        }

        /// <summary>
        /// Get a list of camera models for a particular brand id.
        /// </summary>
        /// <param name="brandId">The ID of the brand you want the models of.</param>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        /// <returns></returns>
        public async Task<CameraCollection> CamerasGetBrandModelsAsync(string brandId, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> parameters = new()
            {
                {"method", "flickr.cameras.getBrandModels"},
                { "brand", brandId}
        };

            var t = await GetResponseAsync<CameraCollection>(parameters, cancellationToken);
            if (t.HasError)
            {
                throw t.Error;
            }

            return t.Result;
        }
    }
}