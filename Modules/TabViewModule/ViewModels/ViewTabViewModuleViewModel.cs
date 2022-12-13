using Collections;
using Medo.Core.Collections;
using Medo.Core.Models;
using Medo.Modules.TabViewModule.Filtration;
using NLog;
using Prism.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Threading;
using System.ServiceModel;
using Medo.Core.EventsAggregator;
using Prism.Commands;
using System.Collections.Generic;
using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using Medo.Core.Interfaces;
using Prism.Interactivity.InteractionRequest;
using Medo.Client.Notifications;
using Medo.Client.Notifications.Models;
using System.IO;
using Medo.Client.Notifications.Interfaces;
using Medo.Core.ErrorsValidation;
using Medo.Core.Enums;
using System.Windows.Threading;

namespace Medo.Modules.TabViewModule.ViewModels
{

    //Event Publications
    //DocumentSelectionChangedEvent
    //DocumentSelectionEvent
    //StartDocumentEditEvent
    //UpdateCurrentDocumentFromBaseEvent

    public class ViewTabViewModuleViewModel : FilteringAndSorting
    {
        Dispatcher disp = Dispatcher.CurrentDispatcher;
        #region Глобальные переменные
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        IEventAggregator _aggregator;
        #endregion

        #region Диалоги взаимодействия с пользователем (модальные окна)
        /// <summary>
        /// Запрос на удаление документа
        /// </summary>
        public InteractionRequest<DeleteDocumentNotificationModel> DocumentDeleteRequest { get; private set; }
        /// <summary>
        /// Запрус на замену документа
        /// </summary>
        public InteractionRequest<ChangeDocumentNotificationModel> DocumentChangeRequest { get; private set; }
        /// <summary>
        /// Отображение окна центра обновлений
        /// </summary>
        public InteractionRequest<ClientUpdaterRequestModel> UpdateCenterRequest { get; private set; }
        /// <summary>
        /// Диалоговое окно редактирования карточки документа
        /// </summary>
        public InteractionRequest<CardEditorRequestModel> CardEditorRequest { get; private set; }
        /// <summary>
        /// Диалоговое окно выбора дат для обновления базы
        /// </summary>
        public InteractionRequest<UpdateDocumentsForIntervalRequestModel> UpdateDocumentsForIntervalRequest { get; private set; }

        public InteractionRequest<PdfConverterModel> DocumentConvertRequest { get; private set; }

        /// <summary>
        /// Инициализация диалогов взаимодействия с пользователем
        /// </summary>
        private void InitializeRequests()
        {
            this.DocumentDeleteRequest = new InteractionRequest<DeleteDocumentNotificationModel>();
            this.DocumentChangeRequest = new InteractionRequest<ChangeDocumentNotificationModel>();
            this.UpdateCenterRequest = new InteractionRequest<ClientUpdaterRequestModel>();
            this.CardEditorRequest = new InteractionRequest<CardEditorRequestModel>();
            this.UpdateDocumentsForIntervalRequest = new InteractionRequest<UpdateDocumentsForIntervalRequestModel>();
            this.DocumentConvertRequest = new InteractionRequest<PdfConverterModel>();
        }
        #endregion

        public ViewTabViewModuleViewModel(IEventAggregator eventAggregator) : base (eventAggregator)
        
