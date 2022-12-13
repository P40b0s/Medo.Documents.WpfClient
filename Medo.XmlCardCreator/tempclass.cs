using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Schema;
using medo2_5;
using System.Xml.Serialization;
using System.Xml;

namespace XmlCardCreator
{



    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public class createone
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();


        private void create(object sender, RoutedEventArgs e)
        {
            createdocument();
            createnotification();
        }


        private void createdocument()
        {
            communication com = new communication();
            com.version = 2.5M;

            communicationHeader header = new communicationHeader();
            header.created = MyDateTime.DateTime(DateTime.Now, MyDateTime.DataFormat.XmlDateTime);
            header.type = "Документ";
            header.uid = Guid.NewGuid().ToString("D");
            header.@operator = "Система атоматического создания XML";

            communicationHeaderSource source = new communicationHeaderSource();
            source.organization = "Принявший орган";
            source.uid = Guid.NewGuid().ToString("D");
            header.source = source;

            com.header = header;


            communicationDocument doc = new communicationDocument();
            //kind
            GovAttributes kind = new GovAttributes();
            kind.Value = "Вид документа";
            doc.kind = kind;
            //num
            numberAndDateComplexType num = new numberAndDateComplexType();
            num.date = MyDateTime.DateTime(DateTime.Now.AddYears(-1), MyDateTime.DataFormat.XmlDate);
            num.number = "999999";
            doc.num = num;
            //signatory
            signerComplexType signers = new signerComplexType();
            signerComplexTypeSignatory signer = new signerComplexTypeSignatory();
            signer.signed = MyDateTime.DateTime(DateTime.Now.AddYears(-1), MyDateTime.DataFormat.XmlDate);
            signers.signatory = signer;
            doc.signatories = signers;
            //pages
            doc.pages = 999;
            //annotation
            doc.annotation = "IJOISJFOIWJFOIWEJFPWEFJPWEOFJKpokwefpokwepofkwpeofkpOKEFPOWEKF щлцщзулащзцулащзo koekfpowekf woekfpwoekfp kpeofkp okwpoefkp okwpeofkp okwpeofk pokwpoefkpowe";

            com.Item = doc;

            using (Stream stream = File.Open("new2.xml", FileMode.Create))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.Default,
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    var x = new System.Xml.Serialization.XmlSerializer(typeof(communication));
                    XmlSerializerNamespaces name = new XmlSerializerNamespaces();
                    name.Add("xdms", "http://www.infpres.com/IEDMS");
                    x.Serialize(writer, com, name);
                }



            }

            //    XmlSerializer serializer = new XmlSerializer(typeof(communication));
            //XmlSerializerNamespaces name = new XmlSerializerNamespaces();
            //name.Add("xdms", "http://www.infpres.com/IEDMS");
            //serializer.Serialize(stream, com,name, "Windows-1251");
            //stream.Flush();
            //}

        }

        private void createnotification()
        {
            communication com = new communication();
            com.version = 2.5M;

            communicationHeader header = new communicationHeader();
            header.created = MyDateTime.DateTime(DateTime.Now, MyDateTime.DataFormat.XmlDateTime);
            header.type = "Уведомление";
            header.uid = Guid.NewGuid().ToString("D");
            header.@operator = "УОПИ";

            communicationHeaderSource source = new communicationHeaderSource();
            source.organization = "УОПИ";
            source.uid = Guid.NewGuid().ToString("D");
            header.source = source;

            com.header = header;


            Notification not = new Notification();
            not.type = notificationTypeSimpleType.Опубликование;
            not.uid = Guid.NewGuid().ToString("D");

            NotificationDocumentPublished documentPublished = new NotificationDocumentPublished();
            documentPublished.time = MyDateTime.DateTime(DateTime.Now.AddDays(-50), MyDateTime.DataFormat.XmlDateTime);

            foundationComplexType foundation = new foundationComplexType();

            GovAttributes organization = new GovAttributes();
            organization.Value = "Правительство Российской Федерации";
            GovAttributes person = new GovAttributes();
            person.Value = "Медведев Д.А.";

            foundation.organization = organization;
            foundation.person = person;
            numberAndDateComplexType fnum = new numberAndDateComplexType();
            fnum.number = "6367463";
            fnum.date = MyDateTime.DateTime(DateTime.Now.AddYears(-1), MyDateTime.DataFormat.XmlDate);
            foundation.num = fnum;
            documentPublished.foundation = foundation;

            correspondentSingleType correspondent = new correspondentSingleType();

            GovAttributes organization2 = new GovAttributes();
            organization2.Value = "УОПИ";
            GovAttributes region = new GovAttributes();
            region.Value = "Москва";

            correspondent.region = region;
            correspondent.organization = organization2;
            documentPublished.correspondent = correspondent;

            numberAndDateComplexType num = new numberAndDateComplexType();
            num.number = "90829839283402";
            num.date = MyDateTime.DateTime(DateTime.Now.AddYears(-1), MyDateTime.DataFormat.XmlDateTime);
            documentPublished.num = num;

            documentPublished.publicationPoint = "Портал Право гов ру";
            not.comment = "Бла бла бла бла бла бла бла бл аб лаллалвлалвла";
            not.documentPublished = documentPublished;
            com.Item = not;

            using (Stream stream = File.Open("new.xml", FileMode.Create))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.Default,
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    var x = new System.Xml.Serialization.XmlSerializer(typeof(communication));
                    XmlSerializerNamespaces name = new XmlSerializerNamespaces();
                    name.Add("xdms", "http://www.infpres.com/IEDMS");
                    x.Serialize(writer, com, name);
                }



            }

            //    XmlSerializer serializer = new XmlSerializer(typeof(communication));
            //XmlSerializerNamespaces name = new XmlSerializerNamespaces();
            //name.Add("xdms", "http://www.infpres.com/IEDMS");
            //serializer.Serialize(stream, com,name, "Windows-1251");
            //stream.Flush();
            //}
        }
        private void loaddocument()
        {
            communication com = null;
            communicationDocument doc = null;
            Notification not = null;
            XmlSerializer serializer = new XmlSerializer(typeof(communication));
            using (StreamReader sr = new StreamReader("document.xml", Encoding.Default))
            {
                com = (communication)serializer.Deserialize(sr);
            }
            try
            {
                not = (Notification)com.Item;
            }
            catch (System.InvalidCastException ex)
            {
                doc = (communicationDocument)com.Item;
                logger.Fatal(ex);
            }
        }


        private void validate(object sender, RoutedEventArgs e)
        {

            validatebasedir();


        }

        private void validateall()
        {
            try
            {
                List<DirectoryInfo> dirs = new DirectoryInfo(System.IO.Path.Combine(Medo.Helpers.Paths.PakMedoFolder)).GetDirectories()/*.Where(c=>c.CreationTime >= DateTime.Now.AddYears(-1))*/.ToList();
                List<FileInfo> file = new List<FileInfo>();
                foreach (DirectoryInfo d in dirs)
                {
                    FileInfo f = d.GetFiles("*.xml").FirstOrDefault();
                    if (f != null)
                    {
                        file.Add(f);
                    }

                }
                XmlSchemaSet schema = new XmlSchemaSet();
                try
                {
                    schema.Add("http://www.infpres.com/IEDMS", "Medo2.5xsd\\Card.xsd");
                    //schema.Add("http://www.infpres.com/IEDMS", "Medo2.5xsd\\Medo2.5.xsd");
                    //schema.Add("http://minsvyaz.ru/container", "Medo2.7.xsd");                   
                }
                catch (System.Exception ex)
                {

                }


                bool errors = false;
                foreach (FileInfo f in file)
                {
                    XDocument doc = new XDocument(XElement.Load(f.FullName));
                    // XDocument doc = new XDocument(XElement.Load("document.xml"));

                    doc.Validate(schema, (o, error) =>
                    {
                        errors = true;
                        logger.Info(string.Format("Ошиьбка {0} в документе {1}", error.Message, f.FullName));
                    }
                          );

                }

                if (errors)
                {
                    //valbutton.Background = Brushes.Red;
                    logger.Info(string.Format("Валидация с ошибками"));
                }
                else
                {
                    logger.Info(string.Format("Валидация успешна!"));
                    //valbutton.Background = Brushes.Green;
                }
                //foreach(string s in errorsList)
                //{
                //    txt.Text += s + Environment.NewLine;
                //}

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        private void validatebasedir()
        {
            try
            {
                XmlSchemaSet schema = new XmlSchemaSet();
                try
                {
                    schema.Add("http://www.infpres.com/IEDMS", "Medo2.5xsd\\Medo2.5.xsd");
                    //schema.Add("http://www.infpres.com/IEDMS", "Medo2.5xsd\\Medo2.5.xsd");
                    //schema.Add("http://minsvyaz.ru/container", "Medo2.7.xsd");                   
                }
                catch (System.Exception ex)
                {

                }


                bool errors = false;

                XDocument doc = new XDocument(XElement.Load("document.xml"));
                // XDocument doc = new XDocument(XElement.Load("document.xml"));

                doc.Validate(schema, (o, error) =>
                {
                    errors = true;
                    logger.Info(string.Format("Ошибка {0}", error.Message));

                }
                      );



                if (errors)
                {
                    //valbutton.Background = Brushes.Red;
                    logger.Info(string.Format("Валидация с ошибками"));
                }
                else
                {
                    logger.Info(string.Format("Валидация успешна!"));
                    //valbutton.Background = Brushes.Green;
                }
                //foreach(string s in errorsList)
                //{
                //    txt.Text += s + Environment.NewLine;
                //}

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }




        private XElement WriteDataToXml(DateTime? SignDate, string acttype, FileInfo XmlFile, string num, string text, string organ, DateTime? MJDate, string MJNumber)
        {
            try
            {
                XElement elem = XElement.Load(XmlFile.FullName);
                //Подключаем namespace xdms
                XNamespace xdms = "http://www.infpres.com/IEDMS";
                elem.Element(xdms + "header").Element(xdms + "source").Element(xdms + "organization").Value = organ;
                elem.Element(xdms + "document").Element(xdms + "kind").Value = acttype;
                elem.Element(xdms + "document").Element(xdms + "annotation").Value = string.IsNullOrEmpty(text) ? "" : text;
                elem.Element(xdms + "document").Element(xdms + "num").Element(xdms + "number").Value = string.IsNullOrEmpty(num) ? "" : num; ;
                elem.Element(xdms + "document").Element(xdms + "num").Element(xdms + "date").Value = SignDate.Value.ToString("yyyy-MM-dd");
                elem.Element(xdms + "document").Element(xdms + "signatories").Element(xdms + "signatory").Element(xdms + "signed").Value = SignDate.Value.ToString("yyyy-MM-dd");
                if (elem.Element(xdms + "document").Element(xdms + "mjregistration") == null && MJDate.HasValue && MJDate != DateTime.MinValue)
                {
                    elem.Element(xdms + "document").AddFirst(new XElement(xdms + "mjregistration",
                                                          new XElement(xdms + "number", MJNumber),
                                                          new XElement(xdms + "date", MJDate.Value.ToString(Medo.Helpers.Formats.MJDateFormat))));
                }

                return elem;
            }
            catch (System.Exception ex)
            {
                return new XElement("");
            }
        }







    }
}
