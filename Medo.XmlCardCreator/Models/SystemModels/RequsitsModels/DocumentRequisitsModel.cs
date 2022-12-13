using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XmlCardCreator.Interfaces.SystemInterfaces;

namespace XmlCardCreator.Models.SystemModels.RequsitsModels
{
    [XmlRoot("DocumentRequisites"), Serializable]
    public class DocumentRequisitsModel : IDocumentRequisits
    {
        [XmlAttribute("MedoGuid")]
        public Guid MedoGuid { get; set; }

        [XmlAttribute("MedoSourceGuid")]
        public Guid SourceGuid { get; set; }

        [XmlElement(ElementName = "OrganName")]
        public string OrganName { get; set; }

        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "Number")]
        public string Number { get; set; }

        [XmlElement(ElementName = "SignDate")]
        public DateTime SignDate { get; set; }

        [XmlElement(ElementName = "SignPerson")]
        public string SignPerson { get; set; }
    }
}
