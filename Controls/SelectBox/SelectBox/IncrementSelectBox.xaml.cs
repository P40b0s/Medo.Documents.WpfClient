using Medo.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prism.Commands;

namespace Medo.Controls.SelectBox
{
    /// <summary>
    /// Логика взаимодействия для SelectBox.xaml
    /// </summary>
    public partial class IncrementSelectBox : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        IncrementSearch increment = new IncrementSearch();
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public DelegateCommand ClearTextCommand { get; set; }
        public DelegateCommand<object> SelectionChangedCommand { get; set; }
        public IncrementSelectBox()
        {
            InitializeComponent();
            CommandInitialization();
        }

        void CommandInitialization()
        {
            ClearTextCommand = new DelegateCommand(ClearText);
        }

        #region Команды

        void ClearText()
        {
            RemoveSelectionFromButton = true;
            SelectedItem = null;
            NowSelectedItem = null;
        }
        #endregion

        #region Поля привязки
        /// <summary>
        /// Поле связанное с текстом поискового текстбокса
        /// </summary>
        public string TextBoxText
        {
            get
            {
                return this._TextBoxText;
            }
            set
            {
                if (this.TextBoxText != value)
                {
                    this._TextBoxText = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _TextBoxText { get; set; }
        /// <summary>
        /// Попап с листбоксом списком
        /// </summary>
        public bool PopUpIsOpen
        {
            get
            {
                return this._PopUpIsOpen;
            }
            set
            {
                if (this.PopUpIsOpen != value)
                {
                    this._PopUpIsOpen = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _PopUpIsOpen { get; set; }
        /// <summary>
        /// Связанный выделенный итем в листбоксе
        /// </summary>
        public TextInlineSelection SelectedItem
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
                    OnPropertyChanged();
                    if (value != null)
                    {
                        NowSelectedItem = value.SourceText;
                        TextBoxText = value.SourceText;
                    }
                    else
                    {
                        NowSelectedItem = null;
                        TextBoxText = null;
                    }
                }
            }
        }
        private TextInlineSelection _SelectedItem { get; set; }

        bool RemoveSelectionFromButton { get; set; }
        #endregion

        #region Свойства зависимостей

        #region Связанная коллекиця итемов (ObservableCollection<TextInlineSelection>())
        [Bindable(true)]
        public IEnumerable Collection
        {
            get { return (IEnumerable)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public static readonly DependencyProperty CollectionProperty
            = DependencyProperty.Register("Collection",
                typeof(IEnumerable),
                typeof(IncrementSelectBox));

        #endregion

        #region Специальное поле для привязки свойств выбранного в данный момент принявшего органа документа
        /// <summary>
        /// Специальное поле для привязки свойств выбранного в данный момент принявшего органа документа
        /// </summary>
        [Bindable(true)]
        public string NowSelectedItem
        {
            get { return (string)GetValue(NowSelectedItemProperty); }
            set { SetValue(NowSelectedItemProperty, value); }
        }

        public static readonly DependencyProperty NowSelectedItemProperty
          = DependencyProperty.Register("NowSelectedItem",
              typeof(string),
              typeof(IncrementSelectBox),
              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(NowSelectedItemChanged)));

        private static void NowSelectedItemChanged(DependencyObject dep, DependencyPropertyChangedEventArgs e)
        {
            var item = (IncrementSelectBox)dep;
            var newitem = e.NewValue;
            var olditem = e.OldValue;
            if (olditem == null || newitem != null)
            {
                item.SelectedItem = new TextInlineSelection((string)e.NewValue, string.Empty);
            }
            //if (newitem != null && newitem != olditem)
            //{
            //    item.SelectedItem = new TextInlineSelection((string)e.NewValue, string.Empty);
            //}
            if (newitem == null)
            {
                item.SelectedItem = null;
                Keyboard.Focus(item.txt);
            }
        }

        [Bindable(true)]
        public string SelectedOrganForTrigger
        {
            get { return (string)GetValue(SelectedOrganForTriggerProperty); }
            set { SetValue(SelectedOrganForTriggerProperty, value); }
        }

        public static readonly DependencyProperty SelectedOrganForTriggerProperty
          = DependencyProperty.Register("SelectedOrganForTrigger",
              typeof(string),
              typeof(IncrementSelectBox),
              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(SelectedOrganForTriggerChanged)));

        private static void SelectedOrganForTriggerChanged(DependencyObject dep, DependencyPropertyChangedEventArgs e)
        {
            var item = (IncrementSelectBox)dep;
            string newitem = (string)e.NewValue;
            if (newitem != null)
            {
                if (newitem.ToLower() == "правление пенсионного фонда российской федерации")
                {
                    item.NowSelectedItem = "Постановление";
                }
                if (newitem.ToLower() == "главный государственный санитарный врач российской федерации")
                {
                    item.NowSelectedItem = "Постановление";
                }
            }

        }
        #endregion

        #endregion
    }
}
