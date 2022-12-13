using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using XmlCardCreator.Models;
using XmlCardCreator.Models.SystemModels;
using XmlCardCreator.Models.SystemModels.RequsitsModels;

namespace XmlCardCreator
{
    public class CreateSystemaPublicationReport
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private SystemaPublicationReportModel sys { get; set; }
        public CreateSystemaPublicationReport(Guid medoGuid,
                                               Guid sourceGuid,
                                               string organName,
                                               string type,
                                               string number,
                                               DateTime signDate,
                                               Guid izdanieId,
                                               string eoNumber,
                                               DateTime publicationDate,
                                               string complexName,
                                               int mjNumber = 0,
                                               DateTime? mjDate = null)
        {
            try
            {
                DocumentRequisitsModel req = new DocumentRequisitsModel();
                req.MedoGuid = Guid.NewGuid();
                req.SourceGuid = Guid.NewGuid();
                req.OrganName = organName;
                req.Type = type;
                req.Number = number;
                req.SignDate = signDate;

                MJRequisitsModel mj = null;
                if (mjDate.HasValue && mjNumber != 0)
                {
                    mj = new MJRequisitsModel();
                    mj.RegistrationNumber = mjNumber;
                    mj.RegistrationDate = mjDate;
                }

                PublicationRequsitsModel publication = new PublicationRequsitsModel();
                publication.IzdanieId = izdanieId;
                publication.EONumber = eoNumber;
                publication.PublicationDate = publicationDate;
                publication.ComplexName = complexName;
                if (mjDate.HasValue && mjNumber != 0)
                {
                    publication.ComplexName = $"{publication.ComplexName} \r\n (Зарегистрирован {mjDate.Value.ToString("dd.MM.yyyy")} № {mjNumber})";
                }
                sys = new SystemaPublicationReportModel(req, mj, publication);
            }
            catch (Exception ex) { logger.Fatal(ex); }
        }
        public void SaveSystemaPublicationReport(string path = "")
        {
            try
            {
                if (sys != null)
                {
                    string sDefaultNamespace = "http://publication.pravo.gov.ru";
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    ns.Add("", sDefaultNamespace);

                    XmlSerializer serializer = new XmlSerializer(typeof(SystemaPublicationReportModel));
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "  ",
                        NewLineChars = "\r\n",
                        NewLineHandling = NewLineHandling.Replace,
                        Encoding = Encoding.UTF8,
                        NewLineOnAttributes = true

                    };
                    using (XmlWriter writer = XmlWriter.Create(System.IO.Path.Combine(path, "publication.xml"), settings))
                    {
                        serializer.Serialize(writer, sys, ns);
                    }
                }
            }
            catch (Exception ex) { logger.Fatal(ex); }
        }
    }
}
