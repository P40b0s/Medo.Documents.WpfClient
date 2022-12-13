using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client.SedRegistration
{
    /// <summary>
    /// Модель регистрации документа в СЭДе (по умолчанию осуществляется операция регистрации документа)
    /// </summary>
    public class SedModel
    {
        public SedModel() {
            Operation = SedOperationEnum.Register;
            RefuseStatus = null;
        }
        /// <summary>
        /// Номер документа или номер опубликования (при регистрации)
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Дата подписания
        /// </summary>
        public DateTime SignDate { get; set; }
        /// <summary>
        /// GUID органа приславшего документ
        /// </summary>
        public Guid SGuid { get; set; }
        /// <summary>
        /// GUID документа
        /// </summary>
        public Guid DocGuid { get; set; }
        /// <summary>
        /// Какая операция с документом будет осуществлятся (зарегистрировать, удалить, отклонить)
        /// </summary>
        public SedOperationEnum Operation { get; set; }
        /// <summary>
        /// Наименование документа
        /// </summary>
        public string DocumentName { get; set; }
        /// <summary>
        /// Количество страниц
        /// </summary>
        public int PagesCount { get; set; }
        /// <summary>
        /// Статус отказа в регистрации
        /// </summary>
        public string RefuseStatus { get; set; }
        /// <summary>
        /// Коментарий к отказу в регистрации
        /// </summary>
        public string RefuseComment { get; set; }
    }
}
