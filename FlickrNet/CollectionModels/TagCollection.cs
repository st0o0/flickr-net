﻿using FlickrNet.Models;
using FlickrNet.Models.Interfaces;

namespace FlickrNet.CollectionModels
{
    /// <summary>
    /// List containing <see cref="Tag"/> items.
    /// </summary>
    public sealed class TagCollection : System.Collections.ObjectModel.Collection<Tag>, IFlickrParsable
    {
        void IFlickrParsable.Load(System.Xml.XmlReader reader)
        {
            reader.ReadToDescendant("tag");

            while (reader.LocalName == "tag")
            {
                Tag member = new();
                ((IFlickrParsable)member).Load(reader);
                Add(member);
            }

            reader.Skip();
        }
    }
}