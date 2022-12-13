using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlCardCreator.Models.NotificationsModels.StructureModels
{
    [XmlRoot("documentPublished"), Serializable]
    public class DocumentPublishedModel
    {
        [XmlIgnore]
        private DateTime _NotificationSendDateTime { get; set; }
        /// <summary>
        /// Время отправки уведомления об опубликовании
        /// </summary>
        [XmlElement(ElementName = "time")]
        public string NotificationSendDateTime
        {
            get { return _NotificationSendDateTime.ToString(XmlDataFormats.DateTimeFormat); }
            set { _NotificationSendDateTime = DateTime.Parse(value); }
        }

        /// <summary>
        /// Наименование органа чей документ был опубликован
        /// </summary>
        [XmlElement(ElementName = "foundation")]
        public FoundationModel Foundation { get; set; }

        /// <summary>
        /// Регион и организация отправившая уведомление
        /// </summary>
        [XmlElement(ElementName = "correspondent")]
        public CorrespondentModel Correspondent { get; set; }
        /// <summary>
        /// Дата и номер опубликования
        /// </summary>
        [XmlElement(ElementName = "num")]
        public DateAndNumberModel PublicationDateAndNumber { get; set; }
        /// <summary>
        /// Точка публикации документа (Официальный интернет-портал правовой информации (www.pravo.gov.ru))
        /// </summary>
        [XmlElement(ElementName = "publicationPoint")]
        public string PublicationPoint { get; set; }
    }
}
