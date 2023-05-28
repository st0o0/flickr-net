using System.Net;

namespace FlickrNetCore.Test.TestUtilities
{
    public static class UrlHelper
    {
        //public static bool Exists(string url)
        //{
        //    var req = (HttpWebRequest)WebRequest.Create(url);
        //    req.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.BypassCache);
        //    req.AllowAutoRedirect = false;
        //    req.Method = "HEAD";

        //    try
        //    {
        //        using var res = (HttpWebResponse)req.GetResponse();
        //        return res.StatusCode == HttpStatusCode.OK;
        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine(exception.GetType() + " thrown.");
        //        Console.WriteLine("Message:" + exception.Message);
        //        return false;
        //    }
        //}

        public static async Task<bool> Exists(string url, CancellationToken cancellationToken = default)
        {
            using HttpClient client = new();
            HttpResponseMessage result = await client.GetAsync(url, cancellationToken);
            return result.StatusCode switch
            {
                HttpStatusCode.Accepted => true,
                HttpStatusCode.OK => true,
                _ => false,
            };
        }
    }
}