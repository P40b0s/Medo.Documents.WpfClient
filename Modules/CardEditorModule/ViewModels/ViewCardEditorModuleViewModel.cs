using Prism.Events;
using System;
using Medo.Core.Models;
using Medo.Core.ErrorsValidation;
using System.ComponentModel;
using System.Collections.Concurrent;
using Medo.Core.Enums;
using Medo.Core.EventsAggregator;
using Medo.Core.Interfaces;
using Prism.Commands;
using Medo.Core.Structures;
using System.Collections.ObjectModel;

namespace Medo.Modules.CardEditorModule.ViewModels
{
    class ViewCardEditorModuleViewModel : SpellCheckAutomat
    {

        const byte MJREQUSITSCOUNT = 6;
        const byte MIDREQUSITSCOUNT = 3;
        const byte ALLREQUSITSCOUNT = 4;
        public Document EditDocument { get; set; }
        IEventAggregator _aggregator;
        object _lock = new object();
        public ViewCardEditorModuleViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        
        {
            this._aggregator = eventAggregator;
            EventAggregatorSubscribe();
            IsRegisterNotAllowed = false;
            CommandInitialization();
            this.PropertyChanged += ViewCardEditorModuleViewModel_PropertyChanged;
        }

        private void ViewCardEditorModuleViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CriticalErrorsCount")
                getErrorProgressBarValue();
        }

        private void getErrorProgressBarValue()
        {
            ErrorsProgressBarValue = RequsitsCount - CriticalErrorsCount;
        }
            
        void LoadDocumentForEdit(Document doc)
        {
            EditDocument = doc;
            this.errorsDictionary = new ConcurrentDictionary<string, ObservableCollection<CustomErrorType>>();
            Update((ICardEditorInterface)doc);
            
            IsMJDocument = false;
            RequsitsCount = ALLREQUSITSCOUNT;
            if (SourceGuid == Guid.Parse(Helpers.SourceGuidOrgansNames.Минюст))
            {
                IsMJDocument = true;
                RequsitsCount = MJREQUSITSCOUNT;
            }
            if (SourceGuid == Guid.Parse(Helpers.SourceGuidOrgansNames.МИД))
            {
                RequsitsCount = MIDREQUSITSCOUNT;
            }
            getErrorProgressBarValue();
        }

        void EventAggregatorSubscribe()
        {
            _aggregator.GetEvent<LoadDocumentIntoCardEditorEvent>().Subscribe(LoadDocumentForEdit);
            _aggregator.GetEvent<RecognizeCompliteEvent>().Subscribe(GetTextFromRecognizeService);
            _aggregator.GetEvent<CancelSaveCardEditorEvent>().Subscribe(ClearData);
        }
        #region Комманды
        public DelegateCommand ReloadRecognitionDictionaryCommand { get; set; }
        public DelegateCommand SaveCardCommand { get; set; }
        public DelegateCommand TextToLowerCaseCommand { get; set; }
        public DelegateCommand ClearTextCommand { get; set; }
        private void CommandInitialization()
        {
            SaveCardCommand = new DelegateCommand(SaveCard);
            TextToLowerCaseCommand = new DelegateCommand(TextToLowerCase);
            ClearTextCommand = new DelegateCommand(ClearText);
            ReloadRecognitionDictionaryCommand = new DelegateCommand(ReloadRecognitionDictionary);
        }
        #endregion

        private void ReloadRecognitionDictionary()
        {
            _aggregator.GetEvent<ReloadRecognizeDictionaryEvent>().Publish();
        }
        private void TextToLowerCase()
        {
            DocumentText = DocumentText.ToLower();
        }
        private void ClearText()
        {
            DocumentText = string.Empty;
        }

        /// <summary>
        /// Очстка нужных параметров
        /// </summary>
        void ClearData()
        {
            RequsitsCount = 0;
            CriticalErrorsCount = 0;
            ErrorsProgressBarValue = 0;
        }



