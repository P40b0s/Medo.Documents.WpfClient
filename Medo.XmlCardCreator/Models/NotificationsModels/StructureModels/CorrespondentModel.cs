using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlCardCreator.Models.NotificationsModels.StructureModels
{
    [XmlRoot("correspondent"), Serializable]
    public class CorrespondentModel
    {
        /// <summary>
        /// Регион субъекта отправившего уведомление (Москва)
        /// </summary>
        [XmlElement(ElementName = "region")]
        public string Region { get; set; }
        /// <summary>
        /// Организация отправившая уведомление (Управление обеспечения правовой информатизации Службы специальной связи Федеральной службы охраны Российской Федерации (УОПИ Спецсвязи ФСО России))
        /// </summary>
        [XmlElement(ElementName = "organization")]
        public string Organization { get; set; }
    }
}
