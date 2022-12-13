using Medo.Core.Contracts;
using Medo.Core.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.WcfModule
{
    public enum ServiceEnum {MedoService, RecognitionService, ReportsService, AllServices };
    class WCFConnector
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
       
        //private const string MedoWcfServicePath = "net.tcp://182.5.202.96:1000/MedoServer";
        //private const string RecognitionWcfServicePath = "net.tcp://182.5.202.96:1000/RecognitionServer";

        private const string MedoWcfServicePath = "net.tcp://182.5.202.221:1000/MedoServer";
        private const string RecognitionWcfServicePath = "net.tcp://182.5.202.221:1000/RecognitionServer";

        private const string ReportsSenderServicePath = "net.tcp://182.5.202.221:999/ReportSenderService";

        public IRecognitionServerContract RecognitionServer { get; set; }
        public IMedoServerContract MedoServer { get; set; }
        public IReportServiceContract ReportSenderService { get; set; }
        #region Фабрики
        private ChannelFactory<IRecognitionServerContract> recognitionFactory
        {
            get
            {
                try
                {
                    var adress = getServiceAdress(RecognitionWcfServicePath);
                    return new ChannelFactory<IRecognitionServerContract>(adress.binding, adress.adress);
                }
                catch (FaultException fex)
                {
                    logger.Fatal(fex);
                    return null;
                }
                catch (CommunicationException ex)
                {
                    logger.Fatal(ex);
                    return null;
                }
                catch (Exception ex1)
                {
                    logger.Fatal(ex1);
                    return null;
                } 
            }
        }
        private ChannelFactory<IMedoServerContract> medoFactory
        {
            get
            {
                try
                {
                    var adress = getServiceAdress(MedoWcfServicePath);
                    return new ChannelFactory<IMedoServerContract>(adress.binding, adress.adress);
                }
                catch (FaultException fex)
                {
                    logger.Fatal(fex);
                    return null;
                }
                catch (CommunicationException ex)
                {
                    logger.Fatal(ex);
                    return null;
                }
                catch (Exception ex1)
                {
                    logger.Fatal(ex1);
                    return null;
                }
            }
        }
        private ChannelFactory<IReportServiceContract> reportSenderFactory
        {
            get
            {
                try
                {
                    var adress = getServiceAdress(ReportsSenderServicePath);
                    return new ChannelFactory<IReportServiceContract>(adress.binding, adress.adress);
                }
                catch (FaultException fex)
                {
                    logger.Fatal(fex);
                    return null;
                }
                catch (CommunicationException ex)
                {
                    logger.Fatal(ex);
                    return null;
                }
                catch (Exception ex1)
                {
                    logger.Fatal(ex1);
                    return null;
                }
            }
        }
        #endregion

        public WCFConnector()
        {
            ConnectToService();
        }

        public void ConnectToService(ServiceEnum serv = ServiceEnum.AllServices)
        {
            try
            {
                switch (serv)
                {
                    case ServiceEnum.AllServices:
                        {
                            RecognitionServer = recognitionFactory.CreateChannel();
                            MedoServer = medoFactory.CreateChannel();
                            ReportSenderService = reportSenderFactory.CreateChannel();
                            break;
                        }
                    case ServiceEnum.MedoService:
                        {
                            MedoServer = medoFactory.CreateChannel();
                            break;
                        }
                    case ServiceEnum.RecognitionService:
                        {
                            RecognitionServer = recognitionFactory.CreateChannel();
                            break;
                        }
                    case ServiceEnum.ReportsService:
                        {
                            ReportSenderService = reportSenderFactory.CreateChannel();
                            break;
                        }
                }
            }
            catch (FaultException fex)
            {
                logger.Fatal(fex);
            }
            catch (CommunicationException ex)
            {
                logger.Fatal(ex);
            }
            catch (Exception ex1)
            {
                logger.Fatal(ex1);
            }
          
        }

        private WcfAdressObject getServiceAdress(string adress)
        {
            WcfAdressObject obj = new WcfAdressObject();
            try
            {
                Uri tcpUri = new Uri(adress);
                EndpointAddress address = new EndpointAddress(tcpUri);
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
                binding.MaxReceivedMessageSize = 2147483647;
                binding.ReceiveTimeout = new System.TimeSpan(8, 0, 0);
                binding.SendTimeout = new System.TimeSpan(8, 0, 0);
                //TcpTransportSecurity transport = new TcpTransportSecurity();
                //transport.ClientCredentialType = TcpClientCredentialType.Windows;
                //NetTcpSecurity sec = new NetTcpSecurity();
                //sec.Mode = SecurityMode.Transport;
                //sec.Transport = transport;
                //binding.Security = sec;
                obj.adress = address;
                obj.binding = binding;
                return obj;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return obj;
            }
        }

    }
}
