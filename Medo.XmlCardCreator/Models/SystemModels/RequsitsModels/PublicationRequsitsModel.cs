using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XmlCardCreator.Interfaces.SystemInterfaces;

namespace XmlCardCreator.Models.SystemModels.RequsitsModels
{
    [XmlRoot("PublicationRequsits"), Serializable]
    public class PublicationRequsitsModel : IPublicationRequsits
    {
        [XmlAttribute("IzdanieId")]
        public Guid IzdanieId { get; set; }

        [XmlElement(ElementName = "PublicationNumber")]
        public string EONumber
        {
            get { return _EONumber; }
            set
            {
                _EONumber = value;
                DocumentPublicationLink = $"http://publication.pravo.gov.ru/Document/View/{value}";
            }
        }
        private string _EONumber { get; set; }

        [XmlElement(ElementName = "PublicationDate")]
        public DateTime PublicationDate
        {
            get { return _PublicationDate; }
            set
            {
                _PublicationDate = value;
                DialyPublicationLink = $"http://publication.pravo.gov.ru/Search/Date?date={value.Month}%2F{value.Day}%2F{value.Year}%2000%3A00%3A00";
            }
        }
        private DateTime _PublicationDate { get; set; }

        [XmlElement(ElementName = "ComplexName")]
        public string ComplexName { get; set; }

        [XmlAttribute("DialyPublicationLink")]
        public string DialyPublicationLink { get; set; }

        [XmlAttribute("DocumentPublicationLink")]
        public string DocumentPublicationLink { get; set; }
    }
}