        #region Байндинги
        private bool _IsMJDocument { get; set; }
        public bool IsMJDocument
        {
            get
            {
                return this._IsMJDocument;
            }
            set
            {
                if (this._IsMJDocument != value)
                {
                    this._IsMJDocument = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsRegisterNotAllowed { get; set; }
        public bool IsRegisterNotAllowed
        {
            get
            {
                return this._IsRegisterNotAllowed;
            }
            set
            {
                if (this._IsRegisterNotAllowed != value)
                {
                    this._IsRegisterNotAllowed = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        private void SaveCard()
        {
            IsEdit = false;
            CardEditDateTime = DateTime.Now;
            ComputerName = Environment.MachineName;
            if (DefferedOpen.HasValue && DefferedOpen > DateTime.Now)
            {
                var sedoperation = new Client.SedRegistration.SedOperations();
                sedoperation.ChangeSedComment(SourceGuid, DocumentNumber, "Документ отложен до: " + DefferedOpen.Value.ToShortDateString(), Client.SedRegistration.SedCommentIcon.StopIcon);
            }
            //Чтобы не городить черти что сначала апдейтим редактируемый документ а потом прогоняем через стандартный метод апдейта документа
            //Иначе не удасться малой кровью передать ICardEditorInterface этого класса через WCF (он не сериализуется и не указан как KnownTypeAttribute)
            // Итого - двойной апдейт одного и того же документа, с временем обработки незаметным глазу супермена...
            EditDocument.Update((ICardEditorInterface)this);
            _aggregator.GetEvent<UpdateICardEditorInterfaceEvent>().Publish((ICardEditorInterface)EditDocument);
            ClearData();      
        }

        public void ReloadRecognizeDictionary()
        {
            _aggregator.GetEvent<ReloadRecognizeDictionaryEvent>().Publish();
            
        }

        void GetTextFromRecognizeService(RecognitionTypeStruct s)
        {
            switch (s.RecognitionType)
            {
                case RecognitionTypeEnum.Наименование:
                    {
                        this.DocumentText = s.RecognizedString;
                        break;
                    }
                case RecognitionTypeEnum.Номер:
                    {
                        this.ChangedNumber = s.RecognizedString;
                        break;
                    }
                case RecognitionTypeEnum.Орган:
                    {
                        this.OrganName = s.RecognizedString;
                        break;
                    }
                //case RecognitionTypeEnum.Подписан:
                //    {
                //        DateTime dat = DateTime.MinValue;
                //        string data = s.RecognizedString.Replace(" ", string.Empty);
                //        string[] dataFormats = new string[] {
                //            "dd.MM.yyyy",
                //            "ddMMMMyyyy"
                //        };
                //        IFormatProvider provider = System.Globalization.CultureInfo.CurrentCulture;
                //        DateTime.TryParseExact(data, dataFormats, provider, System.Globalization.DateTimeStyles.None, out dat);
                //        if (dat != DateTime.MinValue)
                //        {
                //            this.SignDate = dat;
                //        }

                //        break;
                //    }
            }
        }

        #region Органы и типы документов

        //public void GetOrgans(List<string> s)
        //{
        //    try
        //    {
        //        var list = s.Select(item => new TextInlineSelection(item, string.Empty)).ToList();
        //        if (OrganName != null)
        //        {
        //            TextInlineSelection newitem = new TextInlineSelection(OrganName, string.Empty);
        //            if (list.IndexOf(newitem) == -1)
        //            {
        //                list.Insert(0, newitem);
        //            }
        //        }
        //        OrganList = list;
        //        BindingOperations.EnableCollectionSynchronization(OrganList, _lock);


        //    }
        //    catch (System.Exception ex)
        //    {
        //        logger.Fatal(ex);
        //    }
        //}

        //public void GetActTypes(List<string> s)
        //{
        //    try
        //    {
        //        var list = s.Select(item => new TextInlineSelection(item, string.Empty)).ToList();
        //        if (ActType != null)
        //        {
        //            TextInlineSelection newitem = new TextInlineSelection(ActType, string.Empty);
        //            if (list.IndexOf(newitem) == -1)
        //            {
        //                list.Insert(0, newitem);
        //            }
        //        }
        //        TypeList = list;
        //        BindingOperations.EnableCollectionSynchronization(TypeList, _lock);


        //    }
        //    catch (System.Exception ex)
        //    {
        //        logger.Fatal(ex);
        //    }
        //}

        //private List<TextInlineSelection> _OrganList { get; set; }
        //public List<TextInlineSelection> OrganList
        //{
        //    get
        //    {
        //        return this._OrganList;
        //    }
        //    set
        //    {
        //        if (this.OrganList != value)
        //        {
        //            this._OrganList = value;
        //            this.OnPropertyChanged("OrganList");
        //        }
        //    }
        //}
        //private List<TextInlineSelection> _TypeList { get; set; }
        //public List<TextInlineSelection> TypeList
        //{
        //    get
        //    {
        //        return this._TypeList;
        //    }
        //    set
        //    {
        //        if (this.TypeList != value)
        //        {
        //            this._TypeList = value;
        //            this.OnPropertyChanged("TypeList");
        //        }
        //    }
        //}

        #endregion

        #region Валидация данных
        private int _RequsitsCount { get; set; }
        public int RequsitsCount
        {
            get
            {
                return this._RequsitsCount;
            }
            set
            {
                if (this.RequsitsCount != value)
                {
                    this._RequsitsCount = value;
                    this.OnPropertyChanged();
                }
            }
        }
       

        private int _ErrorsProgressBarValue { get; set; }
        public int ErrorsProgressBarValue
        {
            get
            {
                return this._ErrorsProgressBarValue;
            }
            set
            {
                if (this.ErrorsProgressBarValue != value)
                {
                    this._ErrorsProgressBarValue = value;
                    this.OnPropertyChanged();
                }
            }
        }
        #endregion

      
    }
}
