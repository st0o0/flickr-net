using FlickrNet.Common;
using FlickrNet.Enums;
using FlickrNet.Exceptions;
using FlickrNet.Exceptions.Handlers;
using FlickrNet.HttpContents;
using FlickrNet.Models.Interfaces;
using FlickrNet.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Xml;

namespace FlickrNet
{
    /// <summary>
    /// The main Flickr class.
    /// </summary>
    /// <remarks>
    /// Create an instance of this class and then call its methods to perform methods on Flickr.
    /// </remarks>
    /// <example>
    /// <code>
    /// FlickrNet.Flickr flickr = new FlickrNet.Flickr();
    /// User user = flickr.PeopleFindByEmail("cal@iamcal.com");
    /// Console.WriteLine("User Id is " + u.UserId);
    /// </code>
    /// </example>
    // [System.Net.WebPermission(System.Security.Permissions.SecurityAction.Demand, ConnectPattern="http://www.flickr.com/.*")]
    public partial class Flickr
    {
        /// <summary>
        ///
        /// </summary>
        /// <summary>
        /// UploadProgressHandler is fired during a synchronous upload process to signify that
        /// a segment of uploading has been completed. This is approximately 50 bytes. The total
        /// uploaded is recorded in the <see cref="UploadProgressEventArgs"/> class.
        /// </summary>
        public event EventHandler<UploadProgressEventArgs> OnUploadProgress;

        private static SupportedService _defaultService = SupportedService.Flickr;

        private SupportedService _service = SupportedService.Flickr;

        /// <summary>
        /// The base URL for all Flickr REST method calls.
        /// </summary>
        public Uri BaseUri => _baseUris[(int)_service];

        private readonly Uri[] _baseUris = {
            new Uri("https://api.flickr.com/services/rest/"),
            new Uri("http://beta.zooomr.com/bluenote/api/rest"),
            new Uri("http://www.23hq.com/services/rest/")
        };

        private string _uploadUrl
        {
            get { return _uploadUrls[(int)_service]; }
        }

        private static string[] _uploadUrls = {
            "https://up.flickr.com/services/upload/",
            "http://beta.zooomr.com/bluenote/api/upload",
            "http://www.23hq.com/services/upload/"
        };

        private string _replaceUrl
        {
            get { return _replaceUrls[(int)_service]; }
        }

        private static string[] _replaceUrls = {
            "https://up.flickr.com/services/replace/",
            "http://beta.zooomr.com/bluenote/api/replace",
            "http://www.23hq.com/services/replace/"
        };

        private string _authUrl => _authUrls[(int)_service];

        private static string[] _authUrls = {
            "https://www.flickr.com/services/auth/",
            "http://beta.zooomr.com/auth/",
            "http://www.23hq.com/services/auth/"
        };

        private string _apiKey;
        private string _apiToken;
        private string _sharedSecret;

        private int _timeout = 3600000;  // (Andrew Keil) Changed to 1 hour in milliseconds to avoid timeout issues when uploading picture & videos

        private string _lastRequest;
        private byte[] _lastResponse;

