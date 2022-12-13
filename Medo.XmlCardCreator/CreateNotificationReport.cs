using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using XmlCardCreator.Models;
using XmlCardCreator.Models.NotificationModels;
using XmlCardCreator.Models.NotificationsModels.StructureModels;
using XmlCardCreator.Models.SystemModels;
using XmlCardCreator.Enums;

namespace XmlCardCreator
{
    public class CreateNotificationReport
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private PublicationNotificationModel notification { get; set; }

        public CreateNotificationReport() { }
        /// <summary>
        /// Создание уведомления об опубликовании
        /// </summary>
        /// <param name="medoGuid">GUID документа из системы МЭДО</param>
        /// <param name="sourceGuid">Идентификатор МЭДО субъекта отправившего уведомление</param>
        /// <param name="izdanieId">Идентификатор документа в системе Издание</param>
        /// <param name="publicationOrganName">Наменование органа, чей документ был опубликован</param>
        /// <param name="senderOrganName">Наименование органа отправившего уведомление</param>
        /// <param name="senderRegion">Регион субъекта отправившего уведомление</param>
        /// <param name="eoNumber">Номер электронного опубликования опубликованного документа</param>
        /// <param name="publicationPoint"></param>
        /// <param name="complexName">Составное наименование документа из системы Издание</param>
        /// <param name="documentNumber">Номер опубликованного документа</param>
        /// <param name="docType">Тип XML</param>
        /// <param name="notType">Тип уведомления</param>
        /// <param name="sendNotificationDateTime">Время отправки уведомления</param>
        /// <param name="publicationDateTime">Дата опубликования документа в системе Издание</param>
        /// <param name="documentSignDate">Дата подписания опубликованного документа</param>
        /// <param name="mjDate">Дата регистрации в минюсте (используется при необходимости)</param>
        /// <param name="mjNumber">Номер регистрации в минюсте (используется при необходимости)</param>
        /// <param name="actType">Вид документа (используется при необходимости)</param>
        public CreateNotificationReport(Guid medoGuid,
                                        Guid sourceGuid,
                                        Guid izdanieId,
                                        string publicationOrganName,
                                        string senderOrganName,
                                        string senderRegion,
                                        string eoNumber,
                                        string publicationPoint,
                                        string complexName,
                                        string documentNumber,
                                        DocumentType docType,
                                        NotificationType notType,
                                        string sendNotificationDateTime,
                                        string publicationDateTime,
                                        string documentSignDate,
                                        string mjDate = null,
                                        int mjNumber = 0,
                                        string actType = null)
        {
            try
            {
                SourceModel source = new SourceModel();
                source.Organization = publicationOrganName;
                source.SourceGuid = sourceGuid;

                HeaderModel header = new HeaderModel();
                header.Uid = izdanieId;
                header.Type = docType;
                header.Source = source;

                DateAndNumberModel documentSignBlock = new DateAndNumberModel(true);
                documentSignBlock.Date = documentSignDate;
                documentSignBlock.Number = documentNumber;
                if(mjDate != null && mjNumber != 0)
                {
                    documentSignBlock.MJDate = mjDate;
                    documentSignBlock.MJNumber = mjNumber;

                }

                DateAndNumberModel publicationDateAndNumber = new DateAndNumberModel();
                publicationDateAndNumber.Date = publicationDateTime;
                publicationDateAndNumber.Number = eoNumber;

                FoundationModel foundation = new FoundationModel();
                foundation.DateAndNumber = documentSignBlock;
                foundation.Organization = publicationOrganName;
                if (actType != null)
                    foundation.ActType = actType;

                CorrespondentModel correspondent = new CorrespondentModel();
                correspondent.Region = senderRegion;
                correspondent.Organization = senderOrganName;

                DocumentPublishedModel published = new DocumentPublishedModel();
                published.Foundation = foundation;
                published.Correspondent = correspondent;
                published.NotificationSendDateTime = sendNotificationDateTime;
                published.PublicationDateAndNumber = publicationDateAndNumber;
                published.PublicationPoint = publicationPoint;

                NotificationModel not = new NotificationModel();
                not.NotificationGuid = medoGuid;
                not.Type = notType;
                not.Published = published;
                not.Comment = complexName;
                if (mjDate != null && mjNumber != 0)
                {
                    not.Comment = not.Comment + $" \r\n(Зарегистрирован в Минюсте России {DateTime.Parse(mjDate).ToString("dd.MM.yyyy")} № {mjNumber})";

                }

                notification = new PublicationNotificationModel(header, not);
            }
            catch (Exception ex) { logger.Fatal(ex); }
        }
        public void SaveNotificationReport(string path = "")
        {
            try
            {
                if (notification != null)
                {
                    string sDefaultNamespace = "http://www.infpres.com/IEDMS";
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    ns.Add("xdms", sDefaultNamespace);

                    XmlSerializer serializer = new XmlSerializer(typeof(PublicationNotificationModel));
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "  ",
                        NewLineChars = "\r\n",
                        NewLineHandling = NewLineHandling.Replace,
                        Encoding = Encoding.GetEncoding(1251)
                    };
                    using (XmlWriter writer = XmlWriter.Create(System.IO.Path.Combine(path, "document.xml"), settings))
                    {
                        serializer.Serialize(writer, notification, ns);
                    }
                }
            }
            catch (Exception ex) { logger.Fatal(ex); }
        }
    }
}
