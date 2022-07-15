using FlickrNet.Common;
using FlickrNet.Models;
using FlickrNet.Models.Interfaces;
using System.Collections.ObjectModel;

namespace FlickrNet.CollectionModels
{
    /// <summary>
    /// Contains a list of <see cref="Blog"/> items for the user.
    /// </summary>
    public sealed class BlogCollection : Collection<Blog>, IFlickrParsable
    {
        void IFlickrParsable.Load(System.Xml.XmlReader reader)
        {
            if (reader.LocalName != "blogs")
            {
                UtilityMethods.CheckParsingException(reader);
            }

            reader.Read();

            while (reader.LocalName == "blog")
            {
                Blog b = new();
                ((IFlickrParsable)b).Load(reader);
                Add(b);
            }

            // Skip to next element (if any)
            reader.Skip();
        }
    }
}