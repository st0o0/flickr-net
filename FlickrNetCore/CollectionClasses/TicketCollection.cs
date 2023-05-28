using FlickrNetCore.Classes;
using FlickrNetCore.Classes.Interfaces;
using System.Collections.ObjectModel;

namespace FlickrNetCore.CollectionClasses
{
    /// <summary>
    /// A collection of <see cref="Ticket"/> instances.
    /// </summary>
    public sealed class TicketCollection : Collection<Ticket>, IFlickrParsable
    {
        void IFlickrParsable.Load(System.Xml.XmlReader reader)
        {
            reader.Read();

            while (reader.LocalName == "ticket")
            {
                Ticket ticket = new();
                ((IFlickrParsable)ticket).Load(reader);
                Add(ticket);
            }

            reader.Skip();
        }
    }
}