        {
            _aggregator = eventAggregator;
            try
            {
                SubscribeEvents();
                CommandsInitialization();
                ControlsIsEnabled = true;
                InitializeRequests();
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
     
        /// <summary>
        /// Подписывание на события
        /// </summary>
        private void SubscribeEvents()
        {
            _aggregator.GetEvent<StartDocumentsUploadEvent>().Subscribe(UploadDocumentsToIzdanie);
            _aggregator.GetEvent<UpdaterWindowIsOpenEvent>().Subscribe(UpdateWindowRequest);
            _aggregator.GetEvent<UpdateExistsEvent>().Subscribe(s => IsNewUpdateExists = s);
            _aggregator.GetEvent<DocumentEditorWindowIsOpenEvent>().Subscribe(CardEditorWindowRequest);
            _aggregator.GetEvent<UploadOrChangeDoneEvent>().Subscribe(() => UploadDocumentsInProgress = false);
            
        }

        #region Связанные свойства


        private bool _ControlsIsEnabled { get; set; }
        /// <summary>
        /// Активность Таба (должен быть неактивен во время редактирования карточки документа)
        /// </summary>
        public bool ControlsIsEnabled
        {
            get
            {
                return this._ControlsIsEnabled;
            }
            set
            {
                if (this.ControlsIsEnabled != value)
                {
                    this._ControlsIsEnabled = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsNewUpdateExists { get; set; }
        /// <summary>
        /// Есть ли обновления системы
        /// </summary>
        public bool IsNewUpdateExists
        {
            get
            {
                return this._IsNewUpdateExists;
            }
            set
            {
                if (this.IsNewUpdateExists != value)
                {
                    this._IsNewUpdateExists = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _DoublesIsSelected { get; set; }
        /// <summary>
        /// Открыта ли закладка с дублями документов
        /// </summary>
        public bool DoublesIsSelected
        {
            get
            {
                return this._DoublesIsSelected;
            }
            set
            {

                if (this.DoublesIsSelected != value)
                {
                    this._DoublesIsSelected = value;
                    this.OnPropertyChanged();

                }
            }
        }
        #endregion

        #region Команды
        /// <summary>
        /// Команда обновления выбранного документа напрямую из базы данных
        /// </summary>
        public DelegateCommand<object> UpdateCurrentDocumentFromBaseCommand { get; set; }
        /// <summary>
        /// Смена наименования документа
        /// </summary>
        public DelegateCommand<object> ChangeActNameCommand { get; set; }
        /// <summary>
        /// Возвращение наименование документа кторое было при получении
        /// </summary>
        public DelegateCommand<object> ReturnActNameCommand { get; set; }
        /// <summary>
        /// Закрытие окна выгрузки документов в издание
        /// </summary>
        public DelegateCommand CloseCopyWindowCommand { get; set; }
        /// <summary>
        /// Замена документа (Копирование документа на флешку и при необходимости в НТЦ систему) установка флага IsChanged = true
        /// </summary>
        public DelegateCommand<object> ChangeDocumentCommand { get; set; }
        /// <summary>
        /// Добавление документа в список удаленных (документ становитьсяневидимым в общем списке)
        /// </summary>
        public DelegateCommand<object> DeleteDocumentCommand { get; set; }
        /// <summary>
        /// Команда просмотра дубля у данного документа
        /// </summary>
        public DelegateCommand<object> ViewDoublesCommand { get; set; }

        public DelegateCommand UpdateDocumentsForIntervalCommand { get; set; }

        public DelegateCommand<object> DocumentConvertRequestCommand { get; set; }

        /// <summary>
        /// Инициализация команд
        /// </summary>
        void CommandsInitialization()
        {
            try
            {
                UpdateCurrentDocumentFromBaseCommand = new DelegateCommand<object>(UpdateCurrentDocumentFromBase);            
                ChangeActNameCommand = new DelegateCommand<object>(ChangeActName);
                CloseCopyWindowCommand = new DelegateCommand(CloseCopyWindow);
                ChangeDocumentCommand = new DelegateCommand<object>(ChangeDocument);
                DeleteDocumentCommand = new DelegateCommand<object>(DeleteDocument);
                ReturnActNameCommand = new DelegateCommand<object>(ReturnActName);
                ViewDoublesCommand = new DelegateCommand<object>(s => _aggregator.GetEvent<ViewDoublesEvent>().Publish((Document)s));
                UpdateDocumentsForIntervalCommand = new DelegateCommand(UpdateDocumentsForIntervalWindowRequest);
                DocumentConvertRequestCommand = new DelegateCommand<object>(DocumentConvertWindowRequest);

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        #endregion

        #region Публикации EventsAggregator

        void UpdateCurrentDocumentFromBase(object binterface)
        {
            try
            {
                if (binterface != null)
                {
                    _aggregator.GetEvent<UpdateCurrentDocumentFromBaseEvent>().Publish((IBaseInterface)binterface);
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }

        }
        private Document _SelectedItem { get; set; }
        /// <summary>
        /// Выбранный в данный момент документ
        /// </summary>
        public Document SelectedItem
        {
            get
            {
                return this._SelectedItem;
            }
            set
            {
                if (this.SelectedItem != value)
                {
                    this._SelectedItem = value;
                    this.OnPropertyChanged();
                    Client.Collections.StaticCollections.SelectedDocument = value;
                    if (value != null)
                    {
                        _aggregator.GetEvent<DocumentSelectionChangedEvent>().Publish(SelectedItem);
                    }
                }
            }
        }
        /// <summary>
        /// Смена наименования документа
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeActName(object obj)
        {
            ITextDocumentUpdater txt = new MedoBaseModel();
            txt.DocumentText = (string)obj;
            txt.HeaderGuid = SelectedItem.HeaderGuid;
            _aggregator.GetEvent<UpdateITextDocumentUpdaterInterfaceEvent>().Publish(txt);
        }
        /// <summary>
        /// Смена наименования документа
        /// </summary>
        /// <param name="obj">HeaderGuid документа</param>
        private void ReturnActName(object obj)
        {
            var hguid = (Guid)obj;
            if(Client.Collections.StaticCollections.MainCollection.ActNameRollBackDictionary.ContainsKey(hguid))
            {
                string etalon = Client.Collections.StaticCollections.MainCollection.ActNameRollBackDictionary[hguid];
                ITextDocumentUpdater txt = new MedoBaseModel();
                txt.DocumentText = etalon;
                txt.HeaderGuid = SelectedItem.HeaderGuid;
                _aggregator.GetEvent<UpdateITextDocumentUpdaterInterfaceEvent>().Publish(txt);
            }
        }

        #region Замена или выгрузка документов в систему Издание
        private void UploadDocumentsToIzdanie()
        {
            try
            {
                List<Document> docs = Client.Collections.StaticCollections.MainCollection.Where(d1 => d1.IsSelected == true).ToList();

                if (docs.Count() > 0)
                {
                    UploadDocumentsInProgress = true;
                    _aggregator.GetEvent<UploadSelectedDocumentsEvent>().Publish(docs);
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        private void ChangeDocumentsToIzdanie(ChangeDocumentNotificationModel change)
        {
            try
            {
                UploadDocumentsInProgress = true;
                _aggregator.GetEvent<ChangeDocumentEvent>().Publish(new ChangeDocumentModel
                {
                    document = change.OperationDocument,
                    flashDisk = change.SelectedFlashDisk.SelectedFlashDisk
                });
                SedRegistrationOnDocumentOperation(change);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private bool _UploadDocumentsInProgress { get; set; }
        /// <summary>
        /// Активна ли кнопка выгрузки документов
        /// </summary>
        public bool UploadDocumentsInProgress
        {
            get
            {
                return this._UploadDocumentsInProgress;
            }
            set
            {
                if (this.UploadDocumentsInProgress != value)
                {
                    this._UploadDocumentsInProgress = value;
                    this.OnPropertyChanged();

                }
            }
        }
        #endregion

        #region Методы регистрации в сэде 

        private Client.SedRegistration.SedModel getSedModel(Document doc)
        {
            try
            {
                Client.SedRegistration.SedModel sed = new Client.SedRegistration.SedModel();
                sed.DocGuid = doc.DocGuid;
                sed.DocumentName = doc.DocumentText.Trim();
                sed.Number = doc.DocumentNumber.Trim();
                sed.PagesCount = doc.PagesCount;
                sed.SGuid = doc.SourceGuid;
                sed.SignDate = doc.SignDate.HasValue ? doc.SignDate.Value : DateTime.Now;
                return sed;

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return null;
            }
        }
        /// <summary>
        /// Регистрация, удаление или отклонение с определенным статусом документов в СЭДе
        /// </summary>
        /// <param name="not"></param>
        private async void SedRegistrationOnDocumentOperation(DocumentOperationInterface not)
        {
            try
            {
                Client.SedRegistration.SedOperations sedoperation = new Client.SedRegistration.SedOperations();
                if (not.RegisterDocumentInSED)
                {
                    var sm = getSedModel(not.OperationDocument);
                    sm.Operation = Client.SedRegistration.SedOperationEnum.Register;
                    await sedoperation.Register(sm);
                }
                if (not.DeleteDocumentFromSED)
                {
                    var sm = getSedModel(not.OperationDocument);
                    sm.Operation = Client.SedRegistration.SedOperationEnum.Delete;
                    await sedoperation.Register(sm);
                }
                if (not.RejectRegistrationInSED)
                {
                    if (!string.IsNullOrEmpty(not.RejectStatus))
                    {
                        var sm = getSedModel(not.OperationDocument);
                        sm.Operation = Client.SedRegistration.SedOperationEnum.Refuse;
                        sm.RefuseStatus = not.RejectStatus;
                        await sedoperation.Register(sm);
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        #endregion

        #endregion

        #region Подписки EventsAggregator
        /// <summary>
        /// Получение полной коллекции или обновленных документов с сервера WCF
        /// </summary>
        /// <param name="serverDictionary"></param>
        //public void getDocs(ConcurrentDictionary<Guid, Document> serverDictionary)
        //{
        //    LoadOrgansAndActTypesForFiltering();
        //}

        #endregion

        #region Окно копирования документа
        /// <summary>
        /// Метод закрытия окна выгрузки документов
        /// </summary>
        private void CloseCopyWindow()
        {
            CopyWindowIsOpen = false;
        }
        private bool _CopyWindowIsOpen { get; set; }
        public bool CopyWindowIsOpen
        {
            get
            {
                return this._CopyWindowIsOpen;
            }
            set
            {
                if (this.CopyWindowIsOpen != value)
                {
                    this._CopyWindowIsOpen = value;
                    this.OnPropertyChanged();

                }
            }
        }
        #endregion


        #region Обработка диалогов взаимодействия с пользователем

        #region Замена документа
        /// <summary>
        /// Отправляем документ на замену (IsChanged = true + копирование документа на флешку - модуль DocumentsUploadModule
        /// </summary>
        /// <param name="obj">Класс Document</param>
        private void ChangeDocument(object obj)
        {
            try
            {
                Document doc = obj as Document;
                var d = new ChangeDocumentNotificationModel { OperationDocument = doc, Title = "Замены документа" };
                DocumentChangeRequest.Raise(d, returned =>
                {
                    if (returned.Confirmed && returned.OperationDocument != null && returned.SelectedFlashDisk.SelectedFlashDisk != null)
                    {
                        returned.OperationDocument.IsChanged = true;
                        ChangeDocumentsToIzdanie(returned);
                    }
                });
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }

        }
        #endregion

        #region Удаление документа    
        /// <summary>
        /// Удаление документа из общего списка или из списка удаленных
        /// </summary>
        /// <param name="obj">Класс Document</param>
        private void DeleteDocument(object obj)
        {
            try
            {
                Document doc = obj as Document;
                var d = new DeleteDocumentNotificationModel { OperationDocument = doc, Title = "Удаления документа" };
                DocumentDeleteRequest.Raise(d, returned =>
                {
                    if (returned.Confirmed && returned.OperationDocument != null)
                    {
                        if (returned.OperationDocument.IsInvisible)
                        {
                            returned.OperationDocument.IsInvisible = false;
                        }
                        else
                        {
                            returned.OperationDocument.IsInvisible = true;
                        }
                        _aggregator.GetEvent<UpdateBaseStateEvent>().Publish(returned.OperationDocument);
                        SedRegistrationOnDocumentOperation(returned);
                    }
                });
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        #endregion

        /// <summary>
        /// Отображение диалогового окна центра обновления программы
        /// </summary>
        private void UpdateWindowRequest()
        {
            var d = new ClientUpdaterRequestModel { Title = "Центр обновления" };
            UpdateCenterRequest.Raise(d);
        }
        /// <summary>
        /// Отображение диалогового окна редактирования карточки документа
        /// </summary>
        private void CardEditorWindowRequest()
        {
            if (SelectedItem != null)
            {
                SelectedItem.IsEdit = true;
                _aggregator.GetEvent<UpdateNonBaseStateEvent>().Publish(SelectedItem);
                _aggregator.GetEvent<LoadDocumentIntoCardEditorEvent>().Publish(SelectedItem);
                var d = new CardEditorRequestModel { Title = "Редактор карточки документа" };
                CardEditorRequest.Raise(d, returned =>
                {
                    if (!returned.Confirmed)
                    {
                        SelectedItem.IsEdit = false;
                        _aggregator.GetEvent<UpdateNonBaseStateEvent>().Publish(SelectedItem);
                    }
                });
            }
        }

        /// <summary>
        /// Отображение диалогового окна редактирования карточки документа
        /// </summary>
        private void DocumentConvertWindowRequest(object obj)
        {
            Document document = obj as Document;
            if (document != null)
            {
                var d = new PdfConverterModel { Title = "Конвертация документа", doc = document, PdfForConvert = new FileInfo(Path.Combine(Helpers.Paths.PakMedoFolder, document.DirectoryName, document.DefaultPdf)) };
                DocumentConvertRequest.Raise(d, returned =>
                {
                    if (!returned.Confirmed)
                    {
                    }
                });
            }
        }

       


        /// <summary>
        /// Отображение диалогового окна обновления документов из базы по выбранным датам
        /// </summary>
        private void UpdateDocumentsForIntervalWindowRequest()
        {
            var dates = new UpdateDocumentsForIntervalRequestModel { Title = "Выбор даты обновления документов", dateFrom = DateTime.Now.Date, dateTo = DateTime.Now.Date };
            UpdateDocumentsForIntervalRequest.Raise(dates, returned =>
            {
                if(returned.Confirmed)
                {
                    _aggregator.GetEvent<UpdateDocumentsFromBaseForDatesEvent>().Publish(returned);
                }
            });

        }
        #endregion

    }
}
