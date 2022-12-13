using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XmlCardCreator.Models.NotificationsModels.StructureModels;

namespace XmlCardCreator.Models.NotificationModels
{
    [XmlRoot("communication", Namespace = "http://www.infpres.com/IEDMS", IsNullable = false), Serializable]
    public class PublicationNotificationModel
    {

        //[XmlNamespaceDeclarations]
        //public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();

        [XmlAttribute("version", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string Version { get; set; }
        public PublicationNotificationModel() { }
        public PublicationNotificationModel(HeaderModel header, NotificationModel notification)
        {
            //xmlns.Add("", "");
            //xmlns.Add("xdms", "http://www.infpres.com/IEDMS");
            Header = header;
            Notification = notification;
            Version = "1.0";
        }
        [XmlElement("header")]
        public HeaderModel Header { get; set; }

        [XmlElement("notification")]
        public NotificationModel Notification {get;set;}
    }
    

}
