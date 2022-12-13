using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlCardCreator.Models.NotificationsModels.StructureModels
{
    [XmlRoot("num"), Serializable]
    public class DateAndNumberModel
    {
        [XmlIgnore]
        private DateTime _Date { get; set; }
        [XmlIgnore]
        public DateTime? _MJDate { get; set; }
        [XmlIgnore]
        private bool IsDataOnly = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateFormat">date</param>
        public DateAndNumberModel(bool OnlyData)
        {
            IsDataOnly = true;
        }
        public DateAndNumberModel() { }

        [XmlElement(ElementName = "number")]
        public string Number { get; set; }

        [XmlElement(ElementName = "date")]
        public string Date
        {
            get
            {
                if (IsDataOnly)
                {
                    return _Date.ToString(XmlDataFormats.DateFormat);
                }
                else
                {
                    return _Date.ToString(XmlDataFormats.DateTimeFormat);
                }
            }
            set { _Date = DateTime.Parse(value); }
        }

        /// <summary>
        /// Номер регистрации в минюсте (не используется в стандартной схеме МЭДО сделано специально для Системы)
        /// </summary>
        [XmlElement(ElementName = "mjnumber")]
        public int MJNumber { get; set; }
        /// <summary>
        /// Дана регистрации в минюсте (не используется в стандартной схеме МЭДО сделано специально для Системы)
        /// </summary>
        [XmlElement(ElementName = "mjdate")]
        public string MJDate
        {
            get
            {
                if (_MJDate.HasValue)
                {
                    return _MJDate.Value.ToString(XmlDataFormats.DateFormat);
                }
                else
                    return null;
            }
            set { _MJDate = DateTime.Parse(value); }
        }

        public bool ShouldSerializeMJNumber()
        {
            return MJNumber > 0;
        }
        public bool ShouldSerializeMJDate()
        {
            return _MJDate.HasValue;
        }


    }
}
