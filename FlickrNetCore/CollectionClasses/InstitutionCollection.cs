using FlickrNetCore.Classes;
using FlickrNetCore.Classes.Interfaces;
using System.Collections.ObjectModel;

namespace FlickrNetCore.CollectionClasses
{
    /// <summary>
    /// A collection of <see cref="Institution"/> instances.
    /// </summary>
    public sealed class InstitutionCollection : Collection<Institution>, IFlickrParsable
    {
        void IFlickrParsable.Load(System.Xml.XmlReader reader)
        {
            reader.Read();

            while (reader.LocalName == "institution")
            {
                Institution service = new();
                ((IFlickrParsable)service).Load(reader);
                Add(service);
            }
        }
    }
}