using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Izdanie.WebCabinet.Medo.DTO;
using System.Collections;
using System.IO;

namespace MedoIntegrationWcfTest
{
    class Program
    {
        private const string medodir = "D:\\Medo\\";
        private const string login = "nikitin";
        private const string password = "ὕͼ꟥䦒菮첺뇔";
        private const string logs = "\\\\182.5.202.220\\logs";
        private const string firstSearchString = "Process document:";
        private const string pngDir = "\\Png\\";
        static void Main(string[] args)
        {
            try
            {
                Uri tcpUri = new Uri("http://182.5.202.220/wcf/MedoIntegrationService.svc");
                EndpointAddress address = new EndpointAddress(tcpUri);
                WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);
                binding.MaxReceivedMessageSize = 2147483647;
                binding.ReceiveTimeout = new System.TimeSpan(8, 0, 0);
                binding.SendTimeout = new System.TimeSpan(8, 0, 0);
                string search = firstSearchString + medodir + "ЭСД МЭДО (231 от 28.02.2017 {ED0D0FC2-33DC-48E1-8EE6-EACA4B9AA90E}) 2V7W0A2O";
                string t = docGuid(search);
                int png = pngsCount(t);
                //MedoIntegrationServiceClient client = new MedoIntegrationServiceClient(binding, address);
                //var tt = client.State;
                //client.ProcessDocument(System.IO.Path.Combine(medodir, "ЭСД МЭДО (231 от 28.02.2017 {ED0D0FC2-33DC-48E1-8EE6-EACA4B9AA90E}) 2V7W0A2O"), login, password);
                //var ttt = client.GetFinishedDocuments();

            }
            catch (System.Exception ex)
            {
                var message = ex;
            }

        }

        public static int pngsCount(string docGuid)
        {
            List<string> logList = new List<string>();
            try
            {
                string logFile = string.Format("log.{0}.log", DateTime.Now.ToString("yyyyMMdd"));
                string logFilePath = Path.Combine(logs, logFile);
                using (var fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        logList.Add(sr.ReadLine());
                    }
                }
                int png = logList.Where(s => s.Contains(docGuid + pngDir)).Count();
                return png;
            }
            catch (System.Exception ex)
            {
                var m = ex;
                return 0;
            }
        }

        public static string docGuid(string search = null)
        {
            List<string> logList = new List<string>();
            try
            {
                string docGuid = string.Empty;
                string logFile = string.Format("log.{0}.log", DateTime.Now.ToString("yyyyMMdd"));
                string logFilePath = Path.Combine(logs, logFile);
                using (var fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        logList.Add(sr.ReadLine());
                    }
                }
                for (int i = logList.Count - 1; i >= 0; i--)
                {
                    string s = logList[i];
                    if (s.IndexOf(search) != -1)
                    {
                        for (int ind2 = i; ind2 < logList.Count; ind2++)
                        {
                            string s2 = logList[ind2];
                            if (s2.IndexOf("Создание директории") != -1)
                            {
                                int index = s2.Length - 36;
                                string guid = s2.Remove(0, index);
                                return guid;
                            }
                        }
                    }
                }
                return docGuid;
            }
            catch (System.Exception ex)
            {
                var m = ex;
                return string.Empty;
            }
        }
    }
}
