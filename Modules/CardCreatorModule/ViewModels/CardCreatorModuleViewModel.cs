using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Medo.Core.Models;
using Medo.Core.ErrorsValidation;
using System.Runtime.CompilerServices;
using NLog;
using System.ComponentModel;
using System.Collections.Concurrent;
using Medo.Core.Enums;
using Medo.Core.EventsAggregator;
using System.Windows.Data;
using Medo.Core.Interfaces;
using Prism.Commands;
using Medo.Core.Structures;

namespace Medo.Modules.CardCreatorModule.ViewModels
{
    class CardCreatorModuleViewModel : Document, INotifyDataErrorInfo
    {

        const byte MJREQUSITSCOUNT = 6;
        const byte MIDREQUSITSCOUNT = 3;
        const byte ALLREQUSITSCOUNT = 4;
        public Document EditDocument { get; set; }
        IEventAggregator _aggregator;
        object _lock = new object();
        public CardCreatorModuleViewModel(IEventAggregator eventAggregator)
        {
            this._aggregator = eventAggregator;
            EventAggregatorSubscribe();
            errorsDictionary = new ConcurrentDictionary<string, List<CustomErrorType>>();
            IsRegisterNotAllowed = false;
            CommandInitialization();
        }
        void LoadDocumentForEdit(Document doc)
        {
            EditDocument = doc;
            this.Update(doc);
        }

        void EventAggregatorSubscribe()
        {
            //_aggregator.GetEvent<StartDocumentEditEvent>().Subscribe(LoadDocumentForEdit);
            _aggregator.GetEvent<GetOrgansListFromBaseEvent>().Subscribe(GetOrgans);
            _aggregator.GetEvent<GetActTypesListFromBaseEvent>().Subscribe(GetActTypes);
            _aggregator.GetEvent<RecognizeCompliteEvent>().Subscribe(GetTextFromRecognizeService);
        }
        #region Комманды
        public DelegateCommand SaveCardCommand { get; set; }
        public DelegateCommand OpenPdfViewerCommand { get; set; }
        private void CommandInitialization()
        {
            SaveCardCommand = new DelegateCommand(SaveCard);
            OpenPdfViewerCommand = new DelegateCommand(openPdf);
        }
        private void openPdf()
        {
            _aggregator.GetEvent<PdfViewerIsOpenEvent>().Publish();
        }
        #endregion
        #region ICardEditorInterface realization 
        /// <summary>
        /// Очстка нужных параметров
        /// </summary>
        void ClearFieldsOnStart()
        {
            if (errorsDictionary != null)
            {
                errorsDictionary.Clear();
            }
            RequsitsCount = 0;
            CriticalErrorsCount = 0;
            ErrorsProgressBarValue = 0;
        }

        void Update(Document editor)
        {
            ClearFieldsOnStart();
            this.SourceGuid = editor.SourceGuid;
            this.HeaderGuid = editor.HeaderGuid;
            this.DocGuid = editor.DocGuid;
            if (editor.ChangedNumber == null)
            {
                if (ChangedNumber != editor.DocumentNumber)
                    ChangedNumber = editor.DocumentNumber;
            }
            else
            {
                ChangedNumber = editor.ChangedNumber;
            }

            this.DocumentNumber = editor.DocumentNumber;

            if (editor.OrganName == "Минюст России")
            {
                OrganName = null;
            }
            else
            {
                OrganName = editor.OrganName;
            }
            this.SignDate = editor.SignDate;
            this.ActType = editor.ActType;
            this.DocumentText = editor.DocumentText;
            this.MJDate = editor.MJDate;
            this.MJNumber = editor.MJNumber;
            this.Comments = editor.Comments;
            this.ComputerName = editor.ComputerName;
            this.DefferedOpen = editor.DefferedOpen;
            this.Status = editor.Status;
        }


