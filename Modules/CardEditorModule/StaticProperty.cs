using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Medo.Modules.CardEditorModule.Models;

namespace Medo.Modules.CardEditorModule
{
    public class StaticProperty
    {
        readonly static Logger logger = LogManager.GetCurrentClassLogger();
        public const string DictionaryPath = @"\\182.5.202.220\Софт для работы\Словарь\RussianNEW.lex";
        public static ObservableCollection<string> WordsDictionary = new ObservableCollection<string>();
        #region Инициализация прокси класса
        public StaticProperty(){
            try
            {
                WordsDictionary = new ObservableCollection<string>(File.ReadAllLines(DictionaryPath).ToList());
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);	
            }
        }
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

    }
}
