using System;

namespace FlickrNet.Settings.Interfaces
{
    public interface IFlickrSettings
    {
        string ApiKey { get; set; }

        string SharedKey { get; set; }
    }
}