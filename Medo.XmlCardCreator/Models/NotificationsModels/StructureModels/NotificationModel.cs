using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XmlCardCreator.Enums;

namespace XmlCardCreator.Models.NotificationsModels.StructureModels
{
    [XmlRoot("notification"), Serializable]
    public class NotificationModel
    {
        /// <summary>
        /// Тип уведомления об опубликовании (Опубликование , возможно было бы еще что-то типа Отзыв, Замена и т.д.)
        /// </summary>
        [XmlAttribute("type", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public NotificationType Type { get; set; }

        /// <summary>
        /// Id уведомления (Используется MedoGuid опубликованного документа)
        /// </summary>
        [XmlAttribute("uid", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public Guid NotificationGuid { get; set; }

        /// <summary>
        /// Блок информации о документа
        /// </summary>
        [XmlElement(ElementName = "documentPublished")]
        public DocumentPublishedModel Published { get; set; }

        /// <summary>
        /// Комментарий к уведомлению (Сюда записываем составное название из Издания)
        /// </summary>
        [XmlElement(ElementName = "comment")]
        public string Comment { get; set; }
    }
}
