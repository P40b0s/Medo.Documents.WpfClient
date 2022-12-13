using Medo.Core.EventsAggregator;
using Medo.Core.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Medo.Core.Collections;
using Medo.Core.Interfaces;
using System.Windows.Shapes;
using NLog;
using Medo.Controls.PdfRecognitionViewer;
using System.Collections.ObjectModel;
using Prism.Regions;
using System.Windows.Threading;
using MoonPdfLib;
using Medo.Core.Enums;

namespace Medo.Modules.PdfViewerModule.ViewModels
{
    class ViewContentViewerViewModel : INotifyPropertyChanged
    {
        public IMoonPdfInjection Moon { get; set; }
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private DispatcherTimer PagesTimer { get; set; }
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
        IEventAggregator _aggregator;
        IRegionManager region;


        public ViewContentViewerViewModel(IEventAggregator eventAggregator, IRegionManager _region)
        {
            
            _aggregator = eventAggregator;
            region = _region;
            Commandsinitialization();
            SubscribeEvents();
            PagesTimer = new DispatcherTimer();
            PagesTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            PagesTimer.Tick += PagesTimer_Tick;
            PagesTimer.Start();
            PagesTimer.IsEnabled = false;
            ToolButtonsIsVisible = false;
        }

        private void SubscribeEvents()
        {
            _aggregator.GetEvent<SelectFileEvent>().Subscribe(DocumentSelectionChanged);
        }

        #region Команды
        public DelegateCommand PageToRightCommand { get; set; }
        public DelegateCommand PageToLeftCommand { get; set; }
        public DelegateCommand PageToDoubleRightCommand { get; set; }
        public DelegateCommand PageToDoubleLeftCommand { get; set; }
        //public DelegateCommand ZoomInCommand { get; set; }
        //public DelegateCommand ZoomOutCommand { get; set; }

        private void Commandsinitialization()
        {
            PageToRightCommand = new DelegateCommand(PageToRight, () => FileSource != null);
            PageToLeftCommand = new DelegateCommand(PageToLeft, () => FileSource != null);
            PageToDoubleLeftCommand = new DelegateCommand(() => Moon.MoonPanel.GotoFirstPage(), () => FileSource != null);
            PageToDoubleRightCommand = new DelegateCommand(() => Moon.MoonPanel.GotoLastPage(), () => FileSource != null);
            //ZoomInCommand = new DelegateCommand(zoomIn);
            //ZoomOutCommand = new DelegateCommand(zoomOut);

        }
        #endregion

        #region Методы PdfReadera

        void zoomIn()
        {
            Moon.MoonPanel.Zoom(1.3);
        }
        void zoomOut()
        {
            Moon.MoonPanel.Zoom(1);
        }


        void PageToRight()
        {
            if (FileSource != null)
            {
                if (CurrentPageNumber == TotalPages && TotalPages > 1)
                {
                    Moon.MoonPanel.GotoFirstPage();

                }
                else
                {
                    Moon.MoonPanel.GotoNextPage();
                }
            }
        }
        void PageToLeft()
        {
            if (FileSource != null)
            {
                if (CurrentPageNumber == 1 && TotalPages > 1)
                {
                    Moon.MoonPanel.GotoLastPage();
                }
                else
                {
                    Moon.MoonPanel.GotoPreviousPage();
                }
            }
        }
        private void Pages()
        {
            CurrentPageNumber = Moon.MoonPanel.GetCurrentPageNumber();
            TotalPages = Moon.MoonPanel.TotalPages;
            StaticProperty.DocumentControlHeight = Moon.MoonPanel.ActualHeight;
            StaticProperty.DocumentControlWidth = Moon.MoonPanel.ActualWidth;
        }

