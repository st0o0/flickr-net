namespace FlickrNet.Settings.Interfaces
{
    public interface IFlickrSettings
    {
        string ApiKey { get; set; }

        string SharedSecret { get; set; }
    }
}