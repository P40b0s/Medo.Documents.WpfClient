using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ContactsUpdater.Models
{
    public class MedoContactsModel : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public MedoContactsModel()
        {
            Contacts = new ObservableCollection<MedoContactsModel>();
            Organs = new Dictionary<Guid, string>();
        }

        public ObservableCollection<MedoContactsModel> Contacts { get; set; }
        public Dictionary<Guid, string> Organs { get; set; }
       
        private bool _CreateNewContactWindowIsOpen { get; set; }
        /// <summary>
        /// Время последнего обновления статуса
        /// </summary>
        public bool CreateNewContactWindowIsOpen
        {
            get
            {
                return this._CreateNewContactWindowIsOpen;
            }
            set
            {
                if (this.CreateNewContactWindowIsOpen != value)
                {
                    this._CreateNewContactWindowIsOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private MedoContactsModel _SelectedItem { get; set; }
        /// <summary>
        /// Время последнего обновления статуса
        /// </summary>
        public MedoContactsModel SelectedItem
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
                }
            }
        }

        private Guid _SourceGuid { get; set; }
        /// <summary>
        /// Идентификатор принявшего органа
        /// </summary>
        public Guid SourceGuid
        {
            get
            {
                return this._SourceGuid;
            }
            set
            {
                if (this.SourceGuid != value)
                {
                    this._SourceGuid = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private Guid _Id { get; set; }
        /// <summary>
        /// Идентификатор записи в базе данных
        /// </summary>
        public Guid Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                if (this.Id != value)
                {
                    this._Id = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _FIO { get; set; }
        /// <summary>
        /// Фамилия, имя и отчество контакта
        /// </summary>
        public string FIO
        {
            get
            {
                return this._FIO;
            }
            set
            {
                if (this.FIO != value)
                {
                    this._FIO = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Department { get; set; }
        /// <summary>
        /// Отдел
        /// </summary>
        public string Department
        {
            get
            {
                return this._Department;
            }
            set
            {
                if (this.Department != value)
                {
                    this._Department = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Post { get; set; }
        /// <summary>
        /// Должность контакта
        /// </summary>
        public string Post
        {
            get
            {
                return this._Post;
            }
            set
            {
                if (this.Post != value)
                {
                    this._Post = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _WorkingPhones { get; set; }
        /// <summary>
        /// Рабочий телефон контакта
        /// </summary>
        public string WorkingPhones
        {
            get
            {
                return this._WorkingPhones;
            }
            set
            {
                if (this.WorkingPhones != value)
                {
                    this._WorkingPhones = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _MobilePhones { get; set; }
        /// <summary>
        /// Мобильный телефон контакта
        /// </summary>
        public string MobilePhones
        {
            get
            {
                return this._MobilePhones;
            }
            set
            {
                if (this.MobilePhones != value)
                {
                    this._MobilePhones = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Comment { get; set; }
        /// <summary>
        /// Коментарий
        /// </summary>
        public string Comment
        {
            get
            {
                return this._Comment;
            }
            set
            {
                if (this.Comment != value)
                {
                    this._Comment = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Email { get; set; }
        /// <summary>
        /// E-Mail
        /// </summary>
        public string Email
        {
            get
            {
                return this._Email;
            }
            set
            {
                if (this.Email != value)
                {
                    this._Email = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as MedoContactsModel;
            if (item == null)
            {
                return false;
            }
            return this.Id.Equals(item.Id);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override string ToString()
        {
            return $"{FIO}, {Department}, {Post}, {WorkingPhones}, {MobilePhones}, {Email}, {Comment}";
        }
    }
}
