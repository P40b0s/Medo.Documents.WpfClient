using Medo.Core.EventsAggregator;
using Medo.Core.Models;
using NLog;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client.GlobalCommands
{
    public static class Commands
    {
        readonly static Logger logger = LogManager.GetCurrentClassLogger();
        private static IEventAggregator _EventAggregator { get; set; }
        public static void InitializeCommands(IEventAggregator _aggregator)
        {
            _EventAggregator = _aggregator;
            PdfViewerIsVisibleCommand = new DelegateCommand(PdfViewerIsVisible);
            StartDocumentsUploadCommand = new DelegateCommand(StartDocumentsUpload);
            DocumentsEditorWindowIsOpenCommand = new DelegateCommand(DocumentsEditorWindowIsOpen);
            ClientUpdateCenterWindowIsOpenCommand = new DelegateCommand(ClientUpdateCenterWindowIsOpen);
            IsSelectedDocumentCommand = new DelegateCommand<object>(IsSelectedDocument);
            UpdateIzdanieOrgansAndActTypesCommand = new DelegateCommand(UpdateIzdanieOrgansAndActTypes);
            ChangeActNameCommand = new DelegateCommand<object>(ChangeActName);
            StartFilteringCommand = new DelegateCommand(StartFiltering);
            OneDocumentFilteringCommand = new DelegateCommand<object>(OneDocumentFiltering);
            GetOrganContactsCommand = new DelegateCommand<object>(GetOrganContacts);
            AddNewDocumentToMedoCommand = new DelegateCommand<object>(AddNewDocumentToMedo);

        }

        #region Открытие модальных окон для взаимодействия с пользователем
        private static void PdfViewerIsVisible()
        {
            _EventAggregator.GetEvent<PdfViewerIsOpenEvent>().Publish();
        }
        private static void StartDocumentsUpload()
        {
            _EventAggregator.GetEvent<StartDocumentsUploadEvent>().Publish();
        }
        private static void DocumentsEditorWindowIsOpen()
        {
            _EventAggregator.GetEvent<DocumentEditorWindowIsOpenEvent>().Publish();
        }
        private static void ClientUpdateCenterWindowIsOpen()
        {
            _EventAggregator.GetEvent<UpdaterWindowIsOpenEvent>().Publish();
        }
        #endregion

        private static void AddNewDocumentToMedo(object obj)
        {

        }
        private static void GetOrganContacts(object c)
        {
            Guid g = (Guid)c;
            _EventAggregator.GetEvent<GetContactsModelEvent>().Publish(g);
        }
        /// <summary>
        /// Метод для команды выделения документа кнопкой
        /// </summary>
        /// <param name="HGuid">Guid документа передается при нажатии на кнопку выделения документа</param>
        private static void IsSelectedDocument(object HGuid)
        {
            try
            {
                Guid headerGuid = (Guid)HGuid;
                _EventAggregator.GetEvent<UpdateNonBaseStateEvent>().Publish(Collections.StaticCollections.MainCollection.AddOrUpdate(headerGuid));
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        /// <summary>
        /// Обновление принявших органов и видов документов из системы Издание
        /// </summary>
        private static void UpdateIzdanieOrgansAndActTypes()
        {
            _EventAggregator.GetEvent<UpdateIzdanieOrgansAndActTypesEvent>().Publish();
        }

        private static void StartFiltering()
        {
            _EventAggregator.GetEvent<StartFilteringEvent>().Publish();
        }
        private static void OneDocumentFiltering(object doc)
        {
            _EventAggregator.GetEvent<UpdateDocumentEvent>().Publish((Document)doc);
        }

        #region Удаление или восстановление наименования документа
        private static void ChangeActName(object obj)
        {
            //Collections.StaticCollections.SelectedDocument.DocumentText = (string)obj;
           // _EventAggregator.GetEvent<UpdateITextDocumentUpdaterInterfaceEvent>().Publish(Collections.StaticCollections.SelectedDocument);
        }
       
        #endregion


        /// <summary>
        /// Открытие или закрытие окна выгрузки документов в издание
        /// </summary>
        public static DelegateCommand StartDocumentsUploadCommand { get; set; }
        /// <summary>
        /// Открытие или закрытие окна центра обновлений клиентского приложения
        /// </summary>
        public static DelegateCommand ClientUpdateCenterWindowIsOpenCommand { get; set; }
        /// <summary>
        /// открытие или закрытие окна редактора карточки документа
        /// </summary>
        public static DelegateCommand DocumentsEditorWindowIsOpenCommand { get; set; }
        /// <summary>
        /// Открытие или закрытие окна просмотра PDF
        /// </summary>
        public static DelegateCommand PdfViewerIsVisibleCommand { get; set; }
        /// <summary>
        /// Изменение свойства IsSelected модели Document
        /// </summary>
        public static DelegateCommand<object> IsSelectedDocumentCommand { get; set; }
        /// <summary>
        /// Обновление принявших органов и видов документов из системы Издание
        /// </summary>
        public static DelegateCommand UpdateIzdanieOrgansAndActTypesCommand { get; set; }
        /// <summary>
        /// Начало редактирования карточки документа
        /// </summary>
        public static DelegateCommand OpenCardEditWindowCommand { get; set; }

        /// <summary>
        /// Замена или удаление наименования документа
        /// </summary>
        public static DelegateCommand<object> ChangeActNameCommand { get; set; }

        public static DelegateCommand StartFilteringCommand { get; set; }
        public static DelegateCommand<object> OneDocumentFilteringCommand { get; set; }

        public static DelegateCommand<object> GetOrganContactsCommand { get; set; }

        public static DelegateCommand<object> AddNewDocumentToMedoCommand { get; set; }
    }
}
