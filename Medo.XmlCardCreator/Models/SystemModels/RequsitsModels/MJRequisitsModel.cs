using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XmlCardCreator.Interfaces.SystemInterfaces;

namespace XmlCardCreator.Models.SystemModels.RequsitsModels
{
    [XmlRoot("MJRequisits"), Serializable]
    public class MJRequisitsModel : IMJRequisites
    {
        [XmlElement(ElementName = "MJRegistrationNumber")]
        public int RegistrationNumber { get; set; }
        [XmlElement(ElementName = "MJRegistrationDate")]
        public DateTime? RegistrationDate { get; set; }
    }
}
