using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XmlCardCreator.Models.SystemModels.RequsitsModels;

namespace XmlCardCreator.Models.SystemModels
{
    [XmlRoot("PublicationReport", Namespace = "http://publication.pravo.gov.ru", IsNullable = false), Serializable]
    public class SystemaPublicationReportModel
    {
        public SystemaPublicationReportModel(){ }
        public SystemaPublicationReportModel(DocumentRequisitsModel requsits, MJRequisitsModel mjrequisits, PublicationRequsitsModel publ)
        {
            document = requsits;
            mj = mjrequisits;
            publication = publ;
        }
        [XmlElement(ElementName = "DocumentRequisites")]
        public DocumentRequisitsModel document { get; set; }

        [XmlElement(ElementName = "MinJustRequisites")]
        public MJRequisitsModel mj { get; set; }

        [XmlElement(ElementName = "PublicationRequisites")]
        public PublicationRequsitsModel publication {get;set;}
    }
}
