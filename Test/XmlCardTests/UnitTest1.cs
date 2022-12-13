using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using XmlCardCreator;

namespace XmlCardCreator.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCreateSystemaPublicationReportMethod()
        {

            CreateSystemaPublicationReport sys = new CreateSystemaPublicationReport(Guid.NewGuid(),
                                                                                    Guid.NewGuid(),
                                                                                    "Президент Российской Федерации",
                                                                                    "Федеральный закон",
                                                                                    "316-ФЗ",
                                                                                    new DateTime(2017, 11, 15),
                                                                                    Guid.NewGuid(),
                                                                                    "0001201710150001",
                                                                                    new DateTime(2017, 10, 15),
                                                                                    "Федеральный закон от 15.11.2017 № 316-ФЗ \r\n О внесении изменений в Федеральный закон О федеральном бюджете на 2017 год и на плановый период 2018 и 2019 годов",
                                                                                    33456,
                                                                                    new DateTime(2017,11,1)
                                                                                    );
            sys.SaveSystemaPublicationReport();
        }

        [TestMethod]
        public void TestCreateNotificationReportMethod()
        {
            CreateNotificationReport report = new CreateNotificationReport(Guid.NewGuid(),
                                                                           Guid.NewGuid(),
                                                                           Guid.NewGuid(),
                                                                           "Министерство строительства и жилищно-коммунального хозяйства Российской Федерации",
                                                                           "Управление обеспечения правовой информатизации Службы специальной связи Федеральной службы охраны Российской Федерации (УОПИ Спецсвязи ФСО России)",
                                                                           "Москва",
                                                                           "0001201711080034",
                                                                           "Официальный интернет-портал правовой информации (www.pravo.gov.ru)",
                                                                           "Приказ Министерства строительства и жилищно-коммунального хозяйства Российской Федерации от 16.10.2017 № 1431/пр \r\n О внесении изменений в перечень должностей в организациях, созданных для выполнения задач, поставленных перед Министерством строительства",
                                                                           "1431/пр",
                                                                            Enums.DocumentType.Уведомление,
                                                                            Enums.NotificationType.Опубликование,
                                                                            DateTime.Now.ToString(),
                                                                            new DateTime(2017, 11, 08, 15, 39, 48).ToString(),
                                                                            new DateTime(2017, 10, 16, 15, 39, 48).ToString());
            report.SaveNotificationReport();
        }

       

        [TestMethod]
        public void TestCreateNotificationReportWithMinJustMethod()
        {
            CreateNotificationReport report = new CreateNotificationReport(Guid.NewGuid(),
                                                                           Guid.NewGuid(),
                                                                           Guid.NewGuid(),
                                                                           "Министерство строительства и жилищно-коммунального хозяйства Российской Федерации",
                                                                           "Управление обеспечения правовой информатизации Службы специальной связи Федеральной службы охраны Российской Федерации (УОПИ Спецсвязи ФСО России)",
                                                                           "Москва",
                                                                           "0001201711080034",
                                                                           "Официальный интернет-портал правовой информации (www.pravo.gov.ru)",
                                                                           "Приказ Министерства строительства и жилищно-коммунального хозяйства Российской Федерации от 16.10.2017 № 1431/пр \r\n О внесении изменений в перечень должностей в организациях, созданных для выполнения задач, поставленных перед Министерством строительства",
                                                                           "1431/пр",
                                                                            Enums.DocumentType.Уведомление,
                                                                            Enums.NotificationType.Опубликование,
                                                                            DateTime.Now.ToString(),
                                                                            new DateTime(2017, 11, 08, 15, 39, 48).ToString(),
                                                                            new DateTime(2017, 10, 16, 15, 39, 48).ToString(),
                                                                            DateTime.Now.ToString(),
                                                                            33543,
                                                                            "Приказ"
                                                                           );
            report.SaveNotificationReport();
        }
    }
}
