using FlickrNet.Settings.Interfaces;

namespace FlickrNet.Settings
{
    public class FlickrSettings : IFlickrSettings
    {
        public string ApiKey { get; set; }

        public string SharedSecret { get; set; }
    }
}