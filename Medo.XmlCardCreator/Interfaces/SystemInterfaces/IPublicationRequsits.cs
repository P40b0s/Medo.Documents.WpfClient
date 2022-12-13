using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCardCreator.Interfaces.SystemInterfaces
{
    interface IPublicationRequsits
    {
        Guid IzdanieId { get; set; }
        string EONumber { get; set; }
        DateTime PublicationDate { get; set; }
        string DialyPublicationLink { get; }
        string DocumentPublicationLink { get; }
        string ComplexName { get; set; }
    }
}
