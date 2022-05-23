using FlickrNet.CollectionModels;
using FlickrNet.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// Gets information about a collection. Requires authentication with 'read' access.
        /// </summary>
        /// <param name="collectionId">The ID for the collection to return.</param>
        public async Task<CollectionInfo> CollectionsGetInfoAsync(string collectionId, CancellationToken cancellationToken = default)
        {
            CheckRequiresAuthentication();

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.collections.getInfo" },
                { "collection_id", collectionId }
            };
            return await GetResponseAsync<CollectionInfo>(parameters, cancellationToken);
        }

        /// <summary>
        /// Gets a tree of collection. Requires authentication.
        /// </summary>
        public async Task<CollectionCollection> CollectionsGetTreeAsync(CancellationToken cancellationToken = default)
        {
            return await CollectionsGetTreeAsync(null, null, cancellationToken);
        }

        /// <summary>
        /// Gets a tree of collection.
        /// </summary>
        /// <param name="collectionId ">The ID of the collection to fetch a tree for, or zero to fetch the root collection.</param>
        /// <param name="userId">The ID of the user to fetch the tree for, or null if using the authenticated user.</param>
        public async Task<CollectionCollection> CollectionsGetTreeAsync(string collectionId, string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                CheckRequiresAuthentication();
            }

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.collections.getTree" }
            };

            if (collectionId != null)
            {
                parameters.Add("collection_id", collectionId);
            }

            if (userId != null)
            {
                parameters.Add("user_id", userId);
            }
            return await GetResponseAsync<CollectionCollection>(parameters, cancellationToken);
        }
    }
}