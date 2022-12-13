//using Medo.Core.Models;
//using NLog;
//using Prism.Commands;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Threading;
//using System.IO;
//using System.Threading.Tasks;
//using Prism.Events;
//using System.Windows.Shapes;

//namespace Medo.Controls.PdfRecognitionViewer
//{
//    /// <summary>
//    /// Логика взаимодействия для DocumentsViewer.xaml
//    /// </summary>
//    public partial class PdfViewer : UserControl, INotifyPropertyChanged
//    {
//        readonly Logger logger = LogManager.GetCurrentClassLogger();
//        public TextSearch textSearch { get; set; }
//        private DispatcherTimer PagesTimer { get; set; }

//        #region INotifyPropertyChanged
//        public event PropertyChangedEventHandler PropertyChanged;
//        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
//        {
//            if (this.PropertyChanged != null)
//            {
//                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }
//        #endregion

//        #region Инициализация

//        public PdfViewer()
//        {
//            InitializeComponent();
//            textSearch = new TextSearch();
//            textSearch.PropertyChanged += TextSearch_PropertyChanged;
//            textSearch.SearchInProgress = false;
//            Commandsinitialization();
//            PagesTimer = new DispatcherTimer();
//            PagesTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
//            PagesTimer.Tick += PagesTimer_Tick;
//            PagesTimer.Start();
//            PagesTimer.IsEnabled = false;
//            ToolButtonsIsVisible = false;
//        }

//        private void TextSearch_PropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            OnPropertyChanged("textSearch." + e.PropertyName);
//        }
//        #endregion

//        #region Свойства зависимостей
//        [Bindable(true)]
//        public FileInfo FileSource
//        {
//            get { return (FileInfo)GetValue(FileSourceProperty); }
//            set { SetValue(FileSourceProperty, value); }
//        }

//        public static readonly DependencyProperty FileSourceProperty
//          = DependencyProperty.Register("FileSource",
//              typeof(FileInfo),
//              typeof(PdfViewer),
//              new PropertyMetadata(null, new PropertyChangedCallback(FileSourceChanged)));

//        private static void FileSourceChanged(DependencyObject dep, DependencyPropertyChangedEventArgs e)
//        {
//            var item = (PdfViewer)dep;
//            var newitem = e.NewValue;
//            var olditem = e.OldValue;
//            if (e.NewValue != null)
//            item.DocumentSelectionChanged();
//        }

//        #region Свойство зависимости и обработка прямоугольника для передачи изображения в модуль распознования
//        [Bindable(true)]
//        public System.Drawing.Bitmap BitmapForRecognition
//        {
//            get { return (System.Drawing.Bitmap)GetValue(BitmapForRecognitionProperty); }
//            set { SetValue(BitmapForRecognitionProperty, value); }
//        }

//        public static readonly DependencyProperty BitmapForRecognitionProperty
//          = DependencyProperty.Register("BitmapForRecognition",
//              typeof(System.Drawing.Bitmap),
//              typeof(PdfViewer));

//        private Rectangle _RectangleForRecognition { get; set; }
//        public Rectangle RectangleForRecognition
//        {
//            get
//            {
//                return this._RectangleForRecognition;
//            }
//            set
//            {
//                this._RectangleForRecognition = value;
//                using (System.Drawing.Bitmap b = getCropImage(value))
//                {
//                    BitmapForRecognition = b;
//                }
//                this.OnPropertyChanged();


//            }
//        }

//        private System.Drawing.Bitmap getCropImage(System.Windows.Shapes.Rectangle rec)
//        {
//            try
//            {
//                double YCoords = Proportions.ToImageProportions(Canvas.GetTop(rec), can.ActualHeight, textSearch.BitmapHeight);
//                double XCoords = Proportions.ToImageProportions(Canvas.GetLeft(rec), can.ActualWidth, textSearch.BitmapWidth);
//                double height = Proportions.ToImageProportions(rec.ActualHeight, can.ActualHeight, textSearch.BitmapHeight);
//                double width = Proportions.ToImageProportions(rec.ActualWidth, can.ActualWidth, textSearch.BitmapWidth);
//                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle();
//                rectangle.Height = Convert.ToInt16(height);
//                rectangle.Width = Convert.ToInt16(width);
//                rectangle.X = Convert.ToInt16(XCoords);
//                rectangle.Y = Convert.ToInt16(YCoords);
//                return textSearch.getCropImage(rectangle);
//            }
//            catch (System.Exception ex)
//            {
//                logger.Fatal(ex);
//                return null;
//            }
//        }

