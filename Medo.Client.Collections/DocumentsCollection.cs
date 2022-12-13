using Medo.Core.Collections;
using Medo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Prism.Commands;
using Medo.Core.Collections.Concurrent;
using NLog;
using Medo.Core.Enums;
using Medo.Client.Collections.Filtration;
using Medo.Core.Interfaces;
using System.Collections.ObjectModel;
using Medo.Core.Collections.Concurrent.Dictionary;

namespace Medo.Client.Collections
{
    public class DocumentsCollection : List<Document>, INotifyPropertyChanged
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region PropertyChanged realization
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public DocumentsCollection()
        {
            Default();
        }

        private void Default()
        {
            InitializeFilteringCommands();
            ActiveFilters = new FiltrationRules(ActiveFilterEnum.NotOpublic, new Core.Comparators.DeliveryTimeComparer(ListSortDirection.Descending));
            Contacts = new ObservableCollection<ContactsModel>();
            DocumentsInUpdateProcess = new ConcurrentObservableDictionary<Guid, bool>();
        }
        public readonly Dictionary<Guid, string> ActNameRollBackDictionary = new Dictionary<Guid, string>();

        private ObservableCollection<ContactsModel> _Contacts { get; set; }
        /// <summary>
        /// Коллекция контактов принявших органов (заполняется только по запросу)
        /// </summary>
        public ObservableCollection<ContactsModel> Contacts
        {
            get
            {
                return this._Contacts;
            }
            set
            {
                if (this.Contacts != value)
                {
                    this._Contacts = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ConcurrentObservableDictionary<Guid, bool> _DocumentsInUpdateProcess { get; set; }
        /// <summary>
        /// Коллекция контактов принявших органов (заполняется только по запросу)
        /// </summary>
        public ConcurrentObservableDictionary<Guid, bool> DocumentsInUpdateProcess
        {
            get
            {
                return this._DocumentsInUpdateProcess;
            }
            set
            {
                if (this.DocumentsInUpdateProcess != value)
                {
                    this._DocumentsInUpdateProcess = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private FiltrationRules _ActiveFilters { get; set; }
        /// <summary>
        /// Коллекция правил фильтрации,  активных фильтров  и отфильтрованных документов
        /// </summary>
        public FiltrationRules ActiveFilters
        {
            get
            {
                return this._ActiveFilters;
            }
            set
            {
                if (this.ActiveFilters != value)
                {
                    this._ActiveFilters = value;
                    this.OnPropertyChanged();
                }
            }
        }
     

        #region Фильтрация

        /// <summary>
        /// Делаем все добавленные правила фильтрации активными и запускаем фильтрацию
        /// </summary>
        public void FullFiltering()
        {
            ActiveFilters.SourceCollectionFiltration(this);
        }
        public void FilterOneDocument(Document doc)
        {
            ActiveFilters.OneElementFiltration(doc);
        }
        private List<Document> testDocuments = new List<Document>();

        public DelegateCommand StartFilteringCommand { get; set; }
        void InitializeFilteringCommands()
        {
            StartFilteringCommand = new DelegateCommand(FullFiltering);
        }

        #endregion

        #region Переменные
        /// <summary>
        /// Количество документов в основной коллекции
        /// </summary>
        public double ItemsCount
        {
            get { return this.Count(); }
        }
        /// <summary>
        /// Количество документов в основном списке с флагом IsSelected
        /// </summary>
        public double SelectedItemsCount
        {
            get
            {
                double count = 0;
                for (int i = 0; i < this.Count(); i++)
                {
                    if (this[i].IsSelected)
                        count++;
                }
                return count;
            }
        }
        #endregion

        /// <summary>
        /// Обновление или добавление документа в коллекцию
        /// </summary>
        /// <param name="doc"></param>
        public Document AddOrUpdate(Document doc)
        {
            Document item = null;
            for (int i = 0; i < this.Count(); i++)
            {
                if (this[i].HeaderGuid == doc.HeaderGuid)
                    item = this[i];
            }
            if (item == null)
            {
                this.Add(doc);
            }
            else
            {
                item.Update(doc);
            }
            if (DocumentsInUpdateProcess.ContainsKey(doc.HeaderGuid))
            {
                DocumentsInUpdateProcess.Remove(doc.HeaderGuid);
            }
            FilterOneDocument(doc);
            GetItemsCount();
            return item;
        }
        /// <summary>
        /// Обновление флага IsSelected объекта Document
        /// </summary>
        /// <param name="hguid"></param>
        public Document AddOrUpdate(Guid hguid)
        {
            Document item = null;
            for (int i = 0; i < this.Count(); i++)
            {
                if (this[i].HeaderGuid == hguid)
                    item = this[i];
            }
            item.IsSelected = !item.IsSelected;
            FilterOneDocument(item);
            GetItemsCount();
            return item;
        }
        public Document AddOrUpdate(ICardEditorInterface cardEdit)
        {
            Document item = null;
            for (int i = 0; i < this.Count(); i++)
            {
                if (this[i].HeaderGuid == cardEdit.HeaderGuid)
                    item = this[i];
            }
            if (!ActNameRollBackDictionary.ContainsKey(item.HeaderGuid))
                ActNameRollBackDictionary.Add(item.HeaderGuid, item.DocumentText);
            item.Update(cardEdit);
            if (!DocumentsInUpdateProcess.ContainsKey(cardEdit.HeaderGuid))
                DocumentsInUpdateProcess.Add(cardEdit.HeaderGuid, true);
            FilterOneDocument(item);
            GetItemsCount();
            return item;
        }
        public Document AddOrUpdate(IBaseStatesInterface states)
        {
            Document item = null;
            for (int i = 0; i < this.Count(); i++)
            {
                if (this[i].HeaderGuid == states.HeaderGuid)
                    item = this[i];
            }
            item.Update(states);
            if (!DocumentsInUpdateProcess.ContainsKey(states.HeaderGuid))
                DocumentsInUpdateProcess.Add(states.HeaderGuid, true);
            FilterOneDocument(item);
            GetItemsCount();
            return item;
        }
        public Document AddOrUpdate(ITextDocumentUpdater text)
        {
            Document item = null;
            for (int i = 0; i < this.Count(); i++)
            {
                if (this[i].HeaderGuid == text.HeaderGuid)
                    item = this[i];
            }
            if (!ActNameRollBackDictionary.ContainsKey(item.HeaderGuid))
                ActNameRollBackDictionary.Add(item.HeaderGuid, item.DocumentText);
            item.Update(text);
            if (!DocumentsInUpdateProcess.ContainsKey(text.HeaderGuid))
                DocumentsInUpdateProcess.Add(text.HeaderGuid, true);
            FilterOneDocument(item);
            GetItemsCount();
            return item;
        }
        public Document AddOrUpdate(INonBaseStatesInterface nbs)
        {
            Document item = null;
            for (int i = 0; i < this.Count(); i++)
            {
                if (this[i].HeaderGuid == nbs.HeaderGuid)
                    item = this[i];
            }
            item.Update(nbs);
            FilterOneDocument(item);
            GetItemsCount();
            return item;
        }
      
        private void GetItemsCount()
        {
            OnPropertyChanged("ItemsCount");
            OnPropertyChanged("SelectedItemsCount");
        }


        /// <summary>
        /// Добавление массива объектов в коллекцию
        /// </summary>
        /// <param name="docs"></param>
        public async void AddOrUpdateRange(IList<Document> docs)
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < docs.Count(); i++)
                {
                    Document item = null;
                    for (int d = 0; d < this.Count(); d++)
                    {
                        if (this[d].HeaderGuid == docs[i].HeaderGuid)
                            item = this[d];
                    }
                    if (item == null)
                    {
                        this.Add(docs[i]);
                    }
                    else
                    {
                        item.Update(docs[i]);
                    }

                    FilterOneDocument(docs[i]);
                    OnPropertyChanged("ItemsCount");
                }
                OnPropertyChanged("SelectedItemsCount");
            });
        }
    }
}
