using Ionic.Zip;
using Medo.Core.EventsAggregator;
using Medo.Core.Models.ReportsSenderModel;
using NLog;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Medo.Modules.ReportsListModule
{
    /// <summary>
    /// Класс для формирования локальных отчетов (отправка по сисеме СЗИ)
    /// </summary>
    public class LocalReports : Collections
    {
        #region Properties 
        IEventAggregator aggregator;
        public double SelectedReportsCount
        {
            get
            {
                return this._SelectedReportsCount;
            }
            set
            {
                if (this.SelectedReportsCount != value)
                {
                    this._SelectedReportsCount = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _SelectedReportsCount { get; set; }
        public double SendedReportsCount
        {
            get
            {
                return this._SendedReportsCount;
            }
            set
            {
                if (this.SendedReportsCount != value)
                {
                    this._SendedReportsCount = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _SendedReportsCount { get; set; }

        public DateTime SelectedDate
        {
            get
            {
                return this._SelectedDate;
            }
            set
            {
                if (this.SelectedDate != value)
                {
                    this._SelectedDate = value;
                    OnPropertyChanged();
                    aggregator.GetEvent<DateSelectedEvent>().Publish(value);
                }
            }
        }
        private DateTime _SelectedDate { get; set; }
        #endregion

        public LocalReports(IEventAggregator _aggregator) : base(_aggregator)
        {
            aggregator = _aggregator;
            SelectedDate = DateTime.Now;
        }
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        #region Блок создания и копирования отчетов на флешку (zip для СЗИ)
        public void SendSZIReport()
        {
            try
            {
                if (ReportsCollection.Any(i => i.IsSelected))
                {
                    int count = 0;
                    SelectedReportsCount = ReportsCollection.Where(r => r.IsSelected).Count();
                    DirectoryInfo reportDir = Directory.CreateDirectory(Path.Combine(Medo.Helpers.Paths.MedoReportPath, "Отчеты УОПИ за " + SelectedDate.ToShortDateString()));
                    IEnumerable<ReportModel> reports = ReportsCollection.Where(n => n.IsSelected);
                    foreach (ReportModel report in reports)
                    {
                      
                            GetOneXmlFileReport(reportDir.FullName, report, "publication");
                            //GetManyXmlReportFiles(Medo.Helpers.Paths.GpuFolder, report, true);

                            //switch (report.SourceGuid.ToString("B").ToUpper())
                            //{
                            //    case Medo.Helpers.SourceGuidOrgansNames.КС:
                            //        {

                            //            GetManyXmlReportFiles(Medo.Helpers.Paths.KSFolder, report, false);
                            //            break;
                            //        }
                            //}
                            //switch (report.SourceGuid.ToString("B").ToUpper())
                            //{
                            //    case Medo.Helpers.SourceGuidOrgansNames.МИД:
                            //        {
                            //            GetOneXmlFileReport(Medo.Helpers.Paths.MidFolder, report, "МИД");
                            //            break;
                            //        }
                            //}
                            //switch (report.SourceGuid.ToString("B").ToUpper())
                            //{
                            //    case Medo.Helpers.SourceGuidOrgansNames.Правительство:
                            //        {

                            //            GetOneXmlFileReport(Medo.Helpers.Paths.GovFolder, report, "publication");
                            //            break;
                            //        }
                            //}
                            count++;
                            report.IsSelected = false;
                            SendedReportsCount = count;
                    }
                    //ZipAndCopyReportsToOutDir(true, Medo.Helpers.Paths.GpuFolder, "ГПУ");
                    ZipAndCopyReportsToOutDir(true, reportDir.FullName, "ДУМА");
                    //ZipAndCopyReportsToOutDir(true, Medo.Helpers.Paths.NtcFolder, "НТЦ");
                    //ZipAndCopyReportsToOutDir(true, Medo.Helpers.Paths.KSFolder, "КС");
                    //ZipAndCopyReportsToOutDir(false, Medo.Helpers.Paths.MidFolder, "МИД_");
                    //ZipAndCopyReportsToOutDir(false, Medo.Helpers.Paths.MinyustFolder, "Минюст");
                    //ZipAndCopyReportsToOutDir(false, Medo.Helpers.Paths.GovFolder, "publication");
                    logger.Info(string.Format("Отчеты СЗИ из {0} документов успешно сформированы", count));
                }
                else
                    throw new NotImplementedException("Документы для отправки не выбраны!");

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private void ZipAndCopyReportsToOutDir(bool zipping, string directory, string whois)
        {
            string path = Path.Combine(directory, whois + "_" + SelectedDate.ToString("dd.MM.yyyy_") + DateTime.Now.ToString("HH-mm-ss") + ".zip");
            if (zipping == true && Directory.GetFiles(directory).Count() > 0)
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddFiles(Directory.GetFiles(directory), SelectedDate.ToString("yyyy-MM-dd") + "-DayXML");
                    zip.Save(path);
                }
            }
            else if (Directory.GetFiles(directory).Count() > 0)
            {
                List<FileInfo> fi = new DirectoryInfo(directory).GetFiles().ToList();
                fi.ForEach(f => f.CopyTo(Path.Combine(Medo.Helpers.Paths.FlashReportPath, whois + SelectedDate.ToString("yyyy-MM-dd") + f.Extension)));
            }
            ClearTempDir(directory);
            File.WriteAllLines(Path.Combine(directory, "reports.ini"), IniConstructor(new FileInfo(path)), Encoding.Default);
        }

        private void ClearTempDir(string directory)
        {
            List<FileInfo> fi = new DirectoryInfo(directory).GetFiles("*.xml").ToList();
            fi.ForEach(d => d.Delete());
        }

        #region Создание XML файлов
        private void GetOneXmlFileReport(string dirname, ReportModel items, string org)
        {
            if (!File.Exists(Path.Combine(dirname, org + SelectedDate.ToString("yyyy-MM-dd") + ".xml")))
            {
                XmlTextWriter OneXmltw = new XmlTextWriter(Path.Combine(dirname, org + SelectedDate.ToString("yyyy-MM-dd") + ".xml"), Encoding.GetEncoding(1251));
                OneXmltw.WriteStartDocument();
                OneXmltw.WriteStartElement("Document");
                OneXmltw.WriteEndElement();
                OneXmltw.Close();
            }


            System.Xml.XmlDocument OneXmldoc = new System.Xml.XmlDocument();
            OneXmldoc.Load(Path.Combine(dirname, org + SelectedDate.ToString("yyyy-MM-dd") + ".xml"));
            XmlNode publication = OneXmldoc.CreateElement("publication");
            OneXmldoc.DocumentElement.AppendChild(publication);
            XmlNode code = OneXmldoc.CreateElement("code");
            code.InnerText = items.EoNumber;
            publication.AppendChild(code);
            XmlNode content = OneXmldoc.CreateElement("content");
            content.InnerText = items.ComplexName;
            publication.AppendChild(content);
            XmlNode Operator = OneXmldoc.CreateElement("operator");
            Operator.InnerText = "УОПИ";
            publication.AppendChild(Operator);
            XmlNode pages = OneXmldoc.CreateElement("pages");
            pages.InnerText = "0";
            publication.AppendChild(pages);
            XmlNode publ_date_portal = OneXmldoc.CreateElement("publ_date_portal");
            publ_date_portal.InnerText = items.PublicationDate.ToString("dd.MM.yyyy HH:mm:ss");
            publication.AppendChild(publ_date_portal);
            XmlNode upload_date = OneXmldoc.CreateElement("upload_date");
            upload_date.InnerText = items.PublicationDate.ToString("dd.MM.yyyy HH:mm:ss");
            publication.AppendChild(upload_date);
            XmlNode regdate = OneXmldoc.CreateElement("regdate");
            regdate.InnerText = items.SignDate.ToString("dd.MM.yyyy HH:mm:ss");
            publication.AppendChild(regdate);
            XmlNode regnumber = OneXmldoc.CreateElement("regnumber");
            regnumber.InnerText = items.Number;
            publication.AppendChild(regnumber);
            XmlNode organ = OneXmldoc.CreateElement("organ");
            organ.InnerText = items.OrganName;
            publication.AppendChild(organ);
            XmlNode status = OneXmldoc.CreateElement("status");
            status.InnerText = "Открытый";
            publication.AppendChild(status);
            XmlNode viddoc = OneXmldoc.CreateElement("viddoc");
            viddoc.InnerText = items.ActType;
            publication.AppendChild(viddoc);
            XmlNode publ_number = OneXmldoc.CreateElement("publ_number");
            publ_number.InnerText = items.EoNumber;
            publication.AppendChild(publ_number);
            OneXmldoc.Save(Path.Combine(dirname, org + SelectedDate.ToString("yyyy-MM-dd") + ".xml"));
        }

        private void GetManyXmlReportFiles(string dirname, ReportModel items, bool CopyPdf)
        {

            XmlTextWriter ManyXmltw = new XmlTextWriter(Path.Combine(dirname, items.EoNumber + ".xml"), Encoding.GetEncoding(1251));
            ManyXmltw.WriteStartDocument();
            ManyXmltw.WriteStartElement("Document");
            ManyXmltw.WriteEndElement();
            ManyXmltw.Close();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(Path.Combine(dirname, items.EoNumber + ".xml"));
            XmlNode code = doc.CreateElement("code");
            code.InnerText = items.EoNumber;
            doc.DocumentElement.AppendChild(code);
            XmlNode Database = doc.CreateElement("Database");
            Database.InnerText = "cd00000";
            doc.DocumentElement.AppendChild(Database);
            XmlNode content = doc.CreateElement("content");
            content.InnerText = items.ComplexName;
            doc.DocumentElement.AppendChild(content);
            XmlNode Operator = doc.CreateElement("operator");
            Operator.InnerText = "УОПИ";
            doc.DocumentElement.AppendChild(Operator);
            XmlNode pages = doc.CreateElement("pages");
            pages.InnerText = "0";
            doc.DocumentElement.AppendChild(pages);
            XmlNode publ_date_portal = doc.CreateElement("publ_date_portal");
            publ_date_portal.InnerText = items.PublicationDate.ToString("dd.MM.yyyy HH:mm:ss");
            doc.DocumentElement.AppendChild(publ_date_portal);
            XmlNode upload_date = doc.CreateElement("upload_date");
            upload_date.InnerText = items.PublicationDate.ToString("dd.MM.yyyy HH:mm:ss");
            doc.DocumentElement.AppendChild(upload_date);
            XmlNode regdate = doc.CreateElement("regdate");
            regdate.InnerText = items.SignDate.ToString("dd.MM.yyyy HH:mm:ss");
            doc.DocumentElement.AppendChild(regdate);
            XmlNode regnumber = doc.CreateElement("regnumber");
            regnumber.InnerText = items.Number;
            doc.DocumentElement.AppendChild(regnumber);
            XmlNode organ = doc.CreateElement("organ");
            organ.InnerText = items.OrganName;
            doc.DocumentElement.AppendChild(organ);
            XmlNode status = doc.CreateElement("status");
            status.InnerText = "Открытый";
            doc.DocumentElement.AppendChild(status);
            XmlNode viddoc = doc.CreateElement("viddoc");
            viddoc.InnerText = items.ActType;
            doc.DocumentElement.AppendChild(viddoc);
            XmlNode publ_number = doc.CreateElement("publ_number");
            publ_number.InnerText = items.EoNumber;
            doc.DocumentElement.AppendChild(publ_number);
            XmlNode theme = doc.CreateElement("theme");
            doc.DocumentElement.AppendChild(theme);
            doc.Save(Path.Combine(dirname, items.EoNumber + ".xml"));
            if ((items.SourceGuid.ToString("B").ToUpper() == Medo.Helpers.SourceGuidOrgansNames.КС) && CopyPdf == true)
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(Medo.Helpers.Paths.PakMedoFolder, items.Directory));
                FileInfo pdf = di.GetFiles("*.pdf").FirstOrDefault();
                pdf.CopyTo(Path.Combine(dirname, items.EoNumber + ".pdf"));
            }
        }
        #endregion
        private List<string> IniConstructor(FileInfo file)
        {
            try
            {
                    List<string> IniFile = new List<string>();
                    IniFile.Add("[ПИСЬМО КП ПС СЗИ]");
                    IniFile.Add(string.Format("ТЕМА=ЭСД МЭДО (Отчеты об опубликовании) {0}", Guid.NewGuid()));
                    IniFile.Add("ШИФРОВАНИЕ=0");
                    IniFile.Add("ЭЦП=1");
                    IniFile.Add("ДОСТАВЛЕНО=1");
                    IniFile.Add("ПРОЧТЕНО=1");
                    IniFile.Add("[АДРЕСАТЫ]");
                    IniFile.Add("0=GOSD_S~MEDOGU");
                    IniFile.Add("[ФАЙЛЫ]");
                    IniFile.Add("0="+ file.Name);
                    return IniFile;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return new List<string>();
            }
        }
        #endregion
    }
}
