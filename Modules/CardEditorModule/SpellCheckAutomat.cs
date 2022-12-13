using Medo.Modules.CardEditorModule.Models;
using NLog;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medo.Core.Models;
using Prism.Commands;
using System.IO;

namespace Medo.Modules.CardEditorModule
{
    class SpellCheckAutomat : MedoBaseModel
    {
        new readonly Logger logger = LogManager.GetCurrentClassLogger();
        public SpellCheckAutomat(IEventAggregator _eventAggregator)
        {
            CommandsInitialization();
            //WordIsAddToDictionary = true;
        }

        #region Команды
        /// <summary>
        /// Открытие\закрытие интерфейса проверки орфографии
        /// </summary>
        public DelegateCommand SpellCheckFlyoutIsOpenCommand { get; set; }

        public DelegateCommand AddToDictionaryOrChangeSelectWordCommand { get; set; }

        private void CommandsInitialization()
        {
            SpellCheckFlyoutIsOpenCommand = new DelegateCommand(() => SpellCheckPanelIsOpen = !SpellCheckPanelIsOpen);
            AddToDictionaryOrChangeSelectWordCommand = new DelegateCommand(AddToDictionaryOrChangeSelectWord);
        }
        #endregion


        #region Автомат Левинштейна
        /// <summary>
        /// Состояние локального измениня выбранного слова (true изменине из этого класса false изменение свойства зависимости у Texteditor'a)
        /// </summary>
        private bool localChange = false;
        private void GetLevTWords(string value, int errorchars = 2)
        {
            try
            {
                var levT = new LevenshteinAutomaton.LevTAutomataImitation(value.Trim().ToLower(), errorchars);
                WordsCollectionAfterLevT = new ObservableCollection<string>(StaticProperty.WordsDictionary.Where(str => levT.AcceptWord(str)));
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }

        }

        private void AddToDictionaryOrChangeSelectWord()
        {
            if(WordIsAddToDictionary)
            {
                if (!StaticProperty.WordsDictionary.Contains(SpellCheckerSelectedWord))
                using (StreamWriter sw = new StreamWriter(StaticProperty.DictionaryPath, true, Encoding.Unicode))
                {
                    sw.WriteLine(SelectedWord.Word.ToLower());
                    StaticProperty.WordsDictionary.Add(SelectedWord.Word.ToLower());
                }
            }
          
            else
            {
                localChange = true;
                string word = SelectedWord.Word;
                if(SelectedWord.FirstCharIsUpperCase)
                {
                    char[] ch = SpellCheckerSelectedWord.ToCharArray();
                    ch[0] = Char.ToUpper(ch[0]);
                    WordPositionStruct wp = new WordPositionStruct();
                    wp.IndexOfWord = SelectedWord.IndexOfWord;
                    wp.Word = new string(ch);
                    wp.rootLenght = SelectedWord.rootLenght;
                    SelectedWord = wp;
                }
                else
                {
                    WordPositionStruct wp = new WordPositionStruct();
                    wp.IndexOfWord = SelectedWord.IndexOfWord;
                    wp.Word = SpellCheckerSelectedWord;
                    wp.rootLenght = SelectedWord.rootLenght;
                    SelectedWord = wp;
                }
            }
            SpellCheckPanelIsOpen = false;
        }

        /// <summary>
        /// Коллекция слов выдаваемых алгоритмом Левинштейна после запуска метода 
        /// </summary>
        private ObservableCollection<string> _WordsCollectionAfterLevT { get; set; }
        public ObservableCollection<string> WordsCollectionAfterLevT
        {
            get { return _WordsCollectionAfterLevT; }
            set
            {
                if (WordsCollectionAfterLevT != value)
                {
                    _WordsCollectionAfterLevT = value;
                    OnPropertyChanged();
                }
            }
        }

        private WordPositionStruct _SelectedWord { get; set; }
        public WordPositionStruct SelectedWord
        {
            get { return _SelectedWord; }
            set
            {
                if (SelectedWord != value)
                {
                    _SelectedWord = value;
                    OnPropertyChanged();
                    if (!localChange)
                    {
                        SpellCheckerSelectedIndex = -1;
                        GetLevTWords(SelectedWord.Word.ToLower());
                        SpellCheckPanelIsOpen = true;
                        SpellCheckerSelectedWord = value.Word.ToLower();
                    }

                    localChange = false;
                }
            }
        }

        private bool _SpellCheckPanelIsOpen { get; set; }
        public bool SpellCheckPanelIsOpen
        {
            get { return _SpellCheckPanelIsOpen; }
            set
            {
                if (SpellCheckPanelIsOpen != value)
                {
                    _SpellCheckPanelIsOpen = value;
                    OnPropertyChanged();
                    if (!value)
                        WordsCollectionAfterLevT.Clear();
                }
            }
        }

        private string _SpellCheckerSelectedWord { get; set; }
        public string SpellCheckerSelectedWord
        {
            get { return _SpellCheckerSelectedWord; }
            set
            {
                if (SpellCheckerSelectedWord != value)
                {
                    _SpellCheckerSelectedWord = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _SpellCheckerSelectedIndex { get; set; }
        public int SpellCheckerSelectedIndex
        {
            get { return _SpellCheckerSelectedIndex; }
            set
            {
                if (SpellCheckerSelectedIndex != value)
                {
                    _SpellCheckerSelectedIndex = value;
                    OnPropertyChanged();
                    if (value == -1)
                        WordIsAddToDictionary = true;
                    else
                        WordIsAddToDictionary = false;
                }
            }
        }

        private bool _WordIsAddToDictionary { get; set; }
        public bool WordIsAddToDictionary
        {
            get { return _WordIsAddToDictionary; }
            set
            {
                if (WordIsAddToDictionary != value)
                {
                    _WordIsAddToDictionary = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}
