using Medo.Core.EventsAggregator;
using Medo.Core.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.DocumentsUploaderModule.IzdanieIntegrationService
{
    class IzdanieUploader : INotifyPropertyChanged
    {
        #region Глобальные переменные
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string medodir = "D:\\Medo\\";
        private const string login = "nikitin";
        private const string password = "ὕͼ꟥䦒菮첺뇔";
        private const string logs = "\\\\182.5.202.220\\logs";
        private const string firstSearchString = "Process document:";
        private const string pngDir = "\\Png\\";
        private const string IzdanieDocumentsDirectory = "\\\\182.5.202.220\\IzdanieDocs\\";
        private const double msForOnePage = 240;

        System.Timers.Timer CTimer;
        WSHttpBinding binding;
        EndpointAddress address;
        MedoIntegrationServiceClient client;
        DateTime countDownDate = DateTime.Now;
        #endregion

        #region NotifyPropertyMembers
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private string _TimeToEnd { get; set; }
        /// <summary>
        /// Примерное время оставшееся для загрузки данного документа
        /// </summary>
        public string TimeToEnd
        {
            get
            {
                return this._TimeToEnd;
            }
            set
            {
                if (this.TimeToEnd != value)
                {
                    this._TimeToEnd = value;
                    this.OnPropertyChanged();

                }
            }
        }
        #region Общее время загрузки всех документов

        private string _TotalTime { get; set; }
        /// <summary>
        /// Примерное время оставшееся для загрузки списка документов
        /// </summary>
        public string TotalTime
        {
            get
            {
                return this._TotalTime;
            }
            set
            {
                if (this.TotalTime != value)
                {
                    this._TotalTime = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private int _TotalPages { get; set; }
        public int TotalPages
        {
            get
            {
                return this._TotalPages;
            }
            set
            {
                if (this.TotalPages != value)
                {
                    this._TotalPages = value;
                    TotalCountdownTime = DateTime.Now.AddMilliseconds(TotalPages * msForOnePage);
                    TotalMaximum = Math.Round(TotalCountdownTime.Subtract(DateTime.Now).TotalSeconds, 0);
                    TotalValue = 0;
                    this.OnPropertyChanged();

                }
            }
        }
        private double _TotalValue { get; set; }
        public double TotalValue
        {
            get
            {
                return this._TotalValue;
            }
            set
            {
                if (this.TotalValue != value)
                {
                    this._TotalValue = value;
                    this.OnPropertyChanged();
                    if (value == 0)
                    {
                        logger.Info($"{TotalValue}");
                    }
                }
            }
        }
        private double _TotalMaximum { get; set; }
        public double TotalMaximum
        {
            get
            {
                return this._TotalMaximum;
            }
            set
            {
                if (this.TotalMaximum != value)
                {
                    this._TotalMaximum = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private DateTime TotalCountdownTime { get; set; }
        
        #endregion
        private bool _UploadToIzdanieInProgress { get; set; }
        public bool UploadToIzdanieInProgress
        {
            get
            {
                return this._UploadToIzdanieInProgress;
            }
            set
            {
                if (this.UploadToIzdanieInProgress != value)
                {
                    this._UploadToIzdanieInProgress = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private double _ProcessMaximum { get; set; }
        /// <summary>
        /// Максимум процесса загрузки данного документа
        /// </summary>
        public double ProcessMaximum
        {
            get
            {
                return this._ProcessMaximum;
            }
            set
            {
                if (this.ProcessMaximum != value)
                {
                    this._ProcessMaximum = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private double _ProcessValue { get; set; }
        /// <summary>
        /// Процесс загрузки данного документа
        /// </summary>
        public double ProcessValue
        {
            get
            {
                return this._ProcessValue;
            }
            set
            {
                if (this.ProcessValue != value)
                {
                    this._ProcessValue = value;
                    this.OnPropertyChanged();

                }
            }
        }

        #endregion

        public IzdanieUploader()
        {
            Uri tcpUri = new Uri("http://182.5.202.220/wcf/MedoIntegrationService.svc");
            address = new EndpointAddress(tcpUri);
            binding = new WSHttpBinding(SecurityMode.None);
            binding.MaxReceivedMessageSize = 2147483647;
            binding.ReceiveTimeout = new System.TimeSpan(8, 0, 0);
            binding.SendTimeout = new System.TimeSpan(8, 0, 0);
            client = new MedoIntegrationServiceClient(binding, address);
            CTimer = new System.Timers.Timer();
            CTimer.Interval = 1000;
            CTimer.Elapsed += CTimer_Elapsed;
            CTimer.Start();
            CTimer.Enabled = false;
        }
        private void CTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(countDownDate > DateTime.Now)
            {
                TimeSpan ts = countDownDate.Subtract(DateTime.Now);
                TimeToEnd = ts.ToString("m' мин 's' сек'");
                ProcessValue++;
            }
            if (TotalCountdownTime > DateTime.Now)
            {
                TimeSpan ts = TotalCountdownTime.Subtract(DateTime.Now);
                TotalTime = ts.ToString("m' минут 's' секунд'");
                TotalValue++;
            }
        }


        #region Загрузка документов в Издание
        public void UploadToIzdanie(ref DocumentsCopy data)
        {
            try
            {
                UploadToIzdanieInProgress = true;
                ProcessValue = 0;
                if(data.PagesCount != 0)
                {
                    countDownDate = DateTime.Now.AddMilliseconds(data.PagesCount * msForOnePage);
                    ProcessMaximum = Math.Round(countDownDate.Subtract(DateTime.Now).TotalSeconds, 0);
                    CTimer.Enabled = true;
                }
                else
                {
                    TimeToEnd = "количество страниц неизвестно...";
                }
                if (client.State != CommunicationState.Opened)
                {
                    client.Open();
                }
                   
                if (client.State == CommunicationState.Opened)
                {
                    client.ProcessDocument(System.IO.Path.Combine(medodir, data.DirectoryName), login, password);
                    data.CopyMessage.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " Документ успешно загружен в систему Издание");
                    
                    data.UploadToIzdanieIsCompliete = true;
                    UploadToIzdanieInProgress = false;
                    CTimer.Enabled = false;
                }
                else throw new NullReferenceException("MedoIntegrationServiceClient не подключен!");
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                CTimer.Enabled = false;
                data.CopyMessage.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " " + ex.Message);
                data.UploadToIzdanieIsCompliete = false;
                UploadToIzdanieInProgress = false;
            }
        }
        #endregion
    }
}
