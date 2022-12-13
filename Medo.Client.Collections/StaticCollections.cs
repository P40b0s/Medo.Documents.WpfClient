using Medo.Core.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Medo.Core.Interfaces;
using Medo.Core.Collections;
using Prism.Commands;
using Medo.Core.Enums;
using System.Collections.ObjectModel;
using Medo.Core;
using System.Collections.Concurrent;
using Medo.Core.Collections.Concurrent;
using Prism.Events;
using Medo.Core.EventsAggregator;
using System.Windows.Media;
using System.Resources;
using System.Media;
using System.IO;
using NAudio.Wave;
using System.Threading;

namespace Medo.Client.Collections
{
    public class StaticCollections
    {
       // static NAudio.Wave.Mp3FileReader audioFileReader { get; set; }
        //static IWavePlayer waveOutDevice { get; set; }
       // private static readonly Stream mp3;
        #region Инициализация статического класса
        public StaticCollections() {
            //waveOutDevice = new WaveOut();
            //System.Reflection.Assembly exe = System.Reflection.Assembly.Load(File.ReadAllBytes("Medo.ImageResources.dll"));
            //mp3 = exe.GetManifestResourceStream("Medo.ImageResources.Sounds.solemn.mp3");
            //audioFileReader = new Mp3FileReader(mp3);
        }
        private static readonly StaticCollections scollections = new StaticCollections();
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        static protected void OnStaticPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (StaticPropertyChanged != null)
            {
                StaticPropertyChanged(scollections, new PropertyChangedEventArgs(propertyName));
            }

        }
        #endregion


        private static IEventAggregator eventAggregator { get; set; }
        readonly static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Коллекция с парой SourceGuid - Image
        /// </summary>
        public static Dictionary<Guid, byte[]> Images;
        public static DocumentsCollection MainCollection { get; set; }
        /// <summary>
        /// Словарь для бекапа наименования документа (чтобы можно было вернуть первоначальную версию)
        /// </summary>


        #region Органы и виды документов из Издания
        public static ConcurrentObservableCollection<TextInlineSelection> OrganList { get; set; }
        public static ConcurrentObservableCollection<TextInlineSelection> TypeList { get; set; }
        #endregion

        #region Количество объектов в коллекциях
        /// <summary>
        /// Выбранный в данный момент документ
        /// </summary>
        public static Document SelectedDocument
        {
            get { return _SelectedDocument; }
            set
            {
                if (SelectedDocument != value)
                {
                    _SelectedDocument = value;
                    OnStaticPropertyChanged();
                }
            }
        }

        private static Document _SelectedDocument { get; set; }

        public static double DocumentsUploadProcessMaximum
        {
            get { return _DocumentsUploadProcessMaximum; }
            set
            {
                if (DocumentsUploadProcessMaximum != value)
                {
                    _DocumentsUploadProcessMaximum = value;
                    OnStaticPropertyChanged();
                }
            }
        }
        private static double _DocumentsUploadProcessMaximum { get; set; }

        public static double DocumentsUploadProcessValue
        {
            get { return _DocumentsUploadProcessValue; }
            set
            {
                if (DocumentsUploadProcessValue != value)
                {
                    _DocumentsUploadProcessValue = value;
                    OnStaticPropertyChanged();
                }
            }
        }
        private static double _DocumentsUploadProcessValue { get; set; }

        private static DateTime _LastUpdateData { get; set; }
        public static DateTime LastUpdateData
        {
            get
            {
                return _LastUpdateData;
            }
            set
            {
                if (LastUpdateData != value)
                {
                    _LastUpdateData = value;
                    OnStaticPropertyChanged();
                }
            }
        }

        #endregion

        private static void SubscribeEvents()
        {
            eventAggregator.GetEvent<UpdateDocumentEvent>().Subscribe(d => MainCollection.AddOrUpdate(d));
            eventAggregator.GetEvent<UpdateICardEditorInterfaceEvent>().Subscribe(d => MainCollection.AddOrUpdate(d));
            eventAggregator.GetEvent<UpdateICardEditorInterfaceEvent>().Subscribe(d => MainCollection.AddOrUpdate(d));
            eventAggregator.GetEvent<UpdateITextDocumentUpdaterInterfaceEvent>().Subscribe(d => MainCollection.AddOrUpdate(d));
            eventAggregator.GetEvent<UpdateBaseStateEvent>().Subscribe(d => MainCollection.AddOrUpdate(d));
            eventAggregator.GetEvent<UpdateNonBaseStateEvent>().Subscribe(d => MainCollection.AddOrUpdate(d));


            eventAggregator.GetEvent<GetDocumentsDictionaryEvent>().Subscribe(DocumentsUpdated);

            eventAggregator.GetEvent<GetDoublesDictionaryEvent>().Subscribe(ViewDublicates);
            eventAggregator.GetEvent<ContactsModelEvent>().Subscribe(GetContacts);
        }

