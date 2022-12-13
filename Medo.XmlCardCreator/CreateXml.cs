using medo2_5;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace XmlCardCreator
{
    public class CreateNotification : IDisposable
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private decimal Version = 2.5M;

        string Operator = "Автоматическое формирование";
        string Organization { get; set; }

        private Guid SourceGuid = new Guid("1953a86e-e35e-45cc-b031-556ca72c4080");
        private notificationTypeSimpleType notificationType = notificationTypeSimpleType.Опубликование;
        Guid notificationGuid { get; set; }
        DateTime PublishTime { get; set; }
        string Person { get; set; }
        string Number { get; set; }
        DateTime DocumentDate { get; set; }
        private string SendingOrgan = "Управление обеспечения правовой информатизации Службы специальной связи Федеральной службы охраны Российской Федерации (УОПИ Спецсвязи ФСО России)";
        private string Region = "Москва";
        long PublNumber { get; set; }
        DateTime PublDate { get; set; }
        private string PublicationPoint = "Официальный интернет-портал правовой информации (www.pravo.gov.ru)";
        string ComplexName { get; set; }
        string Path { get; set; }

        public CreateNotification(
            string Organization,
            Guid notificationGuid,
            string Person,
            string Number,
            DateTime DocumentDate,
            long PublNumber,
            DateTime PublDate,
            string ComplexName,
            string Path)
        {
            this.Organization = Organization;
            this.notificationGuid = notificationGuid;
            this.Person = Person;
            this.Number = Number;
            this.DocumentDate = DocumentDate;
            this.PublNumber = PublNumber;
            this.PublDate = PublDate;
            this.ComplexName = ComplexName;
            this.Path = Path;
            Create();
        }
        private void Create()
        {
            try
            {
                communication com = new communication();
                com.version = Version;

                communicationHeader header = new communicationHeader();
                header.created = DateTime.Now.ToString(Medo.Helpers.Formats.DateTimeFormat);
                header.type = "Документ";
                header.uid = Guid.NewGuid().ToString("D");
                header.@operator = Operator;

                communicationHeaderSource source = new communicationHeaderSource();
                source.organization = Organization;
                source.uid = SourceGuid.ToString("D");
                header.source = source;

                com.header = header;

                Notification not = new Notification();
                not.type = notificationType;
                not.uid = notificationGuid.ToString("D");

                NotificationDocumentPublished documentPublished = new NotificationDocumentPublished();
                documentPublished.time = PublishTime.ToString(Medo.Helpers.Formats.DateTimeFormat);

                foundationComplexType foundation = new foundationComplexType();

                GovAttributes organization = new GovAttributes();
                organization.Value = Organization;
                GovAttributes person = new GovAttributes();
                person.Value = Person;

                foundation.organization = organization;
                foundation.person = person;
                numberAndDateComplexType fnum = new numberAndDateComplexType();
                fnum.number = Number;
                fnum.date = DocumentDate.ToString(Medo.Helpers.Formats.DateFormat);
                foundation.num = fnum;
                documentPublished.foundation = foundation;

                correspondentSingleType correspondent = new correspondentSingleType();

                GovAttributes Sendorganization = new GovAttributes();
                Sendorganization.Value = SendingOrgan;
                GovAttributes region = new GovAttributes();
                region.Value = Region;

                correspondent.region = region;
                correspondent.organization = Sendorganization;
                documentPublished.correspondent = correspondent;

                numberAndDateComplexType num = new numberAndDateComplexType();
                num.number = PublNumber.ToString();
                num.date = PublDate.ToString(Medo.Helpers.Formats.DateTimeFormat);
                documentPublished.num = num;

                documentPublished.publicationPoint = PublicationPoint;
                not.comment = ComplexName;

                not.documentPublished = documentPublished;
                com.Item = not;

                using (Stream stream = File.Open(Path, FileMode.Create))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.Default,
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        var x = new XmlSerializer(typeof(communication));
                        XmlSerializerNamespaces name = new XmlSerializerNamespaces();
                        name.Add("xdms", "http://www.infpres.com/IEDMS");
                        x.Serialize(writer, com, name);
                    }
                }

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        public void Dispose()
        {

        }
    }


    public class CreateDocument : IDisposable
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        decimal Version = 2.5M;
        string Operator = "Автоматическое формирование XML";
        string MjNumber { get; set; }
        DateTime? MjDate { get; set; }

        string Message = "Документ";

        string Path;
        Guid HeaderGuid { get; set; }
        Guid SourceGuid { get; set; }

        Guid DocGuid { get; set; }
        List<string> ComplexOrgans { get; set; }
        string PdfFile { get; set; }
        string Organization { get; set; }
        string Person { get; set; }
        string Kind { get; set; }
        DateTime? SignDate { get; set; }
        string Number { get; set; }
        int Pages { get; set; }
        string Annotation { get; set; }
        public FileInfo fileInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(Path))
                    return new FileInfo(Path);
                else return null;
            }
        }


        public CreateDocument(
            string Path,
            Guid HeaderGuid,
            Guid SourceGuid,
            Guid DocGuid,
            string Organ,
            string Person,
            string Kind,
            DateTime? SignDate,
            string Number,
            int Pages,
            string Annotation,
            string pdfFile,
            string MjNumber = null,
            DateTime? MjDate = null,
            List<string> ComplexOrgans = null)
        {
            this.Path = Path;
            this.HeaderGuid = HeaderGuid;
            this.SourceGuid = SourceGuid;
            this.DocGuid = DocGuid;
            this.Organization = Organ;
            this.Person = Person;
            this.ComplexOrgans = ComplexOrgans;
            this.Kind = Kind;
            this.SignDate = SignDate;
            this.Number = Number;
            this.Pages = Pages;
            this.Annotation = Annotation;
            this.MjDate = MjDate;
            this.MjNumber = MjNumber;
            this.ComplexOrgans = ComplexOrgans;
            this.PdfFile = pdfFile;

            createDocument();
        }

        private void createDocument()
        {
            try
            {
                communication com = new communication();
                com.version = Version;

                communicationHeader header = new communicationHeader();
                header.created = DateTime.Now.ToString(Medo.Helpers.Formats.DateTimeFormat);
                header.type = Message;
                header.uid = HeaderGuid.ToString("D");
                header.@operator = Operator;

                communicationHeaderSource source = new communicationHeaderSource();
                source.organization = Organization;
                if (ComplexOrgans != null && ComplexOrgans.Count > 0)
                {
                    for (int i = 0; i < ComplexOrgans.Count; i++)
                    {
                        organsComplexTypeOrgans organs = new organsComplexTypeOrgans();
                        organs.organname = ComplexOrgans[i];
                        source.complexOrgans[i] = organs;
                    }
                }
                source.uid = SourceGuid.ToString("D");
                header.source = source;

                com.header = header;


                communicationDocument doc = new communicationDocument();
                doc.uid = DocGuid.ToString("D");
                //kind
                GovAttributes kind = new GovAttributes();
                kind.Value = Kind;
                doc.kind = kind;
                //Минюст если есть
                if (MjNumber != null && MjDate != null)
                {
                    MJnumberAndDateComplexType mj = new MJnumberAndDateComplexType();
                    mj.number = MjNumber.Trim();
                    mj.date = MjDate.Value.ToString(Medo.Helpers.Formats.MJDateFormat);
                    doc.mjregistration = mj;
                }
                
                //num
                numberAndDateComplexType num = new numberAndDateComplexType();
                if (SignDate.HasValue)
                {
                    num.date = SignDate.Value.ToString(Medo.Helpers.Formats.DateFormat);
                }
                if (!string.IsNullOrEmpty(Number))
                {
                    num.number = Number.Trim();  
                }
                doc.num = num;
                //signatory
                signerComplexType signers = new signerComplexType();
                signerComplexTypeSignatory signer = new signerComplexTypeSignatory();
                if (SignDate.HasValue)
                signer.signed = SignDate.Value.ToString(Medo.Helpers.Formats.DateFormat);
                GovAttributes g = new GovAttributes();
                if(Person != null)
                {
                    GovAttributes pers = new GovAttributes();
                    pers.Value = Person;
                    signer.person = pers;
                }               
                signers.signatory = signer;
                doc.signatories = signers;
                //pages
                doc.pages = ushort.Parse(Pages.ToString());
                //annotation
                doc.annotation = Annotation;
                com.Item = doc;
                List<filesComplexTypeFile> files = new List<filesComplexTypeFile>();
                files.Add(new filesComplexTypeFile() { localName = PdfFile });
                com.files = files.ToArray();

                using (Stream stream = File.Open(Path, FileMode.Create))
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

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        public void Dispose()
        {

        }
    }
}