        #region Выбор нового документа
        /// <summary>
        /// сдклать выбор только для списка
        /// </summary>
        /// <param name="doc"></param>
        private void DocumentSelectionChanged(ExpanderFileSelectorModel file)
        {
            PagesTimer.IsEnabled = false;
            FileSource = file;
            ViewFileType = file.FileType;
            switch (file.FileType)
            {
                case FileTypeEnum.Pdf:
                    {
                        ViewFileType = FileTypeEnum.Pdf;
                        PdfViewerIsVisible = true;
                        Moon.MoonPanel.OpenFile(file.File.FullName);
                        Moon.MoonPanel.Zoom(1.09);
                        Pages();
                        PagesTimer.IsEnabled = true;
                        ToolButtonsIsVisible = false;
                        PageToRightCommand.RaiseCanExecuteChanged();
                        PageToLeftCommand.RaiseCanExecuteChanged();
                        PageToDoubleRightCommand.RaiseCanExecuteChanged();
                        PageToDoubleLeftCommand.RaiseCanExecuteChanged();
                        break;
                    }

                case FileTypeEnum.Image:
                    {
                        ViewFileType = FileTypeEnum.Image;
                        ImageViewer = FileSource.File;
                        break;
                    }
                case FileTypeEnum.Text:
                    {
                        ViewFileType = FileTypeEnum.Text;
                        TextViewerText = File.ReadAllText(FileSource.File.FullName, Encoding.Default);
                        break;
                    }
            }
        }


        /// <summary>
        /// сдклать выбор только для списка
        /// </summary>
        /// <param name="doc"></param>
        //private void DocumentSelectionChanged()
        //{
        //    PdfIsVisible = Visibility.Collapsed;
        //    switch (FileSource.Extension.ToLower())
        //    {
        //        default:
        //            {
        //                //PdfVisibility = Visibility.Collapsed;
        //                //RichTextBoxVisibility = Visibility.Collapsed;
        //                //XmlViewerVisibility = Visibility.Collapsed;
        //                //FlowDocumentVisibility = Visibility.Collapsed;
        //                //ImageVisibility = Visibility.Collapsed;
        //                PdfIsVisible = Visibility.Hidden;
        //                break;
        //            }
        //        case ".pdf":
        //            {
        //                //PdfVisibility = Visibility.Visible;
        //                //RichTextBoxVisibility = Visibility.Collapsed;
        //                //XmlViewerVisibility = Visibility.Collapsed;
        //                //FlowDocumentVisibility = Visibility.Collapsed;
        //                //ImageVisibility = Visibility.Collapsed;
        //                //ButtonsVisibility = Visibility.Visible;                       
        //                PdfIsVisible = Visibility.Visible;
        //                MoonPdf.OpenFile(FileSource.FullName);
        //                MoonPdf.Zoom(1.1);
        //                Pages();
        //                ToolButtonsIsVisible = false;
        //                PageToRightCommand.RaiseCanExecuteChanged();
        //                PageToLeftCommand.RaiseCanExecuteChanged();
        //                PageToDoubleRightCommand.RaiseCanExecuteChanged();
        //                PageToDoubleLeftCommand.RaiseCanExecuteChanged();
        //                //clearPdfChildrens(PdfCanvas);
        //                //SelectedPdf = file;
        //                break;
        //            }
        //        case ".ltr":
        //            {
        //                // PdfSource = null;
        //                //TextView(file);
        //                break;
        //            }
        //            //case ".txt":
        //            //    {
        //            //        TextView(file);
        //            //        break;
        //            //    }
        //            //case ".xml":
        //            //    {
        //            //        XmlDocument XMLdoc = new XmlDocument();
        //            //        try
        //            //        {
        //            //            XMLdoc.Load(file.FullName);
        //            //        }
        //            //        catch (XmlException)
        //            //        {
        //            //            HeaderNameExpander = string.Format("Ошибка формата файла");
        //            //            return;
        //            //        }
        //            //        XmlViewerVisibility = Visibility.Visible;
        //            //        FlowDocumentVisibility = Visibility.Collapsed;
        //            //        PdfVisibility = Visibility.Collapsed;
        //            //        RichTextBoxVisibility = Visibility.Collapsed;
        //            //        ImageVisibility = Visibility.Collapsed;
        //            //        HeaderNameExpander = String.Format("Просмотр файла {0}", file.Name);
        //            //        xmlViewer.xmlDocument = XMLdoc;
        //            //        clearPdfChildrens(PdfCanvas);
        //            //        break;
        //            //    }
        //            //case ".docx":
        //            //    {

