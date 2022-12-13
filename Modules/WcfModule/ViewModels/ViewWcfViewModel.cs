using System;
using System.Threading.Tasks;
using Prism.Mvvm;
using NLog;
using Medo.Core.Models;
using System.ServiceModel;
using Medo.Core.Contracts;
using Medo.Core.EventsAggregator;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Medo.Core;
using Medo.Core.Interfaces;
using Medo.Core.Enums;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Medo.Core.Models.ReportsSenderModel;
using Medo.Core.Structures;

namespace Medo.Modules.WcfModule.ViewModels
{
    class ViewWcfViewModel : WCFConnector, INotifyPropertyChanged
    {

        #region Конструктор
        public ViewWcfViewModel(IEventAggregator eventAggregator)
        {
            _aggregator = eventAggregator;
            CheckTimer.Interval = 10000;
            CheckTimer.Enabled = true;
            CheckTimer.Elapsed += CheckTimer_Tick;
            getCheckConnect();
            SubscribeEventsAggregator();
        }
        #endregion

        #region iNotifyPropertyChanged       
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        /// <summary>
        /// Подписка на события EventsAggregator
        /// </summary>
        void SubscribeEventsAggregator()
        {
            _aggregator.GetEvent<UpdateCurrentDocumentFromBaseEvent>().Subscribe(UpdateDocumentFromBase);
            _aggregator.GetEvent<UpdateNonBaseStateEvent>().Subscribe(UpdateNonBaseStates);
            _aggregator.GetEvent<UpdateBaseStateEvent>().Subscribe(UpdateBaseStates);
            _aggregator.GetEvent<RecognizeCroppedImageEvent>().Subscribe(RecognizeImage);
            _aggregator.GetEvent<UpdateICardEditorInterfaceEvent>().Subscribe(SaveCard);
            _aggregator.GetEvent<UpdateITextDocumentUpdaterInterfaceEvent>().Subscribe(ChangeActName);
            _aggregator.GetEvent<ViewDoublesEvent>().Subscribe(ViewDoublesDocuments);
            _aggregator.GetEvent<SetDefaultPdfFileEvent>().Subscribe(SetDefaultPdf);
            _aggregator.GetEvent<UpdateIzdanieOrgansAndActTypesEvent>().Subscribe(UpdateIzdanieOrgansAndActTypes);
            _aggregator.GetEvent<UpdateDocumentsFromBaseForDatesEvent>().Subscribe(UpdateDocumentsFromBaseForDates);
            _aggregator.GetEvent<DateSelectedEvent>().Subscribe(getReportsList);
            _aggregator.GetEvent<SendReportEvent>().Subscribe(sendReports);
            _aggregator.GetEvent<GetContactsModelEvent>().Subscribe(GetContactsModel);
            _aggregator.GetEvent<GetIzdanieStatusUpdateEvent>().Subscribe(GetIzdanieStatusUpdate);
            _aggregator.GetEvent<UpdateNotificationsFromIzdanieEvent>().Subscribe(UpdateNotificationsFromIzdanie);
            _aggregator.GetEvent<UpdateNotificationsFromIzdanieEvent>().Subscribe(UpdateNotificationsFromIzdanie);
            _aggregator.GetEvent<ReloadRecognizeDictionaryEvent>().Subscribe(ReloadRecognizeDictionary);
        }

        #region Объявление глобальных переменных
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        IEventAggregator _aggregator;
        private System.Timers.Timer CheckTimer = new System.Timers.Timer();

        #endregion

        #region Коллекции
        //Dictionary<Guid, byte[]> ImagesDictionary { get; set; }
        ConcurrentDictionary<Guid, Document> ServerDictionary { get; set; }
        #endregion

        #region Подписки EventsAggregator

        public void ReloadRecognizeDictionary()
        {
            RecognitionServer.ReloadDictionary();
        }


