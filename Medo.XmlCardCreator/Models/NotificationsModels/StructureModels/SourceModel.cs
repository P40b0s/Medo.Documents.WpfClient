using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlCardCreator.Models.NotificationsModels.StructureModels
{
    [XmlRoot("source"), Serializable]
    public class SourceModel
    {
        /// <summary>
        /// Id отправителя уведомления в системе МЭДО (1953A86E-E35E-45CC-B031-556CA72C4080)
        /// </summary>
        [XmlAttribute("uid", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public Guid SourceGuid { get; set; }
        /// <summary>
        /// Организация отправившая уведомления (ГПУ почему то попросило чтобы в этом поле был субъект /r/n чей документ был опубликован, а по идее должно быть наименование нашего управления, мы же субъект отправки....)
        /// </summary>
        [XmlElement(ElementName = "organization")]
        public string Organization { get; set; }
    }
}