        //            //        try
        //            //        {
        //            //            FlowDocument flowDoc = new FlowDocument();
        //            //            loadWord2007(flowDoc, file.FullName);
        //            //            flowDocViewer.Document = flowDoc;
        //            //        }
        //            //        catch (XmlException)
        //            //        {
        //            //            HeaderNameExpander = string.Format("Ошибка формата файла");
        //            //            return;
        //            //        }
        //            //        XmlViewerVisibility = Visibility.Collapsed;
        //            //        FlowDocumentVisibility = Visibility.Visible;
        //            //        PdfVisibility = Visibility.Collapsed;
        //            //        RichTextBoxVisibility = Visibility.Collapsed;
        //            //        ImageVisibility = Visibility.Collapsed;
        //            //        HeaderNameExpander = String.Format("Просмотр файла {0}", file.Name);
        //            //        clearPdfChildrens(PdfCanvas);
        //            //        break;
        //            //    }
        //            //case ".tiff":
        //            //    {
        //            //        ImageView(file);
        //            //        break;
        //            //    }
        //            //case ".tif":
        //            //    {
        //            //        ImageView(file);
        //            //        break;
        //            //    }
        //            //case ".jpeg":
        //            //    {
        //            //        ImageView(file);
        //            //        break;
        //            //    }
        //            //case ".jpg":
        //            //    {
        //            //        ImageView(file);
        //            //        break;
        //            //    }
        //            //case ".png":
        //            //    {
        //            //        ImageView(file);
        //            //        break;
        //            //    }

        //    }
        //}

        private FileTypeEnum _ViewFileType { get; set; }
        public FileTypeEnum ViewFileType
        {
            get
            {
                return this._ViewFileType;
            }
            set
            {
                if (this.ViewFileType != value)
                {
                    this._ViewFileType = value;
                    this.OnPropertyChanged();
                }
            }
        }


        #endregion

        private void PagesTimer_Tick(object sender, EventArgs e)
        {
            Pages();
        }
        #endregion

        #region Поля привязки PdfReadera
        private ExpanderFileSelectorModel _FileSource { get; set; }
        public ExpanderFileSelectorModel FileSource
        {
            get
            {
                return this._FileSource;
            }
            set
            {
                if (this.FileSource != value)
                {
                    this._FileSource = value;
                    this.OnPropertyChanged();
                }
            }
        }
      
        private int _CurrentPageNumber { get; set; }
        public int CurrentPageNumber
        {
            get
            {
                return this._CurrentPageNumber;
            }
            set
            {
                if (this.CurrentPageNumber != value)
                {
                    this._CurrentPageNumber = value;
                    StaticProperty.CurrentPageNumber = value;
                    if (FileSource != null)
                    {
                        Moon.MoonPanel.GotoPage(value);
                    }
                    this.OnPropertyChanged();
                    
                }
            }
        }

        private int _TotalPages { get; set; }
        public int TotalPages
        {
            get
            {
                return this._TotalPages;
            }
            set
            {
                if (this.TotalPages != value)
                {
                    this._TotalPages = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private bool _ToolButtonsIsVisible { get; set; }
        public bool ToolButtonsIsVisible
        {
            get
            {
                return this._ToolButtonsIsVisible;
            }
            set
            {
                if (this.ToolButtonsIsVisible != value)
                {
                    this._ToolButtonsIsVisible = value;
                    this.OnPropertyChanged();
                    if (value)
                    {
                        PagesTimer.IsEnabled = true;
                    }
                    else
                    {
                        PagesTimer.IsEnabled = false;
                    }

                }
            }
        }

        #region Видимость элементов
      
        private bool _PdfViewerIsVisible { get; set; }
        public bool PdfViewerIsVisible
        {
            get
            {
                return this._PdfViewerIsVisible;
            }
            set
            {
                if (this.PdfViewerIsVisible != value)
                {
                    this._PdfViewerIsVisible = value;
                    this.OnPropertyChanged();

                }
            }
        }
       
        private string _TextViewerText { get; set; }
        /// <summary>
        /// Просмотр тектовых файлов
        /// </summary>
        public string TextViewerText
        {
            get
            {
                return this._TextViewerText;
            }
            set
            {
                if (this.TextViewerText != value)
                {
                    this._TextViewerText = value;
                    this.OnPropertyChanged();

                }
            }
        }
       

        
        private FileInfo _ImageViewer { get; set; }
        /// <summary>
        /// Просмотр изображений
        /// </summary>
        public FileInfo ImageViewer
        {
            get
            {
                return this._ImageViewer;
            }
            set
            {
                if (this.ImageViewer != value)
                {
                    this._ImageViewer = value;
                    this.OnPropertyChanged();

                }
            }
        }

        #endregion
        #endregion

    }
}
