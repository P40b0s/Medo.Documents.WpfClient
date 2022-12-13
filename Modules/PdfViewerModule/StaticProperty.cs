using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.PdfViewerModule
{
    public class StaticProperty
    {
        readonly static Logger logger = LogManager.GetCurrentClassLogger();

        #region Инициализация прокси класса
        public StaticProperty(){}
        private static readonly StaticProperty sp = new StaticProperty();
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        static protected void OnStaticPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (StaticPropertyChanged != null)
            {
                StaticPropertyChanged(sp, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Общие переменные
      /// <summary>
      /// Текущая страница MoonPdfPanel
      /// </summary>
        public static int CurrentPageNumber
        {
            get { return _CurrentPageNumber; }
            set
            {
                if (CurrentPageNumber != value)
                {
                    _CurrentPageNumber = value;
                    RecognitionMode = false;
                    OnStaticPropertyChanged();
                }
            }
        }
        private static int _CurrentPageNumber { get; set; }

        private static bool _RecognitionMode { get; set; }
        public static bool RecognitionMode
        {
            get { return _RecognitionMode; }
            set
            {
                if (RecognitionMode != value)
                {
                    _RecognitionMode = value;
                    OnStaticPropertyChanged();
                }
            }
        }

        #region Поля используются при вычислении пропорции для получения битмапа нужного размера из нужной позиции
        /// <summary>
        /// Ширина контрола в котором отображается элемент (pdf или img)
        /// </summary>
        public static double DocumentControlWidth
        {
            get { return _DocumentControlWidth; }
            set
            {
                if (DocumentControlWidth != value)
                {
                    _DocumentControlWidth = value;
                    OnStaticPropertyChanged();
                }
            }
        }
        private static double _DocumentControlWidth { get; set; }
        /// <summary>
        /// Высота контрола в котором отображается элемент (pdf или img)
        /// </summary>
        public static double DocumentControlHeight
        {
            get { return _DocumentControlHeight; }
            set
            {
                if (DocumentControlHeight != value)
                {
                    _DocumentControlHeight = value;
                    OnStaticPropertyChanged();
                }
            }
        }
        private static double _DocumentControlHeight { get; set; }
        #endregion

        #region Ширина и высота текущего Bitmap для которого построены rectangle
        private static double _BitmapHeight { get; set; }
        /// <summary>
        /// Высота битмапа в котором будет усуществлять поиск OpenCv
        /// </summary>
        public static double BitmapHeight
        {
            get { return _BitmapHeight; }
            set
            {
                if (BitmapHeight != value)
                {
                    _BitmapHeight = value;
                    OnStaticPropertyChanged();
                }
            }
        }
        private static double _BitmapWidth { get; set; }
        /// <summary>
        /// Ширина битмапа в котором будет усуществлять поиск OpenCv
        /// </summary>
        public static double BitmapWidth
        {
            get { return _BitmapWidth; }
            set
            {
                if (BitmapWidth != value)
                {
                    _BitmapWidth = value;
                    OnStaticPropertyChanged();
                }
            }
        }
        #endregion



        #endregion

    }
}