        async void UpdateNotificationsFromIzdanie()
        {
            try
            {
                if (MedoServer != null)
                {
                    int reports = await ReportSenderService.UpdateNotificationsFromIzdanie();
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


        async void GetContactsModel(Guid sguid)
        {
            try
            {
                if (MedoServer != null)
                {
                    var contact = await MedoServer.GetOrganContacts(sguid);
                    _aggregator.GetEvent<ContactsModelEvent>().Publish(contact);

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


        void GetIzdanieStatusUpdate(IIzdanieInterface updater)
        {
            try
            {
                if (MedoServer != null)
                {
                    MedoServer.UpdateIzdanieStatuses(updater);
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
        void UpdateNonBaseStates(INonBaseStatesInterface updater)
        {
            try
            {
                if (MedoServer != null)
                {
                    MedoServer.NonBaseStatesUpdater(updater);
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
        void UpdateBaseStates(IBaseStatesInterface updater)
        {
            try
            {
                if (MedoServer != null)
                {
                    MedoServer.BaseStatesUpdater(updater);
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
        async void UpdateIzdanieOrgansAndActTypes()
        {
            try
            {
                if (MedoServer != null)
                {
                    List<string> organs = await MedoServer.GetOrgansName();
                    List<string> actstype = await MedoServer.GetActsTypes();
                    foreach (string s in organs)
                    {
                        var item = new TextInlineSelection(s, string.Empty);
                        if (!Client.Collections.StaticCollections.OrganList.Contains(item))
                        {
                            Client.Collections.StaticCollections.OrganList.Add(item);
                        }
                    }
                    foreach (string s in actstype)
                    {
                        var item = new TextInlineSelection(s, string.Empty);
                        if (!Client.Collections.StaticCollections.TypeList.Contains(item))
                        {
                            Client.Collections.StaticCollections.TypeList.Add(item);
                        }
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

        void UpdateDocumentFromBase(IBaseInterface intf)
        {
            try
            {
                if (MedoServer != null)
                {
                    MedoServer.UpdateCurrentDocumentFromBase(intf);
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

        void RecognizeImage(RecognitionTypeStruct rec)
        {
            try
            {
                byte[] b = Core.Methods.BitmapMethods.BitmapToByte(rec.CroppedImage);
                RecognitionServer.SetBitmapForRecognize(b);
                string recognize = RecognitionServer.GetRecognition();
                rec.RecognizedString = recognize;
                _aggregator.GetEvent<RecognizeCompliteEvent>().Publish(rec);
            }
            catch (FaultException fex)
            {
                logger.Fatal(fex);
            }
            catch (CommunicationException ex)
            {
                logger.Fatal(ex);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        void SaveCard(ICardEditorInterface card)
        {
            try
            {
                if (MedoServer != null)
                {
                    MedoServer.SaveCard(card);
                    logger.Info(string.Format("Сохранена карточка: GUID: {0} {1} {2} {3} {4}", card.HeaderGuid.ToString("D"), card.OrganName, card.ActType, card.ChangedNumber == null ? card.DocumentNumber : card.ChangedNumber, card.DocumentText));
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


        void ChangeActName(ITextDocumentUpdater actName)
        {
            try
            {
                if (MedoServer != null)
                {
                    MedoServer.UpdateDocumentText(actName);
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
        void SetDefaultPdf(Document doc)
        {
            try
            {
                if (MedoServer != null)
                {
                    MedoServer.UpdateDefaultPdf(doc.HeaderGuid, doc.DefaultPdf);
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
        void UpdateDocumentsFromBaseForDates(IUpdateFromBaseModelInterface update)
        {
            try
            {
                if (MedoServer != null)
                {
                    MedoServer.RefreshDictionaryFromBaseForDates(update.dateFrom.Value, update.dateTo.Value);
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


        #endregion


        #region Публикации EventsAggregator
        private bool _IsConnected { get; set; }
        public bool IsConnected
        {
            get
            {
                return this._IsConnected;
            }
            set
            {
                if (this.IsConnected != value)
                {
                    this._IsConnected = value;
                    _aggregator.GetEvent<TrayConnectedEvent>().Publish(value);
                }
            }
        }
        private async void getImagesDictionary()
        {
            var images = (Dictionary<Guid, byte[]>)Core.DataSerializer.Deserialize(await MedoServer.ImagesDictionary());
            if (images != null)
            {
                Client.Collections.StaticCollections.Images = images;
            }
        }

        /// <summary>
        /// Получение словаря документов с сервера WCF
        /// </summary>
        private async void getDocumentsDictionary()
        {
            try
            {
                byte[] dict;
                if (ServerDictionary == null)
                {
                    getImagesDictionary();
                    dict = await MedoServer.GetDocumensCollection(true);
                    UpdateIzdanieOrgansAndActTypes();
                }
                else
                {
                    dict = await MedoServer.GetDocumensCollection(false);
                }
                if (dict != null)
                    ServerDictionary = (ConcurrentDictionary<Guid, Document>)DataSerializer.Deserialize(dict);
                if (ServerDictionary != null && ServerDictionary.Count > 0)
                {
                    _aggregator.GetEvent<GetDocumentsDictionaryEvent>().Publish(ServerDictionary);
                }

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        #endregion


        #region ReportsSender methods
        public async void getReportsList(DateTime? date = null)
        {
            try
            {
                DateTime? dat = date.HasValue ? date : DateTime.Now;
                var reports = await ReportSenderService.GetReportsForSelectedDate(dat.Value);
                if (reports != null)
                {
                    _aggregator.GetEvent<GetReportsListFromServerEvent>().Publish(reports);
                }

                var adresses = await ReportSenderService.GetNotificationSubjectList();
                if (adresses != null)
                {
                    _aggregator.GetEvent<GetNotificationAdressListFromServerEvent>().Publish(adresses);
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
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        public async void sendReports(ReportModel report)
        {
            try
            {
                var r = await ReportSenderService.ManualSendReport(report);
                if (r != null)
                {
                    _aggregator.GetEvent<SendReportCallbackEvent>().Publish(r);
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
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        #endregion

        #region Подключение и проверка подключения к сервису WCF

        private async void getCheckConnect()
        {
            await CheckConnect();
        }


        /// <summary>
        /// Проверка подключен ли модуль к серверу WCF
        /// </summary>
        private async Task CheckConnect()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    if (MedoServer != null)
                    {
                        if (MedoServer.ModuleIsLoaded())
                        {
                            IsConnected = true;
                        }
                        getDocumentsDictionary();
                    }
                    else
                    {
                        IsConnected = false;
                    }
                }
                catch (FaultException fex)
                {
                    logger.Fatal(fex);
                    IsConnected = false;
                    ConnectToService(ServiceEnum.MedoService);
                    ConnectToService(ServiceEnum.RecognitionService);
                }
                catch (CommunicationException cex)
                {
                    logger.Fatal(cex);
                    IsConnected = false;
                    ConnectToService(ServiceEnum.MedoService);
                    ConnectToService(ServiceEnum.RecognitionService);
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex, "Ошибка соединения! начинаю новую попытку");
                    IsConnected = false;
                    ConnectToService(ServiceEnum.MedoService);
                    ConnectToService(ServiceEnum.RecognitionService);
                }
            });
        }

        public async void ViewDoublesDocuments(IBaseInterface doc)
        {
            try
            {
                var dict = await MedoServer.GetDoublesList(doc);
                _aggregator.GetEvent<GetDoublesDictionaryEvent>().Publish(dict);
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

        async void CheckTimer_Tick(object sender, EventArgs e)
        {
            await CheckConnect();
        }
        #endregion
    }
}
