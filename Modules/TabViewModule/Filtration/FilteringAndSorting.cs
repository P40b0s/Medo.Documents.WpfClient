using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Medo.Core.Collections;
using System.Windows.Controls;
using Medo.Core.Models;
using System.ComponentModel;
using Medo.Core.Enums;
using Prism.Commands;
using System.Threading;
using System.Windows;
using Prism.Interactivity.InteractionRequest;
using System.Collections.ObjectModel;
using Prism.Events;
using Medo.Core.EventsAggregator;
using System.Collections.Concurrent;
using Medo.Client.Collections.Filtration;

namespace Medo.Modules.TabViewModule.Filtration
{
    /// <summary>
    /// Фильтрация, сортировка коллекиций. Байндинги всех кнопок сортировки и фильтрации
    /// </summary>
    public class FilteringAndSorting : BaseModel
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        IEventAggregator eventAggregator;
        public FilteringAndSorting(IEventAggregator _eventAggregator)

        {
            eventAggregator = _eventAggregator;
            DeliveryTimeDescendingSortButtonPressed = true;
            InitializeSortingCommands();
            InitializeFilteringCommands();
            NotOpublicDocsIsVisible = true;
            InitializeEvents();
            Client.Collections.StaticCollections.MainCollection.ActiveFilters.DefaultFiltrationIsActive += ActiveFilters_DefaultFiltrationIsActive;
        }

       

        private void ActiveFilters_DefaultFiltrationIsActive(bool obj)
        {
            NotOpublicDocsIsVisible = true;
            Client.Collections.StaticCollections.MainCollection.FullFiltering();
        }

        private void InitializeEvents()
        {
            eventAggregator.GetEvent<StartFilteringEvent>().Subscribe(Client.Collections.StaticCollections.MainCollection.FullFiltering);
            eventAggregator.GetEvent<CompleteUpdateMainCollectionEvent>().Subscribe(()=> LoadOrgansAndActTypesForFiltering());
            //eventAggregator.GetEvent<GetDoublesDictionaryEvent>().Subscribe(d=> );
        }

    