        public DocumentStatus Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                {
                    this._Status = value;
                    OnPropertyChanged();
                }
            }
        }
        private DocumentStatus _Status { get; set; }


        private string _OrganName { get; set; }
        public string OrganName
        {
            get
            {
                return this._OrganName;
            }
            set
            {
                {
                    this._OrganName = value;
                    this.OnPropertyChanged();
                }
                Validation();
            }
        }

        private string _ActType { get; set; }
        public string ActType
        {
            get
            {
                return this._ActType;
            }
            set
            {
                {
                    this._ActType = value;
                    this.OnPropertyChanged();
                }
                Validation();
            }
        }

        private string _DocumentNumber { get; set; }
        public string DocumentNumber
        {
            get
            {
                return this._DocumentNumber;
            }
            set
            {
                {
                    this._DocumentNumber = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private DateTime? _SignDate { get; set; }
        public DateTime? SignDate
        {
            get
            {
                return this._SignDate;
            }
            set
            {
                {
                    this._SignDate = value;
                    this.OnPropertyChanged();
                }
                Validation();
            }
        }

        private DateTime? _DefferedOpen { get; set; }
        public DateTime? DefferedOpen
        {
            get
            {
                return this._DefferedOpen;
            }
            set
            {
                {
                    this._DefferedOpen = value;
                    this.OnPropertyChanged();
                }
                Validation();
            }
        }

        private string _DocumentText { get; set; }
        public string DocumentText
        {
            get
            {
                return this._DocumentText;
            }
            set
            {
                {
                    this._DocumentText = value;
                    this.OnPropertyChanged();

                }
                Validation();
            }
        }

        private string _Comments { get; set; }
        public string Comments
        {
            get
            {
                return this._Comments;
            }
            set
            {
                {
                    this._Comments = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Guid _DocGuid { get; set; }
        public Guid DocGuid
        {
            get
            {
                return this._DocGuid;
            }
            set
            {
                {
                    this._DocGuid = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Guid _HeaderGuid { get; set; }
        public Guid HeaderGuid
        {
            get
            {
                return this._HeaderGuid;
            }
            set
            {
                {
                    this._HeaderGuid = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Guid _SourceGuid { get; set; }
        public Guid SourceGuid
        {
            get
            {
                return this._SourceGuid;
            }
            set
            {
                {
                    IsMJDocument = false;
                    this._SourceGuid = value;
                    this.OnPropertyChanged();
                    RequsitsCount = ALLREQUSITSCOUNT;
                    if (value == Guid.Parse(Helpers.SourceGuidOrgansNames.Минюст))
                    {
                        IsMJDocument = true;
                        RequsitsCount = MJREQUSITSCOUNT;
                    }
                    if (value == Guid.Parse(Helpers.SourceGuidOrgansNames.МИД))
                    {
                        RequsitsCount = MIDREQUSITSCOUNT;
                    }
                }
            }
        }

        private string _ChangedNumber { get; set; }
        public string ChangedNumber
        {
            get
            {
                return this._ChangedNumber;
            }
            set
            {
                {
                    this._ChangedNumber = value;
                    this.OnPropertyChanged();

                }
                Validation();
            }
        }

        private DateTime? _MJDate { get; set; }
        public DateTime? MJDate
        {
            get
            {
                return this._MJDate;
            }
            set
            {
                {
                    this._MJDate = value;
                    this.OnPropertyChanged();
                }
                Validation();
            }
        }

        private string _MJNumber { get; set; }
        public string MJNumber
        {
            get
            {
                return this._MJNumber;
            }
            set
            {
                {
                    this._MJNumber = value;
                    this.OnPropertyChanged();

                }
                Validation();
            }
        }

        private string _ComputerName { get; set; }
        public string ComputerName
        {
            get
            {
                return this._ComputerName;
            }
            set
            {
                {
                    this._ComputerName = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private DateTime? _CardEditDateTime { get; set; }
        public DateTime? CardEditDateTime
        {
            get
            {
                return this._CardEditDateTime;
            }
            set
            {
                {
                    this._CardEditDateTime = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _RequisiteHash { get; set; }
        public string RequisiteHash
        {
            get
            {
                return this._RequisiteHash;
            }
            set
            {
                {
                    this._RequisiteHash = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsEdit { get; set; }
        public bool IsEdit
        {
            get
            {
                return this._IsEdit;
            }
            set
            {
                {
                    this._IsEdit = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Байндинги

        private bool _SettingsFlyoutIsOpen { get; set; }
        public bool SettingsFlyoutIsOpen
        {
            get
            {
                return this._SettingsFlyoutIsOpen;
            }
            set
            {
                if (this._SettingsFlyoutIsOpen != value)
                {
                    this._SettingsFlyoutIsOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }

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

        private bool _CommentIsOpen { get; set; }
        public bool CommentIsOpen
        {
            get
            {
                return this._CommentIsOpen;
            }
            set
            {
                if (this._CommentIsOpen != value)
                {
                    this._CommentIsOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }
        #endregion

        private void SaveCard()
        {

            CardEditDateTime = DateTime.Now;
            ComputerName = Environment.MachineName;
            EditDocument.Update(this);
            EditDocument.IsEdit = false;
            _aggregator.GetEvent<SaveCardEditorEvent>().Publish(EditDocument);
            _aggregator.GetEvent<DocumentEditorWindowIsOpenEvent>().Publish();
            //_aggregator.GetEvent<UpdateNonBaseStateEvent>().Publish(EditDocument);
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
                case RecognitionTypeEnum.Подписан:
                    {
                        DateTime dat = DateTime.MinValue;
                        string data = s.RecognizedString.Replace(" ", string.Empty);
                        string[] dataFormats = new string[] {
                            "dd.MM.yyyy",
                            "ddMMMMyyyy"
                        };
                        IFormatProvider provider = System.Globalization.CultureInfo.CurrentCulture;
                        DateTime.TryParseExact(data, dataFormats, provider, System.Globalization.DateTimeStyles.None, out dat);
                        if (dat != DateTime.MinValue)
                        {
                            this.SignDate = dat;
                        }

                        break;
                    }
            }
        }

        #region Органы и типы документов

        public void GetOrgans(List<string> s)
        {
            try
            {
                var list = s.Select(item => new TextInlineSelection(item, string.Empty)).ToList();
                if (OrganName != null)
                {
                    TextInlineSelection newitem = new TextInlineSelection(OrganName, string.Empty);
                    if (list.IndexOf(newitem) == -1)
                    {
                        list.Insert(0, newitem);
                    }
                }
                OrganList = list;
                BindingOperations.EnableCollectionSynchronization(OrganList, _lock);


            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        public void GetActTypes(List<string> s)
        {
            try
            {
                var list = s.Select(item => new TextInlineSelection(item, string.Empty)).ToList();
                if (ActType != null)
                {
                    TextInlineSelection newitem = new TextInlineSelection(ActType, string.Empty);
                    if (list.IndexOf(newitem) == -1)
                    {
                        list.Insert(0, newitem);
                    }
                }
                TypeList = list;
                BindingOperations.EnableCollectionSynchronization(OrganList, _lock);


            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private List<TextInlineSelection> _OrganList { get; set; }
        public List<TextInlineSelection> OrganList
        {
            get
            {
                return this._OrganList;
            }
            set
            {
                if (this.OrganList != value)
                {
                    this._OrganList = value;
                    this.OnPropertyChanged("OrganList");
                }
            }
        }
        private List<TextInlineSelection> _TypeList { get; set; }
        public List<TextInlineSelection> TypeList
        {
            get
            {
                return this._TypeList;
            }
            set
            {
                if (this.TypeList != value)
                {
                    this._TypeList = value;
                    this.OnPropertyChanged("TypeList");
                }
            }
        }

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
        private int _CriticalErrorsCount { get; set; }
        public int CriticalErrorsCount
        {
            get
            {
                return this._CriticalErrorsCount;
            }
            set
            {
                if (this.CriticalErrorsCount != value)
                {
                    this._CriticalErrorsCount = value;
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

        private bool _HaveCriticalErrors { get; set; }
        public bool HaveCriticalErrors
        {
            get
            {
                return this._HaveCriticalErrors;
            }
            set
            {
                if (this.HaveCriticalErrors != value)
                {
                    this._HaveCriticalErrors = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private async void Validation([CallerMemberName]string propertyName = "")
        {
            //Количество необходимых реквизитов для заполнения
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    switch (propertyName)
                    {
                        default:
                            {
                                break;
                            }
                        case "OrganName":
                            {
                                if (string.IsNullOrEmpty(OrganName))
                                {
                                    AddError(propertyName, Errors.ORGAN_ERROR);
                                }
                                else
                                {
                                    RemoveError(propertyName, Errors.ORGAN_ERROR);
                                }
                                break;
                            }
                        case "ActType":
                            {
                                if (string.IsNullOrEmpty(ActType))
                                {
                                    AddError(propertyName, Errors.ACTTYPE_ERROR);
                                }
                                else
                                {
                                    RemoveError(propertyName, Errors.ACTTYPE_ERROR);
                                }
                                break;
                            }
                        case "ChangedNumber":
                            {
                                if (SourceGuid == Guid.Parse(Helpers.SourceGuidOrgansNames.МИД))
                                {
                                    if (!string.IsNullOrEmpty(ChangedNumber))
                                    {
                                        AddError(propertyName, Errors.ACTNUMBER_WARNING);
                                        RemoveError(propertyName, Errors.MIDNUMBERNOTALLOWED_INFO);
                                    }
                                    else
                                    {
                                        AddError(propertyName, Errors.MIDNUMBERNOTALLOWED_INFO);
                                        RemoveError(propertyName, Errors.ACTNUMBER_WARNING);
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(ChangedNumber))
                                    {
                                        AddError(propertyName, Errors.ACTNUMBER_ERROR);
                                    }
                                    else
                                    {
                                        RemoveError(propertyName, Errors.ACTNUMBER_ERROR);
                                    }
                                }
                                break;
                            }

                        case "SignDate":
                            {
                                if (!SignDate.HasValue)
                                {
                                    AddError(propertyName, Errors.SIGNDATE_ERROR);
                                }
                                else
                                {
                                    RemoveError(propertyName, Errors.SIGNDATE_ERROR);
                                    if (SignDate.Value.Date > DateTime.Now.Date)
                                    {
                                        AddError(propertyName, Errors.SIGNDATEFUTURE_ERROR);
                                    }
                                    else
                                    {
                                        RemoveError(propertyName, Errors.SIGNDATEFUTURE_ERROR);
                                    }
                                }
                                break;
                            }

                        case "MJDate":
                            {
                                if (SourceGuid == Guid.Parse(Helpers.SourceGuidOrgansNames.Минюст))
                                {
                                    if (!MJDate.HasValue)
                                    {
                                        AddError(propertyName, Errors.MJREGDATE_ERROR);
                                    }
                                    else
                                    {
                                        RemoveError(propertyName, Errors.MJREGDATE_ERROR);
                                        if (MJDate.Value.Date > DateTime.Now.Date)
                                        {
                                            AddError(propertyName, Errors.MJREGDATEFUTURE_ERROR);
                                        }
                                        else
                                        {
                                            RemoveError(propertyName, Errors.MJREGDATEFUTURE_ERROR);
                                        }
                                    }
                                }
                                else
                                {
                                    if (MJDate.HasValue)
                                    {
                                        AddError(propertyName, Errors.MJREGNUMBERNOTALLOWED_ERROR);
                                    }
                                    else
                                    {
                                        RemoveError(propertyName, Errors.MJREGNUMBERNOTALLOWED_ERROR);
                                    }
                                }

                                break;
                            }

                        case "MJNumber":
                            {
                                if (SourceGuid == Guid.Parse(Helpers.SourceGuidOrgansNames.Минюст))
                                {
                                    if (string.IsNullOrEmpty(MJNumber))
                                    {
                                        AddError(propertyName, Errors.MJREGNUMBER_ERROR);
                                    }
                                    else
                                    {
                                        RemoveError(propertyName, Errors.MJREGNUMBER_ERROR);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(MJNumber))
                                    {
                                        AddError(propertyName, Errors.MJREGNUMBERNOTALLOWED_ERROR);
                                    }
                                    else
                                    {
                                        RemoveError(propertyName, Errors.MJREGNUMBERNOTALLOWED_ERROR);

                                    }
                                }
                                break;
                            }
                        case "DocumentText":
                            {
                                if (SourceGuid == Guid.Parse(Helpers.SourceGuidOrgansNames.Минюст))
                                {
                                    AddError(propertyName, Errors.MJNAME_INFO);
                                }
                                else
                                {
                                    RemoveError(propertyName, Errors.MJNAME_INFO);
                                }

                                if (SourceGuid == Guid.Parse(Helpers.SourceGuidOrgansNames.МИД))
                                {
                                    AddError(propertyName, Errors.MIDNAME_INFO);
                                }
                                else
                                {
                                    RemoveError(propertyName, Errors.MIDNAME_INFO);
                                }

                                if (SourceGuid == Guid.Parse(Helpers.SourceGuidOrgansNames.Президент))
                                {
                                    AddError(propertyName, Errors.ACTPREZ_INFO);
                                }
                                else
                                {
                                    RemoveError(propertyName, Errors.MIDNAME_INFO);
                                }
                                if (SourceGuid == Guid.Parse(Helpers.SourceGuidOrgansNames.КС))
                                {
                                    AddError(propertyName, Errors.KSACTNAME_INFO);
                                }
                                else
                                {
                                    RemoveError(propertyName, Errors.KSACTNAME_INFO);
                                }
                                break;
                            }
                    }
                    ErrorsProgressBarValue = RequsitsCount - CriticalErrorsCount;

                }
                catch (System.Exception ex)
                {
                    logger.Fatal(ex);
                }

            });
        }
        #endregion

        #region Модель валидации данных
        [NonSerialized]
        public ConcurrentDictionary<string, List<CustomErrorType>> errorsDictionary = new ConcurrentDictionary<string, List<CustomErrorType>>();
        public void AddError(string propertyName, CustomErrorType error)
        {
            if (errorsDictionary != null && !string.IsNullOrEmpty(propertyName) && error != null)
            {
                if (!errorsDictionary.ContainsKey(propertyName))
                {
                    errorsDictionary[propertyName] = new List<CustomErrorType>();
                }
                if (!errorsDictionary[propertyName].Contains(error))
                {
                    errorsDictionary[propertyName].Insert(0, error);
                    OnPropertyErrorsChanged(propertyName);
                }
            }
        }

        public void RemoveError(string propertyName, CustomErrorType error)
        {
            try
            {
                if (errorsDictionary != null && !string.IsNullOrEmpty(propertyName) && error != null)
                {
                    if (errorsDictionary.ContainsKey(propertyName) && errorsDictionary[propertyName].Contains(error))
                    {
                        errorsDictionary[propertyName].Remove(error);
                        if (errorsDictionary[propertyName].Count == 0)
                        {
                            List<CustomErrorType> ce = new List<CustomErrorType>();
                            errorsDictionary.TryRemove(propertyName, out ce);
                        }
                        OnPropertyErrorsChanged(propertyName);
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        #endregion

        #region INotifyDataErrorInfo
        [field: NonSerialized]
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void OnPropertyErrorsChanged([CallerMemberName]string propertyName = "")
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                getCriticalErrorsCount();
            }

        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            List<CustomErrorType> errors = new List<CustomErrorType>();
            if (propertyName != null)
            {
                errorsDictionary.TryGetValue(propertyName, out errors);
                return errors;
            }

            else
                return null;
        }

        public bool HasErrors
        {
            get
            {
                try
                {
                    if (errorsDictionary != null)
                    {
                        return errorsDictionary.Count > 0;
                    }
                    else return false;
                }
                catch { }
                return false;
            }
        }

        #region Мои методы для отсеивания ошибок      
        private void getCriticalErrorsCount()
        {
            try
            {
                int count = 0;
                foreach (var t in errorsDictionary)
                {
                    int c = t.Value.Where(d => d.MessageErrorType == ErrorType.ERROR).Count();
                    count += c;
                }
                CriticalErrorsCount = count;
            }
            catch { }
        }

        #endregion

        #endregion
    }
}
