using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCardCreator.Interfaces.SystemInterfaces
{
    interface IDocumentRequisits
    {
        Guid MedoGuid { get; set; }
        Guid SourceGuid { get; set; }
        string OrganName { get; set; }
        string Type { get; set; }
        string Number { get; set; }

    }
}
