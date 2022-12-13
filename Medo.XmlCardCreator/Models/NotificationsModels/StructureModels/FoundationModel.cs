using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlCardCreator.Models.NotificationsModels.StructureModels
{
    [XmlRoot("foundation"), Serializable]
    public class FoundationModel
    {
        /// <summary>
        /// организация чей докумет был опубликован
        /// </summary>
        [XmlElement(ElementName = "organization")]
        public string Organization { get; set; }
        /// <summary>
        /// Вид акта (не используется в стандартной схеме МЭДО сделано специально для Системы)
        /// </summary>
        [XmlElement(ElementName = "acttype")]
        public string ActType { get; set; }
        /// <summary>
        /// Дата подписания и номер опубликованного документа
        /// </summary>
        [XmlElement(ElementName = "num")]
        public DateAndNumberModel DateAndNumber { get; set; }

        public bool ShouldSerializeActType()
        {
            return !string.IsNullOrEmpty(ActType);
        }
    }
}
