using FlickrNetCore.Classes;
using FlickrNetCore.Classes.Interfaces;
using FlickrNetCore.Common;
using System.Collections.ObjectModel;

namespace FlickrNetCore.CollectionClasses
{
    /// <summary>
    /// A list of <see cref="ActivityItem"/> items.
    /// </summary>
    public sealed class ActivityItemCollection : Collection<ActivityItem>, IFlickrParsable
    {
        void IFlickrParsable.Load(System.Xml.XmlReader reader)
        {
            if (reader.LocalName != "items")
            {
                UtilityMethods.CheckParsingException(reader);
            }

            reader.Read();

            while (reader.LocalName == "item")
            {
                ActivityItem item = new();
                ((IFlickrParsable)item).Load(reader);
                Add(item);
            }

            reader.Skip();
        }
    }
}