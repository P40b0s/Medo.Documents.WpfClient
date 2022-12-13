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
using NLog;

namespace Medo.Modules.SelectBox
{
    /// <summary>
    /// Логика взаимодействия для SelectBox.xaml
    /// </summary>
    public partial class ViewIncrementSelectBox : UserControl, INotifyPropertyChanged
    {
        public ViewIncrementSelectBox()
        {
            InitializeComponent();
            CommandInitialization();

        }

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


        public DelegateCommand ClearTextCommand { get; set; }
        public DelegateCommand<object> SelectionChangedCommand { get; set; }

        void CommandInitialization()
        {
            ClearTextCommand = new DelegateCommand(ClearText);
        }

        #region Команды

        void ClearText()
        {
            SelectedItem = null;
            TextBoxText = null;
        }
        #endregion

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

        public TextInlineSelection InlineSelectedItem
        {
            get
            {
                if (SelectedItem != null)
                    return new TextInlineSelection(SelectedItem, null);
                else return null;
            }
            set
            {
                //if (this.InlineSelectedItem != value)
                {
                    SelectedItem = value.SourceText;
                    OnPropertyChanged();
                }
            }
        }



        #region Свойства зависимости
        [Bindable(true)]
        /// <summary>
        /// Связанная коллекиця итемов (ObservableCollection<TextInlineSelection>())
        /// </summary>
        public IEnumerable ItemsCollection
        {
            get { return (IEnumerable)GetValue(ItemsCollectionProperty); }
            set { SetValue(ItemsCollectionProperty, value); }
        }
        /// <summary>
        /// Связанная коллекиця итемов (ObservableCollection<TextInlineSelection>())
        /// </summary>
        public static readonly DependencyProperty ItemsCollectionProperty
            = DependencyProperty.Register("ItemsCollection",
                typeof(IEnumerable),
                typeof(ViewIncrementSelectBox),
                new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });

        [Bindable(true)]
        /// <summary>
        /// Привязка принявшего органа
        /// </summary>
        public string SelectedOrgan
        {
            get { return (string)GetValue(SelectedOrganProperty); }
            set { SetValue(SelectedOrganProperty, value); }
        }
        /// <summary>
        /// Привязка принявшего органа
        /// </summary>
        public static readonly DependencyProperty SelectedOrganProperty
        = DependencyProperty.Register("SelectedOrgan",
            typeof(string),
            typeof(ViewIncrementSelectBox),
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
            });


        [Bindable(true)]
        /// <summary>
        /// Привязка вида документа
        /// </summary>
        public string SelectedItem
        {
            get { return (string)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        /// <summary>
        /// Привязка вида документа
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty
            = DependencyProperty.Register("SelectedItem",
                typeof(string),
                typeof(ViewIncrementSelectBox),
                 new PropertyMetadata(new PropertyChangedCallback(SelectedItemChanged)));


        private static void SelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var context = (ViewIncrementSelectBox)obj;
            context.OnPropertyChanged("InlineSelectedItem");
            context.TextBoxText = (string)e.NewValue;
        }
        #endregion

    }
}
