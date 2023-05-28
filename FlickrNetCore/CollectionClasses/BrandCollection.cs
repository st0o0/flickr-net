using FlickrNetCore.Classes;
using FlickrNetCore.Classes.Interfaces;
using FlickrNetCore.Common;
using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNetCore.CollectionClasses
{
    /// <summary>
    /// A collection of camera brands
    /// </summary>
    public class BrandCollection : Collection<Brand>, IFlickrParsable
    {
        void IFlickrParsable.Load(XmlReader reader)
        {
            if (reader.LocalName != "brands")
            {
                UtilityMethods.CheckParsingException(reader);
            }

            reader.Read();

            while (reader.LocalName == "brand")
            {
                Brand b = new();
                ((IFlickrParsable)b).Load(reader);
                Add(b);
            }

            // Skip to next element (if any)
            reader.Skip();
        }
    }
}