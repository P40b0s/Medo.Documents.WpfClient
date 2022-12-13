using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Medo.Client.SedRegistration
{
    /// <summary>
    /// Класс создания транспортных пакетов МЭДО о регистрации документа в СЕДе
    /// </summary>
    public class CreateSEDTransportPacket
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private string ReportsDirectory { get; set; }
        private string AutoReportsConnection { get; set; }
        public CreateSEDTransportPacket()
        {
            ReportsDirectory = Helpers.Paths.MedoReportPath;
            AutoReportsConnection = Helpers.ConnectionsStrings.ReserveServerAutoReportsConnectionString;
        }
       
        #region Создание уведомления о регистрации СЭД
        /// <summary>
        /// Создание пакета отказа в регистрации, пакет автоматически помещается в директорию отправки - out на сервере СЕДа
        /// </summary>
        /// <param name="sed">Модель регистрации</param>
        /// <returns></returns>
        public async Task<bool> CreateRefusionPacket(SedModel sed)
        {
            try
            {
                return await Task<bool>.Factory.StartNew(() =>
                {

                    string dir = string.Format("(Отказ в регистрации по документу {0})-{1}", sed.Number.Replace("\\", "-").Replace("/", "-"), sed.DocGuid.ToString("D"));

                    Guid HeaaderGuid = Guid.NewGuid();
                    DirectoryInfo di = Directory.CreateDirectory(Path.Combine(ReportsDirectory, dir));
                    XmlTextWriter Notification = new XmlTextWriter(Path.Combine(di.FullName, "document.xml"), Encoding.GetEncoding(1251));
                    string NS = "http://www.infpres.com/IEDMS";
                    string prefix = "xdms";
                    Notification.WriteStartDocument();
                    //xdms:communication           
                    Notification.WriteStartElement(prefix, "communication", NS);
                    Notification.WriteAttributeString("xmlns", prefix, null, NS);
                    Notification.WriteAttributeString(prefix + ":version", "2.0");
                    {//xdms:header
                        Notification.WriteStartElement(prefix, "header", NS);
                        Notification.WriteAttributeString(prefix + ":uid", HeaaderGuid.ToString("D"));
                        Notification.WriteAttributeString(prefix + ":type", "Уведомление");

                        {//xdms:source
                            Notification.WriteStartElement(prefix, "source", NS);
                            Notification.WriteAttributeString(prefix + ":uid", "1953a86e-e35e-45cc-b031-556ca72c4080");
                            {//xdms:organization 
                                Notification.WriteElementString(prefix, "organization", NS, "Управление обеспечения правовой информатизации Службы специальной связи Федеральной службы охраны Российской Федерации (УОПИ Спецсвязи ФСО России)");
                            }
                            Notification.WriteEndElement();
                        }
                        Notification.WriteEndElement();
                    }
                    { //xdms:notification
                        Notification.WriteStartElement(prefix, "notification", NS);
                        Notification.WriteAttributeString(prefix + ":type", "Отказано в регистрации");
                        Notification.WriteAttributeString(prefix + ":uid", sed.DocGuid.ToString("D"));
                        {//xdms:documentRefused
                            Notification.WriteStartElement(prefix, "documentRefused", NS);
                            {//xdms:time
                                Notification.WriteElementString(prefix, "time", NS, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                            }
                            {//xdms:foundation
                                Notification.WriteStartElement(prefix, "foundation", NS);
                                {//xdms:num
                                    Notification.WriteStartElement(prefix, "num", NS);
                                    {//xdms:number
                                        Notification.WriteElementString(prefix, "number", NS, sed.Number);
                                    }
                                    {//xdms:date
                                        Notification.WriteElementString(prefix, "date", NS, sed.SignDate.ToString("yyyy-MM-dd"));
                                    }
                                    Notification.WriteEndElement();
                                }
                                Notification.WriteEndElement();
                            }
                            {//xdms:reason

                                Notification.WriteElementString(prefix, "reason", NS, sed.RefuseStatus);
                            }
                            Notification.WriteEndElement();
                        }
                        if(!string.IsNullOrEmpty(sed.RefuseComment))
                        Notification.WriteElementString(prefix, "comment", NS, sed.RefuseComment);
                        Notification.WriteEndElement();
                    }
                    Notification.WriteEndElement();
                    Notification.Close();

                    List<string> iniFile = SEDIniConstructor(sed, dir);
                    if (iniFile.Count == 0)
                    {
                        throw new NotImplementedException("Ошибка при формировании Ini файла");
                    }
                    else
                    {
                        File.WriteAllLines(Path.Combine(di.FullName, "envelope.ini"), iniFile, Encoding.Default);
                        return true;
                    }
                });
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return false;
            }
        }

        /// <summary>
        /// Создание пакета регистрации, пакет автоматически помещается в директорию отправки - out на сервере СЕДа
        /// </summary>
        /// <param name="sed">Модель регистрации</param>
        /// <param name="eonumber">Номер электронного опубликования (используется только при регистрации документа совмесно с отправкой уведомлений об упубликовании)</param>
        /// <param name="eodate">Дата электронного опубликования (используется только при регистрации документа совмесно с отправкой уведомлений об упубликовании)</param>
        /// <returns></returns>
        public async Task<bool> CreateRegisterationPacket(SedModel sed, string eonumber = null, DateTime? eodate = null)
        {
            try
            {
                return await Task<bool>.Factory.StartNew(() =>
                {

                    string dir = string.Format("(Уведомление по документу {0})-{1}", sed.Number.Replace("\\", "-").Replace("/", "-"), sed.DocGuid.ToString("D"));

                    Guid HeaaderGuid = Guid.NewGuid();
                    DirectoryInfo di = Directory.CreateDirectory(Path.Combine(ReportsDirectory, dir));
                    XmlTextWriter Notification = new XmlTextWriter(Path.Combine(di.FullName, "document.xml"), Encoding.GetEncoding(1251));
                    string NS = "http://www.infpres.com/IEDMS";
                    string prefix = "xdms";
                    Notification.WriteStartDocument();
                    //xdms:communication           
                    Notification.WriteStartElement(prefix, "communication", NS);
                    Notification.WriteAttributeString("xmlns", prefix, null, NS);
                    Notification.WriteAttributeString(prefix + ":version", "2.0");
                    {//xdms:header
                        Notification.WriteStartElement(prefix, "header", NS);
                        Notification.WriteAttributeString(prefix + ":uid", HeaaderGuid.ToString("D"));
                        Notification.WriteAttributeString(prefix + ":type", "Уведомление");

                        {//xdms:source
                            Notification.WriteStartElement(prefix, "source", NS);
                            Notification.WriteAttributeString(prefix + ":uid", "1953a86e-e35e-45cc-b031-556ca72c4080");
                            {//xdms:organization 
                                Notification.WriteElementString(prefix, "organization", NS, "Управление обеспечения правовой информатизации Службы специальной связи Федеральной службы охраны Российской Федерации (УОПИ Спецсвязи ФСО России)");
                            }
                            Notification.WriteEndElement();
                        }
                        Notification.WriteEndElement();
                    }
                    { //xdms:notification
                        Notification.WriteStartElement(prefix, "notification", NS);
                        Notification.WriteAttributeString(prefix + ":type", "Зарегистрирован");
                        Notification.WriteAttributeString(prefix + ":uid", sed.DocGuid.ToString("D"));
                        {//xdms:documentPublished
                            Notification.WriteStartElement(prefix, "documentAccepted", NS);
                            //{//xdms:kind
                            //    Notification.WriteElementString(prefix, "kind", NS, report.VidDoc);
                            // }
                            {//xdms:time
                                Notification.WriteElementString(prefix, "time", NS, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                            }
                            {//xdms:foundation
                                Notification.WriteStartElement(prefix, "foundation", NS);
                                {//xdms:num
                                    Notification.WriteStartElement(prefix, "num", NS);
                                    {//xdms:number
                                        Notification.WriteElementString(prefix, "number", NS, sed.Number);
                                    }
                                    {//xdms:date
                                        Notification.WriteElementString(prefix, "date", NS, sed.SignDate.ToString("yyyy-MM-dd"));
                                    }
                                    Notification.WriteEndElement();
                                }
                                Notification.WriteEndElement();
                            }
                            {//xdms:num
                                Notification.WriteStartElement(prefix, "num", NS);
                                {//xdms:number
                                    Notification.WriteElementString(prefix, "number", NS, string.IsNullOrEmpty(eonumber) ? sed.Number : eonumber);
                                }
                                {//xdms:date
                                    Notification.WriteElementString(prefix, "date", NS, eodate.HasValue ? eodate.Value.ToString("yyyy-MM-dd") : sed.SignDate.ToString("yyyy-MM-dd"));
                                }
                                Notification.WriteEndElement();
                            }
                            Notification.WriteEndElement();
                        }
                        Notification.WriteEndElement();
                    }
                    Notification.WriteEndElement();
                    Notification.Close();

                    List<string> iniFile = SEDIniConstructor(sed, dir);
                    if (iniFile.Count == 0)
                    {
                        throw new NotImplementedException("Ошибка при формировании Ini файла");
                    }
                    else
                    {
                        File.WriteAllLines(Path.Combine(di.FullName, "envelope.ini"), iniFile, Encoding.Default);
                        return true;
                    }
                });
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return false;
            }
        }
        /// <summary>
        /// Конструктор INI файла уведомления СЭД
        /// </summary>
        /// <param name="sed">Модель регистрации</param>
        /// <param name="dirname">Директория с данным транспортным пакетом</param>
        /// <returns></returns>
        private List<string> SEDIniConstructor(SedModel sed, string dirname)
        {
            List<string> IniFile = new List<string>();
            try
            {
                string adress = GetOrganAdress(sed.SGuid);
                if (string.IsNullOrEmpty(adress))
                {
                    throw new NotImplementedException(string.Format("Адрес данного дапартамента не найден, SourceGuid = {0}", sed.SGuid.ToString("B")));
                }
                IniFile.Add("[ПИСЬМО КП ПС СЗИ]");
                IniFile.Add("ТЕМА=" + dirname);
                IniFile.Add("ШИФРОВАНИЕ=0");
                IniFile.Add("ЭЦП=1");
                IniFile.Add("ДОСТАВЛЕНО=1");
                IniFile.Add("ПРОЧТЕНО=1");
                IniFile.Add("[АДРЕСАТЫ]");
                IniFile.Add("0" + "=" + adress);
                IniFile.Add("[ФАЙЛЫ]");
                IniFile.Add("0=document.xml");
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return new List<string>();
            }
            return IniFile;
        }
        /// <summary>
        /// Получение адреса МЭДО
        /// </summary>
        /// <param name="source">Необходим SourceGuid органа для определения его адреса</param>
        /// <returns></returns>
        private string GetOrganAdress(Guid source)
        {
            string adress = null;
            SqlConnection conn = new SqlConnection(AutoReportsConnection);
            try
            {
                conn.Open();
            }
            catch (SqlException se)
            {
                logger.Fatal(se, "Ошибка подключения");
            }

            string command = (@"Select Adress FROM MedoAdress WHERE SourceGuid = @SourceGuid");
            SqlCommand cmd = new SqlCommand(command);
            cmd.Parameters.Add("@SourceGuid", SqlDbType.UniqueIdentifier).Value = source;
            cmd.Connection = conn;
            try
            {
                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {

                    while (dr.Read())
                    {
                        if (dr.HasRows)
                        {
                            adress = dr.IsDBNull(0) ? null : dr.GetString(0).Trim();
                        }
                    }
                }
                return adress;
            }

            catch (SqlException sq)
            {
                logger.Fatal(sq);
                return null;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return null;
            }

        }
        #endregion
    }
}