        #region Привязка поисковых полей    
        private ObservableCollection<string> _OrgansList { get; set; }
        public ObservableCollection<string> OrgansList
        {
            get
            {
                return this._OrgansList;
            }
            set
            {
                if (this.OrgansList != value)
                {
                    this._OrgansList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<string> _ActTypeList { get; set; }
        public ObservableCollection<string> ActTypeList
        {
            get
            {
                return this._ActTypeList;
            }
            set
            {
                if (this.ActTypeList != value)
                {
                    this._ActTypeList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        protected void LoadOrgansAndActTypesForFiltering()
        {
            OrgansList = new ObservableCollection<string>(Client.Collections.StaticCollections.MainCollection.GroupBy(category => category.OrganName).Select(s => s.Key).OrderBy(s => s));
            ActTypeList = new ObservableCollection<string>(Client.Collections.StaticCollections.MainCollection.GroupBy(category => category.ActType).Select(s => s.Key).OrderBy(s => s));
        }

        private string _OrganSelectedItem { get; set; }
        public string OrganSelectedItem
        {
            get
            {
                return this._OrganSelectedItem;
            }
            set
            {
                if (this.OrganSelectedItem != value)
                {
                    this._OrganSelectedItem = value;
                    this.OnPropertyChanged();
                    if (OrganSelectedItem == null)
                    {
                        ActTypeList = new ObservableCollection<string>(Client.Collections.StaticCollections.MainCollection.GroupBy(category => category.ActType).Select(s => s.Key).OrderBy(s => s));
                        RemoveFilter(ActiveFilterEnum.Organ);
                    }
                    else
                    {
                        ActTypeList = new ObservableCollection<string>(Client.Collections.StaticCollections.MainCollection.Where(s => s.OrganName == OrganSelectedItem).GroupBy(category => category.ActType).Select(s => s.Key).OrderBy(s => s));
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.AddOrChangeFilter(new FiltrationRule { FilterType = ActiveFilterEnum.Organ, SearchingObject = value });
                        ChangedDocsIsVisible = false;
                        DeletedDocsIsVisible = false;
                        NotOpublicDocsIsVisible = false;
                    }
                }
            }
        }

        private string _ActSelectedItem { get; set; }
        public string ActSelectedItem
        {
            get
            {
                return this._ActSelectedItem;
            }
            set
            {
                if (this.ActSelectedItem != value)
                {
                    this._ActSelectedItem = value;
                    this.OnPropertyChanged();
                    if (value == null)
                        RemoveFilter(ActiveFilterEnum.Type);
                    else
                    {
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.AddOrChangeFilter(new FiltrationRule { FilterType = ActiveFilterEnum.Type, SearchingObject = value });
                        ChangedDocsIsVisible = false;
                        DeletedDocsIsVisible = false;
                        NotOpublicDocsIsVisible = false;
                    }
                }
            }
        }


        private string _Number { get; set; }
        public string Number
        {
            get
            {
                return this._Number;
            }
            set
            {
                if (this.Number != value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        this._Number = null;
                        RemoveFilter(ActiveFilterEnum.Number);
                    }
                    else
                    {
                        this._Number = value;
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.AddOrChangeFilter(new FiltrationRule { FilterType = ActiveFilterEnum.Number, SearchingObject = value });
                        ChangedDocsIsVisible = false;
                        DeletedDocsIsVisible = false;
                        NotOpublicDocsIsVisible = false;
                    }
                    this.OnPropertyChanged();
                }
            }
        }

        private DateTime? _SignedDate { get; set; }
        public DateTime? SignedDate
        {
            get
            {
                return this._SignedDate;
            }
            set
            {
                if (this.SignedDate != value)
                {
                    this._SignedDate = value;
                    this.OnPropertyChanged();
                    if (!value.HasValue)
                        RemoveFilter(ActiveFilterEnum.Signed);
                    else
                    {
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.AddOrChangeFilter(new FiltrationRule { FilterType = ActiveFilterEnum.Signed, SearchingObject = value });
                        ChangedDocsIsVisible = false;
                        DeletedDocsIsVisible = false;
                        NotOpublicDocsIsVisible = false;
                    }
                        

                }
            }
        }

        private DateTime? _DeliveryDateTime { get; set; }
        public DateTime? DeliveryDateTime
        {
            get
            {
                return this._DeliveryDateTime;
            }
            set
            {
                if (this.DeliveryDateTime != value)
                {
                    this._DeliveryDateTime = value;
                    this.OnPropertyChanged();
                    if (!value.HasValue)
                        RemoveFilter(ActiveFilterEnum.DeliveryTime);
                    else
                    {
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.AddOrChangeFilter(new FiltrationRule { FilterType = ActiveFilterEnum.DeliveryTime, SearchingObject = value });
                        ChangedDocsIsVisible = false;
                        DeletedDocsIsVisible = false;
                        NotOpublicDocsIsVisible = false;
                    }
                       
                }
            }
        }

        private DateTime? _PublicDateTime { get; set; }
        public DateTime? PublicDateTime
        {
            get
            {
                return this._PublicDateTime;
            }
            set
            {
                if (this.PublicDateTime != value)
                {
                    this._PublicDateTime = value;
                    this.OnPropertyChanged();
                    if (!value.HasValue)
                        RemoveFilter(ActiveFilterEnum.EODate);
                    else
                    {
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.AddOrChangeFilter(new FiltrationRule { FilterType = ActiveFilterEnum.EODate, SearchingObject = value });
                        ChangedDocsIsVisible = false;
                        DeletedDocsIsVisible = false;
                        NotOpublicDocsIsVisible = false;
                    }
                       
                }
            }
        }

        private string _DigitalEoNumber { get; set; }
        public string DigitalEoNumber
        {
            get
            {
                return this._DigitalEoNumber;
            }
            set
            {
                if (this.DigitalEoNumber != value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        this._DigitalEoNumber = null;
                        RemoveFilter(ActiveFilterEnum.EONumber);
                    }
                    else
                    {
                        this._DigitalEoNumber = value;
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.AddOrChangeFilter(new FiltrationRule { FilterType = ActiveFilterEnum.EONumber, SearchingObject = value });
                        ChangedDocsIsVisible = false;
                        DeletedDocsIsVisible = false;
                        NotOpublicDocsIsVisible = false;
                    }
                    this.OnPropertyChanged();
                }
            }
        }

        #region Общие фильтры (без конкретных значений поиска)
        private bool _ChangedDocsIsVisible { get; set; }
        public bool ChangedDocsIsVisible
        {
            get
            {
                return this._ChangedDocsIsVisible;
            }
            set
            {
                if (this.ChangedDocsIsVisible != value)
                {
                    this._ChangedDocsIsVisible = value;
                    this.OnPropertyChanged();
                    if (value)
                    {
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.AddOrChangeFilter(new FiltrationRule { FilterType = ActiveFilterEnum.Changed });
                        DeletedDocsIsVisible = false;
                        NotOpublicDocsIsVisible = false;
                    }
                    else
                    {
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(ActiveFilterEnum.Changed);
                    }
                }
            }
        }
        private bool _DeletedDocsIsVisible { get; set; }
        public bool DeletedDocsIsVisible
        {
            get
            {
                return this._DeletedDocsIsVisible;
            }
            set
            {
                if (this.DeletedDocsIsVisible != value)
                {
                    this._DeletedDocsIsVisible = value;
                    this.OnPropertyChanged();
                    if (value)
                    {
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.AddOrChangeFilter(new FiltrationRule { FilterType = ActiveFilterEnum.Deleted });
                        ChangedDocsIsVisible = false;
                        NotOpublicDocsIsVisible = false;
                    }
                    else
                    {
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(ActiveFilterEnum.Deleted);
                    }
                }
            }
        }

        private bool _NotOpublicDocsIsVisible { get; set; }
        public bool NotOpublicDocsIsVisible
        {
            get
            {
                return this._NotOpublicDocsIsVisible;
            }
            set
            {
                if (this.NotOpublicDocsIsVisible != value)
                {
                    this._NotOpublicDocsIsVisible = value;
                    this.OnPropertyChanged();
                    if (value)
                    {
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.AddOrChangeFilter(new FiltrationRule { FilterType = ActiveFilterEnum.NotOpublic, IsDefaultRule = true, RuleIsOn = true });
                        ChangedDocsIsVisible = false;
                        DeletedDocsIsVisible = false;
                    }
                    else
                    {
                        Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(ActiveFilterEnum.NotOpublic);
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Удаление фильтров
        /// <summary>
        /// Команда удаления фильтра из списка фильтрации
        /// </summary>
        public DelegateCommand<object> RemoveFilterCommand { get; set; }
        void InitializeFilteringCommands()
        {
            RemoveFilterCommand = new DelegateCommand<object>(RemoveFilter);
        }
        void RemoveFilter(object filtername)
        {
            try
            {
                var name = (ActiveFilterEnum)filtername;
                switch (name)
                {
                    case ActiveFilterEnum.Signed:
                        {
                            SignedDate = null;
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(name);
                            break;
                        }
                    case ActiveFilterEnum.DeliveryTime:
                        {
                            DeliveryDateTime = null;
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(name);
                            break;
                        }
                    case ActiveFilterEnum.EODate:
                        {
                            PublicDateTime = null;
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(name);
                            break;
                        }
                    case ActiveFilterEnum.EONumber:
                        {
                            DigitalEoNumber = null;
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(name);
                            break;
                        }
                    case ActiveFilterEnum.Number:
                        {
                            Number = null;
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(name);
                            break;
                        }
                    case ActiveFilterEnum.Organ:
                        {
                            OrganSelectedItem = null;
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(name);
                            break;
                        }
                    case ActiveFilterEnum.Type:
                        {
                            ActSelectedItem = null;
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(name);
                            break;
                        }

                    case ActiveFilterEnum.NotOpublic:
                        {
                            NotOpublicDocsIsVisible = false;
                            break;
                        }
                    case ActiveFilterEnum.Deleted:
                        {
                            DeletedDocsIsVisible = false;
                            break;
                        }
                    case ActiveFilterEnum.Changed:
                        {
                            ChangedDocsIsVisible = false;
                            break;
                        }
                    case ActiveFilterEnum.Specialized:
                        {
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.RemoveFilter(name);
                            break;
                        }

                }
                Client.Collections.StaticCollections.MainCollection.StartFilteringCommand.RaiseCanExecuteChanged();
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        #endregion


        #region Команды сортировки

        public DelegateCommand<object> SortingDocumentsCommand { get; set; }
        public DelegateCommand DocumentNumberAscendingSortCommand { get; set; }
        public DelegateCommand DocumentNumberDescendingSortCommand { get; set; }
        public DelegateCommand OrganNameDescendingSortCommand { get; set; }
        public DelegateCommand EoNumberDescendingSortCommand { get; set; }
        public DelegateCommand ActTypeDescendingSortCommand { get; set; }
        public DelegateCommand DeliveryTimeAscendingSortCommand { get; set; }
        public DelegateCommand DeliveryTimeDescendingSortCommand { get; set; }
        public DelegateCommand SignDateAscendingSortCommand { get; set; }
        public DelegateCommand SignDateDescendingSortCommand { get; set; }
        public DelegateCommand PublDatePortalAscendingSortCommand { get; set; }
        public DelegateCommand PublDatePortalDescendingSortCommand { get; set; }

        void InitializeSortingCommands()
        {
            DocumentNumberAscendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.DocumentNumber, ListSortDirection.Ascending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

            DocumentNumberDescendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.DocumentNumber, ListSortDirection.Descending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

            OrganNameDescendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.OrganName, ListSortDirection.Descending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

            EoNumberDescendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.EoNumber, ListSortDirection.Descending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

            ActTypeDescendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.ActType, ListSortDirection.Descending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

            DeliveryTimeAscendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.DeliveryTime, ListSortDirection.Ascending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

            DeliveryTimeDescendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.DeliveryTime, ListSortDirection.Descending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

            SignDateAscendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.SignDate, ListSortDirection.Ascending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

            SignDateDescendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.SignDate, ListSortDirection.Descending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

            PublDatePortalAscendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.PublDatePortal, ListSortDirection.Ascending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

            PublDatePortalDescendingSortCommand = new DelegateCommand(
                () => setSortDescription(SortingParam.PublDatePortal, ListSortDirection.Descending),
                () => Client.Collections.StaticCollections.CommandsIsActive);

        }

        #endregion

        #region Сортировка коллекции

        #region раскрашивание нажатых кнопок сортировки 
        private bool _OrganAscendingSortButtonPressed { get; set; }
        public bool OrganAscendingSortButtonPressed
        {
            get
            {
                return this._OrganAscendingSortButtonPressed;
            }
            set
            {
                if (this.OrganAscendingSortButtonPressed != value)
                {
                    this._OrganAscendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _OrganDescendingSortButtonPressed { get; set; }
        public bool OrganDescendingSortButtonPressed
        {
            get
            {
                return this._OrganDescendingSortButtonPressed;
            }
            set
            {
                if (this.OrganDescendingSortButtonPressed != value)
                {
                    this._OrganDescendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private bool _ActTypeDescendingSortButtonPressed { get; set; }
        public bool ActTypeDescendingSortButtonPressed
        {
            get
            {
                return this._ActTypeDescendingSortButtonPressed;
            }
            set
            {
                if (this.ActTypeDescendingSortButtonPressed != value)
                {
                    this._ActTypeDescendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _ActTypeAscendingSortButtonPressed { get; set; }
        public bool ActTypeAscendingSortButtonPressed
        {
            get
            {
                return this._ActTypeAscendingSortButtonPressed;
            }
            set
            {
                if (this.ActTypeAscendingSortButtonPressed != value)
                {
                    this._ActTypeAscendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _NumberAscendingSortButtonPressed { get; set; }
        public bool NumberAscendingSortButtonPressed
        {
            get
            {
                return this._NumberAscendingSortButtonPressed;
            }
            set
            {
                if (this.NumberAscendingSortButtonPressed != value)
                {
                    this._NumberAscendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _NumberDescendingSortButtonPressed { get; set; }
        public bool NumberDescendingSortButtonPressed
        {
            get
            {
                return this._NumberDescendingSortButtonPressed;
            }
            set
            {
                if (this.NumberDescendingSortButtonPressed != value)
                {
                    this._NumberDescendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _SignDateDescendingSortButtonPressed { get; set; }
        public bool SignDateDescendingSortButtonPressed
        {
            get
            {
                return this._SignDateDescendingSortButtonPressed;
            }
            set
            {
                if (this.SignDateDescendingSortButtonPressed != value)
                {
                    this._SignDateDescendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _SignDateAscendingSortButtonPressed { get; set; }
        public bool SignDateAscendingSortButtonPressed
        {
            get
            {
                return this._SignDateAscendingSortButtonPressed;
            }
            set
            {
                if (this.SignDateAscendingSortButtonPressed != value)
                {
                    this._SignDateAscendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _DeliveryTimeAscendingSortButtonPressed { get; set; }
        public bool DeliveryTimeAscendingSortButtonPressed
        {
            get
            {
                return this._DeliveryTimeAscendingSortButtonPressed;
            }
            set
            {
                if (this.DeliveryTimeAscendingSortButtonPressed != value)
                {
                    this._DeliveryTimeAscendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private bool _DeliveryTimeDescendingSortButtonPressed { get; set; }
        public bool DeliveryTimeDescendingSortButtonPressed
        {
            get
            {
                return this._DeliveryTimeDescendingSortButtonPressed;
            }
            set
            {
                if (this.DeliveryTimeDescendingSortButtonPressed != value)
                {
                    this._DeliveryTimeDescendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _EoNumberDescendingSortButtonPressed { get; set; }
        public bool EoNumberDescendingSortButtonPressed
        {
            get
            {
                return this._EoNumberDescendingSortButtonPressed;
            }
            set
            {
                if (this.EoNumberDescendingSortButtonPressed != value)
                {
                    this._EoNumberDescendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _EoNumberAscendingSortButtonPressed { get; set; }
        public bool EoNumberAscendingSortButtonPressed
        {
            get
            {
                return this._EoNumberAscendingSortButtonPressed;
            }
            set
            {
                if (this.EoNumberAscendingSortButtonPressed != value)
                {
                    this._EoNumberAscendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _PublDateAscendingSortButtonPressed { get; set; }
        public bool PublDateAscendingSortButtonPressed
        {
            get
            {
                return this._PublDateAscendingSortButtonPressed;
            }
            set
            {
                if (this.PublDateAscendingSortButtonPressed != value)
                {
                    this._PublDateAscendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _PublDateDescendingSortButtonPressed { get; set; }
        public bool PublDateDescendingSortButtonPressed
        {
            get
            {
                return this._PublDateDescendingSortButtonPressed;
            }
            set
            {
                if (this.PublDateDescendingSortButtonPressed != value)
                {
                    this._PublDateDescendingSortButtonPressed = value;
                    this.OnPropertyChanged();

                }
            }
        }

        #endregion
        private void OffAllSortButton()
        {
            OrganAscendingSortButtonPressed = false;
            OrganDescendingSortButtonPressed = false;
            ActTypeAscendingSortButtonPressed = false;
            ActTypeDescendingSortButtonPressed = false;
            NumberAscendingSortButtonPressed = false;
            NumberDescendingSortButtonPressed = false;
            SignDateAscendingSortButtonPressed = false;
            SignDateDescendingSortButtonPressed = false;
            DeliveryTimeAscendingSortButtonPressed = false;
            DeliveryTimeDescendingSortButtonPressed = false;
            EoNumberAscendingSortButtonPressed = false;
            EoNumberDescendingSortButtonPressed = false;
            PublDateAscendingSortButtonPressed = false;
            PublDateDescendingSortButtonPressed = false;
        }
        public async void setSortDescription(SortingParam sortName, ListSortDirection sortDirection)
        {
            await Task.Factory.StartNew(() =>
            {
                switch (sortName)
                {
                    default:
                        {
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.DeliveryTimeComparer(ListSortDirection.Descending));
                            OffAllSortButton();
                            DeliveryTimeDescendingSortButtonPressed = true;
                            break;
                        }
                    case SortingParam.DeliveryTime:
                        {
                            OffAllSortButton();
                            if (sortDirection == ListSortDirection.Ascending)
                            {
                                DeliveryTimeAscendingSortButtonPressed = true;
                                Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.DeliveryTimeComparer(ListSortDirection.Ascending));
                            }
                            else
                            {
                                DeliveryTimeDescendingSortButtonPressed = true;
                                Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.DeliveryTimeComparer(ListSortDirection.Descending));
                            }
                            break;
                        }
                    case SortingParam.ActType:
                        {
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.ActTypeComparer());
                            OffAllSortButton();
                            ActTypeDescendingSortButtonPressed = true;
                            break;
                        }
                    case SortingParam.SignDate:
                        {
                            OffAllSortButton();
                            if (sortDirection == ListSortDirection.Ascending)
                            {
                                SignDateAscendingSortButtonPressed = true;
                                Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.SignDateComparer(ListSortDirection.Ascending));
                            }
                            else
                            {
                                SignDateDescendingSortButtonPressed = true;
                                Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.SignDateComparer(ListSortDirection.Descending));
                            }
                            break;
                        }
                    case SortingParam.PublDatePortal:
                        {
                            OffAllSortButton();
                            if (sortDirection == ListSortDirection.Ascending)
                            {
                                PublDateAscendingSortButtonPressed = true;
                                Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.PublicationDateComparer(ListSortDirection.Ascending));
                            }
                            else
                            {
                                PublDateDescendingSortButtonPressed = true;
                                Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.PublicationDateComparer(ListSortDirection.Descending));
                            }
                            break;
                        }
                    case SortingParam.EoNumber:
                        {
                            OffAllSortButton();
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.EoNumberComparer());
                            EoNumberDescendingSortButtonPressed = true;
                            break;
                        }

                    case SortingParam.OrganName:
                        {
                            OffAllSortButton();
                            Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.OrganComparer());
                            OrganDescendingSortButtonPressed = true;
                            break;
                        }
                    case SortingParam.DocumentNumber:
                        {

                            OffAllSortButton();
                            if (sortDirection == ListSortDirection.Ascending)
                            {
                                NumberAscendingSortButtonPressed = true;
                                Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.ActNumberComparer(ListSortDirection.Ascending));
                            }
                            else
                            {
                                NumberDescendingSortButtonPressed = true;
                                Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItems.Sort(new Comparators.ActNumberComparer(ListSortDirection.Descending));
                            }
                            break;
                        }
                }
            });

        }

        #endregion
    }


}