//        #endregion

//        [Bindable(true)]
//        public bool DocumentIsEdit
//        {
//            get { return (bool)GetValue(DocumentIsEditProperty); }
//            set { SetValue(DocumentIsEditProperty, value); }
//        }

//        public static readonly DependencyProperty DocumentIsEditProperty
//          = DependencyProperty.Register("DocumentIsEdit",
//              typeof(bool),
//              typeof(PdfViewer),
//              new FrameworkPropertyMetadata(false, new PropertyChangedCallback(DocumentIsEditChanged)));

//        private async static void DocumentIsEditChanged(DependencyObject dep, DependencyPropertyChangedEventArgs e)
//        {
//            var item = (PdfViewer)dep;
//            var newitem = e.NewValue;
//            var olditem = e.OldValue;
//            if (newitem != null)
//            {
//                if (!(bool)newitem)
//                {
//                    item.ClearAllChildrens();
//                    item.BitmapForRecognition = null;
//                }
//                else
//                {
//                    if (item.FileSource.Extension.ToLower() == ".pdf")
//                    {
//                        await item.textSearch.GetPageForRecognition(item.CurrentPageNumber, item.FileSource);
//                        item.getContours(item.textSearch.ContoursCollection);
//                    }

//                }
//            }
//        }


//        #endregion

//        #region Команды
//        public DelegateCommand PageToRightCommand { get; set; }
//        public DelegateCommand PageToLeftCommand { get; set; }
//        public DelegateCommand PageToDoubleRightCommand { get; set; }
//        public DelegateCommand PageToDoubleLeftCommand { get; set; }
//        //public DelegateCommand ZoomInCommand { get; set; }
//        //public DelegateCommand ZoomOutCommand { get; set; }

//        private void Commandsinitialization()
//        {
//            PageToRightCommand = new DelegateCommand(PageToRight, () => FileSource != null);
//            PageToLeftCommand = new DelegateCommand(PageToLeft, () => FileSource != null);
//            PageToDoubleLeftCommand = new DelegateCommand(() => MoonPdf.GotoFirstPage(), () => FileSource != null);
//            PageToDoubleRightCommand = new DelegateCommand(() => MoonPdf.GotoLastPage(), () => FileSource != null);
//            //ZoomInCommand = new DelegateCommand(zoomIn);
//            //ZoomOutCommand = new DelegateCommand(zoomOut);

//        }
//        #endregion

//        #region Методы PdfReadera

//        void zoomIn()
//        {
//            MoonPdf.Zoom(1.3);
//        }
//        void zoomOut()
//        {
//            MoonPdf.Zoom(1);
//        }


//        void PageToRight()
//        {
//            if (FileSource != null)
//            {
//                if (CurrentPageNumber == TotalPages && TotalPages > 1)
//                {
//                    MoonPdf.GotoFirstPage();

//                }
//                else
//                {
//                    MoonPdf.GotoNextPage();
//                }
//            }
//        }
//        void PageToLeft()
//        {
//            if (FileSource != null)
//            {
//                if (CurrentPageNumber == 1 && TotalPages > 1)
//                {
//                    MoonPdf.GotoLastPage();
//                }
//                else
//                {
//                    MoonPdf.GotoPreviousPage();
//                }
//            }
//        }
//        private void Pages()
//        {
//            int current = MoonPdf.GetCurrentPageNumber();
//            int total = MoonPdf.TotalPages;
//            CurrentPageNumber = current;
//            TotalPages = total;
//        }