        #region Получение класса контактов
        private static void GetContacts(List<ContactsModel> clist)
        {
            MainCollection.Contacts.Clear();
            foreach(var people in clist)
            {
                if (!MainCollection.Contacts.Contains(people))
                    MainCollection.Contacts.Add(people);
            }
        }
        #endregion


        #region Процесс обновления списка документов с сервера
        public static double MainCollectionUpdatesCount
        {
            get { return _MainCollectionUpdatesCount; }
            set
            {
                if (MainCollectionUpdatesCount != value)
                {
                    _MainCollectionUpdatesCount = value;
                    OnStaticPropertyChanged();
                }
            }
        }
        private static double _MainCollectionUpdatesCount { get; set; }

        public static double MainCollectionUpdateProgress
        {
            get { return _MainCollectionUpdateProgress; }
            set
            {
                if (MainCollectionUpdateProgress != value)
                {
                    _MainCollectionUpdateProgress = value;
                    OnStaticPropertyChanged();
                }
            }
        }
        private static double _MainCollectionUpdateProgress { get; set; }

        private static System.Reflection.Assembly resources = System.Reflection.Assembly.Load(File.ReadAllBytes("Medo.ImageResources.dll"));
        public static bool playSound()
        {
            try
            {
                using (var output = new WaveOutEvent())
                {
                    using (var player = new Mp3FileReader(resources.GetManifestResourceStream("Medo.ImageResources.Sounds.solemn.mp3")))
                    {
                        output.Init(player);
                        output.Play();
                        while (output.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(500);
                        }
                    }
                }
                return false;
            }
            catch(Exception e)
            {
                logger.Fatal(e);
                return true;
            }
           
        }

        /// <summary>
        /// Получение полной коллекции или обновленных документов с сервера WCF
        /// </summary>
        /// <param name="serverDictionary"></param>
        static void DocumentsUpdated(ConcurrentDictionary<Guid, Document> serverDictionary)
        {
            try
            {
                MainCollectionUpdatesCount = serverDictionary.Count;
                MainCollectionUpdateProgress = 0;
                foreach (var data in serverDictionary)
                {
                    if (!MainCollection.Contains(data.Value))
                    {
                        #region Сообщения о новом документе
                        //if (dictionaryCount < DocumentsCount)
                        if (data.Value.Status == DocumentStatus.INSERT)
                        {
                            eventAggregator.GetEvent<NewDocumentNotificationEvent>().Publish(data.Value);
                            logger.Info(String.Format("Получен документ: {0} - {1} - {2} от {3} директория {4}",
                                data.Value.ActType,
                                data.Value.OrganName,
                                data.Value.DocumentNumber,
                                data.Value.SignDate.HasValue ? data.Value.SignDate.Value.ToString("dd.MM.yyyy") : null,
                                data.Value.DirectoryName));
                            var p = playSound();
                        }
                        #endregion
                    }
                    MainCollection.AddOrUpdate(data.Value);
                    MainCollectionUpdateProgress++;
                }
                eventAggregator.GetEvent<CompleteUpdateMainCollectionEvent>().Publish();
                LastUpdateData = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        #endregion

        #region Проверка дубликатов документов
        private static void ViewDublicates(Dictionary<Guid, List<DoubleStatus>> dict)
        {
            try
            {
                MainCollection.ActiveFilters.SpecializedFiltrationList = new List<Guid>();
                MainCollection.ActiveFilters.AddOrChangeFilter(new Filtration.FiltrationRule() { FilterType = ActiveFilterEnum.Specialized, IsGenericRule = false });
                foreach (var doub in dict)
                {
                    try
                    {
                        Document document = MainCollection.FirstOrDefault(d => d.HeaderGuid == doub.Key);
                        document.DoubleIdentification = doub.Value;
                        MainCollection.ActiveFilters.SpecializedFiltrationList.Add(document.HeaderGuid);
                    }
                    catch (Exception ex)
                    {
                        logger.Fatal(ex);
                    }
                }

                MainCollection.ActiveFilters.SourceCollectionFiltration(MainCollection);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        #endregion

        public static void InitializeStaticCollections(IEventAggregator _eventAggregator)
        {
            try
            {
                eventAggregator = _eventAggregator;
                MainCollection = new DocumentsCollection();
                Images = new Dictionary<Guid, byte[]>();


                OrganList = new ConcurrentObservableCollection<TextInlineSelection>();
                TypeList = new ConcurrentObservableCollection<TextInlineSelection>();

                SubscribeEvents();
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }


        public static bool CommandsIsActive
        {
            get
            {
                return MainCollection != null;
            }
        }

    }
}
