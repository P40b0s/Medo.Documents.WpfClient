using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XmlCardCreator.Enums;

namespace XmlCardCreator.Models.NotificationsModels.StructureModels
{
    [XmlRoot("header"), Serializable]
    public class HeaderModel
    {
        /// <summary>
        /// Id уведомления (привязан к Id Издания)
        /// </summary>
        [XmlAttribute("uid", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public Guid Uid { get; set; }
        /// <summary>
        /// Тип документа (Уведомление)
        /// </summary>
        [XmlAttribute("type", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public DocumentType Type { get; set; }
        /// <summary>
        /// Источник уведомления
        /// </summary>
        [XmlElement(ElementName = "source")]
        public SourceModel Source { get; set; }

    }
}