        /// <summary>
        /// Get or set the API Key to be used by all calls. API key is mandatory for all
        /// calls to Flickr.
        /// </summary>
        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                _apiKey = value == null || value.Length == 0 ? null : value;
            }
        }

        /// <summary>
        /// API shared secret is required for all calls that require signing, which includes
        /// all methods that require authentication, as well as the actual flickr.auth.* calls.
        /// </summary>
        public string ApiSecret
        {
            get { return _sharedSecret; }
            set
            {
                _sharedSecret = value == null || value.Length == 0 ? null : value;
            }
        }

        /// <summary>
        /// OAuth Access Token. Needed for authenticated access using OAuth to Flickr.
        /// </summary>
        public string OAuthAccessToken { get; set; }

        /// <summary>
        /// OAuth Access Token Secret. Needed for authenticated access using OAuth to Flickr.
        /// </summary>
        public string OAuthAccessTokenSecret { get; set; }

        /// <summary>
        /// Gets or sets whether the cache should be disabled. Use only in extreme cases where you are sure you
        /// don't want any caching.
        /// </summary>
        public static bool CacheDisabled
        {
            get { return Cache.CacheDisabled; }
            set { Cache.CacheDisabled = value; }
        }

        /// <summary>
        /// Override if the cache is disabled for this particular instance of <see cref="Flickr"/>.
        /// </summary>
        public bool InstanceCacheDisabled { get; set; }

        /// <summary>
        /// All GET calls to Flickr are cached by the Flickr.Net API. Set the <see cref="CacheTimeout"/>
        /// to determine how long these calls should be cached (make this as long as possible!)
        /// </summary>
        public static TimeSpan CacheTimeout
        {
            get { return Cache.CacheTimeout; }
            set { Cache.CacheTimeout = value; }
        }

        /// <summary>
        /// Sets or gets the location to store the Cache files.
        /// </summary>
        public static string CacheLocation
        {
            get { return Cache.CacheLocation; }
            set { Cache.CacheLocation = value; }
        }

        /// <summary>
        /// <see cref="CacheSizeLimit"/> is the cache file size in bytes for downloaded
        /// pictures. The default is 50MB (or 50 * 1024 * 1025 in bytes).
        /// </summary>
        public static long CacheSizeLimit
        {
            get { return Cache.CacheSizeLimit; }
            set { Cache.CacheSizeLimit = value; }
        }

        /// <summary>
        /// The default service to use for new Flickr instances
        /// </summary>
        public static SupportedService DefaultService
        {
            get
            {
                return _defaultService;
            }
            set
            {
                _defaultService = value;
            }
        }

        /// <summary>
        /// The current service that the Flickr API is using.
        /// </summary>
        public SupportedService CurrentService
        {
            get
            {
                return _service;
            }
            set
            {
                _service = value;
            }
        }

        /// <summary>
        /// Internal timeout for all web requests in milliseconds. Defaults to 30 seconds.
        /// </summary>
        public int HttpTimeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Checks to see if a shared secret and an api token are stored in the object.
        /// Does not check if these values are valid values.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return _sharedSecret != null && _apiToken != null;
            }
        }

        /// <summary>
        /// Returns the raw XML returned from the last response.
        /// Only set it the response was not returned from cache.
        /// </summary>
        public byte[] LastResponse
        {
            get { return _lastResponse; }
        }

        /// <summary>
        /// Returns the last URL requested. Includes API signing.
        /// </summary>
        public string LastRequest
        {
            get { return _lastRequest; }
        }

        /// <summary>
        /// Clears the cache completely.
        /// </summary>
        public static void FlushCache()
        {
            Cache.FlushCache();
        }

        /// <summary>
        /// Clears the cache for a particular URL.
        /// </summary>
        /// <param name="url">The URL to remove from the cache.</param>
        /// <remarks>
        /// The URL can either be an image URL for a downloaded picture, or
        /// a request URL (see <see cref="LastRequest"/> for getting the last URL).
        /// </remarks>
        public static void FlushCache(string url)
        {
            FlushCache(new Uri(url));
        }

        /// <summary>
        /// Clears the cache for a particular URL.
        /// </summary>
        /// <param name="url">The URL to remove from the cache.</param>
        /// <remarks>
        /// The URL can either be an image URL for a downloaded picture, or
        /// a request URL (see <see cref="LastRequest"/> for getting the last URL).
        /// </remarks>
        public static void FlushCache(Uri url)
        {
            Cache.FlushCache(url);
        }

        /// <summary>
        /// Create a new instance of the <see cref="Flickr"/> class with no authentication.
        /// </summary>
        /// <param name="apiKey">Your Flickr API Key.</param>
        public Flickr(string apiKey) : this(apiKey, null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Flickr"/> class with an API key and a Shared Secret.
        /// This is only useful really useful for calling the Auth functions as all other
        /// authenticationed methods also require the API Token.
        /// </summary>
        /// <param name="apiKey">Your Flickr API Key.</param>
        /// <param name="sharedSecret">Your Flickr Shared Secret.</param>
        public Flickr(IFlickrSettings flickrSettings) : this(flickrSettings.ApiKey, flickrSettings.SharedSecret)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Flickr"/> class with an API key and a Shared Secret.
        /// This is only useful really useful for calling the Auth functions as all other
        /// authenticationed methods also require the API Token.
        /// </summary>
        /// <param name="apiKey">Your Flickr API Key.</param>
        /// <param name="sharedSecret">Your Flickr Shared Secret.</param>
        public Flickr(string apiKey, string sharedSecret)
        {
            this._apiKey = apiKey;
            this._sharedSecret = sharedSecret;
        }

        public void CheckApiKey()
        {
            if (string.IsNullOrEmpty(ApiKey))
            {
                throw new ApiKeyRequiredException();
            }
        }

        public void CheckSigned()
        {
            CheckApiKey();

            if (string.IsNullOrEmpty(ApiSecret))
            {
                throw new SignatureRequiredException();
            }
        }

        public void CheckRequiresAuthentication()
        {
            CheckSigned();

            if (!string.IsNullOrEmpty(OAuthAccessToken) && !string.IsNullOrEmpty(OAuthAccessTokenSecret))
            {
                return;
            }

            if (!string.IsNullOrEmpty(OAuthAccessTokenSecret))
            {
                return;
            }

            throw new AuthenticationRequiredException();
        }

        /// <summary>
        /// Calculates the Flickr method cal URL based on the passed in parameters, and also generates the signature if required.
        /// </summary>
        /// <param name="parameters">A Dictionary containing a list of parameters to add to the method call.</param>
        /// <param name="includeSignature">Boolean use to decide whether to generate the api call signature as well.</param>
        /// <returns>The <see cref="Uri"/> for the method call.</returns>
        public string CalculateUri(Dictionary<string, string> parameters, bool includeSignature)
        {
            if (includeSignature)
            {
                string signature = CalculateAuthSignature(parameters);
                parameters.Add("api_sig", signature);
            }

            StringBuilder url = new();
            url.Append('?');
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                string escapedValue = UtilityMethods.EscapeDataString(pair.Value ?? "");
                url.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0}={1}&", pair.Key, escapedValue);
            }

            return BaseUri.AbsoluteUri + url.ToString();
        }

        private string CalculateAuthSignature(Dictionary<string, string> parameters)
        {
            SortedList<string, string> sorted = new();
            foreach (KeyValuePair<string, string> pair in parameters) { sorted.Add(pair.Key, pair.Value); }

            StringBuilder sb = new(ApiSecret);
            foreach (KeyValuePair<string, string> pair in sorted)
            {
                sb.Append(pair.Key);
                sb.Append(pair.Value);
            }
            return UtilityMethods.MD5Hash(sb.ToString());
        }

        private MultipartFormDataContent CreateUploadData(Stream imageStream, string fileName, IProgress<double> progress, Dictionary<string, string> parameters, string boundary, CancellationToken cancellationToken = default)
        {
            //bool oAuth = parameters.ContainsKey("oauth_consumer_key");

            MultipartFormDataContent content = new(boundary)
            {
                { new StreamedContent(imageStream, progress, cancellationToken), "photo", Path.GetFileName(fileName) }
            };

            foreach (KeyValuePair<string, string> i in parameters)
            {
                if (i.Key.StartsWith("oauth", StringComparison.Ordinal))
                {
                    continue;
                }
                content.Add(new StringContent(i.Value), i.Key);
            }

            return content;

            // OLD:
            //string[] keys = new string[parameters.Keys.Count];
            //parameters.Keys.CopyTo(keys, 0);
            //Array.Sort(keys);

            //StringBuilder hashStringBuilder = new(sharedSecret, 2 * 1024);
            //MemoryStream ms1 = new();
            //StreamWriter contentStringBuilder = new(ms1, new UTF8Encoding(false));

            //foreach (string key in keys)
            //{
            //    // Silverlight < 5 doesn't support modification of the Authorization header, so all data must be sent in post body.
            //    if (key.StartsWith("oauth", StringComparison.Ordinal))
            //    {
            //        continue;
            //    }
            //    hashStringBuilder.Append(key);
            //    hashStringBuilder.Append(parameters[key]);
            //    contentStringBuilder.Write("--" + boundary + "\r\n");
            //    contentStringBuilder.Write("Content-Disposition: form-data; name=\"" + key + "\"\r\n");
            //    contentStringBuilder.Write("\r\n");
            //    contentStringBuilder.Write(parameters[key] + "\r\n");
            //}

            //if (!oAuth)
            //{
            //    contentStringBuilder.Write("--" + boundary + "\r\n");
            //    contentStringBuilder.Write("Content-Disposition: form-data; name=\"api_sig\"\r\n");
            //    contentStringBuilder.Write("\r\n");
            //    contentStringBuilder.Write(UtilityMethods.MD5Hash(hashStringBuilder.ToString()) + "\r\n");
            //}

            //// Photo
            //contentStringBuilder.Write("--" + boundary + "\r\n");
            //contentStringBuilder.Write("Content-Disposition: form-data; name=\"photo\"; filename=\"" + Path.GetFileName(fileName) + "\"\r\n");
            //contentStringBuilder.Write("Content-Type: image/jpeg\r\n");
            //contentStringBuilder.Write("\r\n");

            //contentStringBuilder.Flush();

            //Stream photoContents = ConvertNonSeekableStreamToByteArray(imageStream);

            //MemoryStream ms2 = new();
            //StreamWriter postFooterWriter = new(ms2, new UTF8Encoding(false));
            //postFooterWriter.Write("\r\n--" + boundary + "--\r\n");
            //postFooterWriter.Flush();

            //StreamCollection collection = new(new[] { ms1, photoContents, ms2 });
        }

        private static Stream ConvertNonSeekableStreamToByteArray(Stream nonSeekableStream)
        {
            if (nonSeekableStream.CanSeek)
            {
                nonSeekableStream.Position = 0;
                return nonSeekableStream;
            }

            return nonSeekableStream;
        }
    }

    internal static class FlickrExtensions
    {
        public static void Load(this IFlickrParsable item, byte[] bytes)
        {
            try
            {
                using MemoryStream ms = new(bytes);
                using XmlReader reader = XmlReader.Create(ms, new XmlReaderSettings
                {
                    IgnoreWhitespace = true
                });

                if (!reader.ReadToDescendant("rsp"))
                {
                    throw new Exception("Unable to find response element 'rsp' in Flickr response");
                }
                while (reader.MoveToNextAttribute())
                {
                    if (reader.LocalName == "stat" && reader.Value == "fail")
                    {
                        throw ExceptionHandler.CreateResponseException(reader);
                    }
                }

                reader.MoveToElement();
                reader.Read();

                item.Load(reader);
            }
            catch (XmlException)
            {
                throw;
            }
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        private static bool IsLegalXmlChar(int character)
        {
            return
            (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }
    }
}