//        #region Выбор нового документа
//        /// <summary>
//        /// сдклать выбор только для списка
//        /// </summary>
//        /// <param name="doc"></param>
//        private void DocumentSelectionChanged()
//        {
//            PdfIsVisible = Visibility.Collapsed;
//            switch (FileSource.Extension.ToLower())
//            {
//                default:
//                    {
//                        //PdfVisibility = Visibility.Collapsed;
//                        //RichTextBoxVisibility = Visibility.Collapsed;
//                        //XmlViewerVisibility = Visibility.Collapsed;
//                        //FlowDocumentVisibility = Visibility.Collapsed;
//                        //ImageVisibility = Visibility.Collapsed;
//                        PdfIsVisible = Visibility.Hidden;
//                        break;
//                    }
//                case ".pdf":
//                    {
//                        //PdfVisibility = Visibility.Visible;
//                        //RichTextBoxVisibility = Visibility.Collapsed;
//                        //XmlViewerVisibility = Visibility.Collapsed;
//                        //FlowDocumentVisibility = Visibility.Collapsed;
//                        //ImageVisibility = Visibility.Collapsed;
//                        //ButtonsVisibility = Visibility.Visible;                       
//                        PdfIsVisible = Visibility.Visible;
//                        MoonPdf.OpenFile(FileSource.FullName);
//                        MoonPdf.Zoom(1.1);
//                        Pages();
//                        ToolButtonsIsVisible = false;
//                        PageToRightCommand.RaiseCanExecuteChanged();
//                        PageToLeftCommand.RaiseCanExecuteChanged();
//                        PageToDoubleRightCommand.RaiseCanExecuteChanged();
//                        PageToDoubleLeftCommand.RaiseCanExecuteChanged();
//                        //clearPdfChildrens(PdfCanvas);
//                        //SelectedPdf = file;
//                        break;
//                    }
//                case ".ltr":
//                    {
//                        // PdfSource = null;
//                        //TextView(file);
//                        break;
//                    }
//                    //case ".txt":
//                    //    {
//                    //        TextView(file);
//                    //        break;
//                    //    }
//                    //case ".xml":
//                    //    {
//                    //        XmlDocument XMLdoc = new XmlDocument();
//                    //        try
//                    //        {
//                    //            XMLdoc.Load(file.FullName);
//                    //        }
//                    //        catch (XmlException)
//                    //        {
//                    //            HeaderNameExpander = string.Format("Ошибка формата файла");
//                    //            return;
//                    //        }
//                    //        XmlViewerVisibility = Visibility.Visible;
//                    //        FlowDocumentVisibility = Visibility.Collapsed;
//                    //        PdfVisibility = Visibility.Collapsed;
//                    //        RichTextBoxVisibility = Visibility.Collapsed;
//                    //        ImageVisibility = Visibility.Collapsed;
//                    //        HeaderNameExpander = String.Format("Просмотр файла {0}", file.Name);
//                    //        xmlViewer.xmlDocument = XMLdoc;
//                    //        clearPdfChildrens(PdfCanvas);
//                    //        break;
//                    //    }
//                    //case ".docx":
//                    //    {

//                    //        try
//                    //        {
//                    //            FlowDocument flowDoc = new FlowDocument();
//                    //            loadWord2007(flowDoc, file.FullName);
//                    //            flowDocViewer.Document = flowDoc;
//                    //        }
//                    //        catch (XmlException)
//                    //        {
//                    //            HeaderNameExpander = string.Format("Ошибка формата файла");
//                    //            return;
//                    //        }
//                    //        XmlViewerVisibility = Visibility.Collapsed;
//                    //        FlowDocumentVisibility = Visibility.Visible;
//                    //        PdfVisibility = Visibility.Collapsed;
//                    //        RichTextBoxVisibility = Visibility.Collapsed;
//                    //        ImageVisibility = Visibility.Collapsed;
//                    //        HeaderNameExpander = String.Format("Просмотр файла {0}", file.Name);
//                    //        clearPdfChildrens(PdfCanvas);
//                    //        break;
//                    //    }
//                    //case ".tiff":
//                    //    {
//                    //        ImageView(file);
//                    //        break;
//                    //    }
//                    //case ".tif":
//                    //    {
//                    //        ImageView(file);
//                    //        break;
//                    //    }
//                    //case ".jpeg":
//                    //    {
//                    //        ImageView(file);
//                    //        break;
//                    //    }
//                    //case ".jpg":
//                    //    {
//                    //        ImageView(file);
//                    //        break;
//                    //    }
//                    //case ".png":
//                    //    {
//                    //        ImageView(file);
//                    //        break;
//                    //    }

//            }
//        }

//        #endregion

//        private void PagesTimer_Tick(object sender, EventArgs e)
//        {
//            Pages();
//        }
//        #endregion

