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
        /// Gets a list of blogs that have been set up by the user.
        /// Requires authentication.
        /// </summary>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        /// <remarks></remarks>
        public async Task<BlogCollection> BlogsGetListAsync(CancellationToken cancellationToken = default)
        {
            CheckRequiresAuthentication();

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.blogs.getList" }
            };

            var t = await GetResponseAsync<BlogCollection>(parameters, cancellationToken);
            if (t.HasError)
            {
                throw t.Error;
            }

            return t.Result;
        }

        /// <summary>
        /// Return a list of Flickr supported blogging services.
        /// </summary>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        public async Task<BlogServiceCollection> BlogsGetServicesAsync(CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.blogs.getServices" }
            };

            var t = await GetResponseAsync<BlogServiceCollection>(parameters, cancellationToken);

            if (t.HasError)
            {
                throw t.Error;
            }

            return t.Result;
        }

        /// <summary>
        /// Posts a photo already uploaded to a blog.
        /// Requires authentication.
        /// </summary>
        /// <param name="blogId">The Id of the blog to post the photo too.</param>
        /// <param name="photoId">The Id of the photograph to post.</param>
        /// <param name="title">The title of the blog post.</param>
        /// <param name="description">The body of the blog post.</param>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        public async Task<NoResponse> BlogsPostPhotoAsync(string blogId, string photoId, string title, string description, CancellationToken cancellationToken = default)
        {
            return await BlogsPostPhotoAsync(blogId, photoId, title, description, null, cancellationToken);
        }

        /// <summary>
        /// Posts a photo already uploaded to a blog.
        /// Requires authentication.
        /// </summary>
        /// <param name="blogId">The Id of the blog to post the photo too.</param>
        /// <param name="photoId">The Id of the photograph to post.</param>
        /// <param name="title">The title of the blog post.</param>
        /// <param name="description">The body of the blog post.</param>
        /// <param name="blogPassword">The password of the blog if it is not already stored in flickr.</param>
        /// <param name="callback">Callback method to call upon return of the response from Flickr.</param>
        public async Task<NoResponse> BlogsPostPhotoAsync(string blogId, string photoId, string title, string description, string blogPassword, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.blogs.postPhoto" },
                { "blog_id", blogId },
                { "photo_id", photoId },
                { "title", title },
                { "description", description }
            };
            if (blogPassword != null)
            {
                parameters.Add("blog_password", blogPassword);
            }

            var t = await GetResponseAsync<NoResponse>(parameters, cancellationToken);

            if (t.HasError)
            {
                throw t.Error;
            }

            return t.Result;
        }
    }
}