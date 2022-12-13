using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using MessagePack.Formatters;

namespace Medo.Client
{
    [Serializable]
    public class Settings
    {
        #region Инициализация статического класса
        public Settings() { }
        [NonSerialized]
        private static readonly Settings s = new Settings();
        [field: NonSerialized]
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        static protected void OnStaticPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (StaticPropertyChanged != null)
            {
                StaticPropertyChanged(s, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Настройки модуля просмотра и распознования PdfViewerModule

        public static bool RectanglesInDebugMode
        {
            get { return _RectanglesInDebugMode; }
            set
            {
                if (RectanglesInDebugMode != value)
                {
                    _RectanglesInDebugMode = value;
                    OnStaticPropertyChanged();
                }
            }
        }
        private static bool _RectanglesInDebugMode { get; set; }
        #endregion

    }
}