//        #region Методы Canvas'a
//        private async void getContours(IEnumerable<RectangleWithCoordinates> collection)
//        {
//            try
//            {
//                await Task.Factory.StartNew(() =>
//                {
//                    int count = 0;
//                    foreach (RectangleWithCoordinates rect in collection)
//                    {
//                        double heightCoords = Proportions.ToElementProportions(rect.YCoords, can.ActualHeight, rect.BitmapHeight);
//                        double widthCoords = Proportions.ToElementProportions(rect.XCoords, can.ActualWidth, rect.BitmapWidth);

//                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
//                    {
//                        rect.rectangle.Height = Proportions.ToElementProportions(rect.rectangle.Height, can.ActualHeight, rect.BitmapHeight);
//                        rect.rectangle.Width = Proportions.ToElementProportions(rect.rectangle.Width, can.ActualWidth, rect.BitmapWidth);
//                        System.Windows.Controls.Canvas.SetTop(rect.rectangle, heightCoords);
//                        System.Windows.Controls.Canvas.SetLeft(rect.rectangle, widthCoords);
//                        System.Windows.Controls.Canvas.SetZIndex(rect.rectangle, 2);

//                        //rect.rectangle.MouseEnter += Rectangle_MouseEnter;
//                        //rect.rectangle.MouseLeave += Rectangle_MouseLeave;
//                        //rect.rectangle.MouseDown += Rectangle_MouseDown;
//                        //rect.rectangle.MouseLeftButtonDown += Rectangle_MouseLeftButtonDown;
//                        //rect.rectangle.MouseRightButtonDown += Rectangle_MouseRightButtonDown;
//                        can.Children.Add(rect.rectangle);
//                        count++;
//                        textSearch.SearchProgressText = string.Format("Отрисовка блоков... {0}", count);
//                        textSearch.SearchProgressValue = count;
//                    }));
//                    }
//                    textSearch.SearchInProgress = false;
//                });
//            }
//            catch (System.Exception ex)
//            {
//                logger.Fatal(ex);
//            }
//        }

//        /// <summary>
//        /// Удаляем всех чилдренов кроме 0-вого 0-вой это грид с пдфвьювером
//        /// </summary>
//        private void ClearAllChildrens()
//        {
//            for (int i = can.Children.Count - 1; i >= 0; i--)
//            {
//                if (i > 0)
//                {
//                    can.Children.RemoveAt(i);
//                }
//            }
//        }
//        #endregion

//        #region Привязки Canvas'a

//        #endregion

//        #region Поля привязки PdfReadera
//        private int _CurrentPageNumber { get; set; }
//        public int CurrentPageNumber
//        {
//            get
//            {
//                return this._CurrentPageNumber;
//            }
//            set
//            {
//                if (this.CurrentPageNumber != value)
//                {
//                    this._CurrentPageNumber = value;
//                    if (FileSource != null)
//                    {
//                        MoonPdf.GotoPage(value);
//                    }
//                    this.OnPropertyChanged();

//                }
//            }
//        }

//        private int _TotalPages { get; set; }
//        public int TotalPages
//        {
//            get
//            {
//                return this._TotalPages;
//            }
//            set
//            {
//                if (this.TotalPages != value)
//                {
//                    this._TotalPages = value;
//                    this.OnPropertyChanged();

//                }
//            }
//        }

//        private bool _ToolButtonsIsVisible { get; set; }
//        public bool ToolButtonsIsVisible
//        {
//            get
//            {
//                return this._ToolButtonsIsVisible;
//            }
//            set
//            {
//                if (this.ToolButtonsIsVisible != value)
//                {
//                    this._ToolButtonsIsVisible = value;
//                    this.OnPropertyChanged();
//                    if (value)
//                    {
//                        PagesTimer.IsEnabled = true;
//                    }
//                    else
//                    {
//                        PagesTimer.IsEnabled = false;
//                    }

//                }
//            }
//        }
//        private Visibility _PdfIsVisible { get; set; }
//        public Visibility PdfIsVisible
//        {
//            get
//            {
//                return this._PdfIsVisible;
//            }
//            set
//            {
//                if (this.PdfIsVisible != value)
//                {
//                    this._PdfIsVisible = value;
//                    this.OnPropertyChanged();

//                }
//            }
//        }

//        #endregion

//        #region Програмные эвенты

//        #endregion

//        #region 
//        #endregion




//    }
//}
