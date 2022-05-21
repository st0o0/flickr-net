using FlickrNet.Models;
using FlickrNet.Models.Interfaces;

namespace FlickrNet.CollectionModels
{
    /// <summary>
    /// A class which encapsulates a single property, an array of
    /// <see cref="License"/> objects in its <see cref="LicenseCollection"/> property.
    /// </summary>
    public sealed class LicenseCollection : System.Collections.ObjectModel.Collection<License>, IFlickrParsable
    {
        void IFlickrParsable.Load(System.Xml.XmlReader reader)
        {
            reader.Read();

            while (reader.LocalName == "license")
            {
                License license = new();
                ((IFlickrParsable)license).Load(reader);
                Add(license);
            }

            reader.Skip();
        }
    }
}