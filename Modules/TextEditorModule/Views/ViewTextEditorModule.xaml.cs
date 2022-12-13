using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Medo.Modules.TextEditorModule.Views
{
    /// <summary>
    /// Логика взаимодействия для DocumentNameEditorRichTextBox.xaml
    /// </summary>
    public partial class ViewTextEditorModule 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public ViewTextEditorModule()
        {
            InitializeComponent();
            AsyncLoad();
        }
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
        private const string DictionaryPath = @"\\182.5.202.220\Софт для работы\Словарь\RussianNEW.lex";
        private string TempText = null;

        #region Свойство зависимости для установки и чтения текста из ричи

        public static readonly DependencyProperty DocumentTextProperty =
            DependencyProperty.Register("DocumentText",
                                         typeof(String),
                                         typeof(ViewTextEditorModule),
                                         new PropertyMetadata(new PropertyChangedCallback(DocumentTextChanged)));
        private static void DocumentTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue != e.OldValue)
            {
                ViewTextEditorModule doc = (ViewTextEditorModule)obj;
                doc.DocumentText = (string)e.NewValue;
            }
        }
        public string DocumentText
        {
            get
            {
                return (string)GetValue(DocumentTextProperty);
            }
            set
            {
                SetValue(DocumentTextProperty, value);
                var doc = new FlowDocument();
                var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
                range.Load(stream, DataFormats.Text);
                rtb.Document = doc;
            }
        }

        #endregion

        #region Переменные
        private List<string> _WordsAutomation { get; set; }
        public List<string> WordsAutomation
        {
            get
            {
                return this._WordsAutomation;
            }
            set
            {
                if (this.WordsAutomation != value)
                {
                    this._WordsAutomation = value;
                    this.OnPropertyChanged("WordsAutomation");
                }
            }
        }
        private bool _IntelliOpen { get; set; }
        public bool IntelliOpen
        {
            get
            {
                return this._IntelliOpen;
            }
            set
            {
                if (this.IntelliOpen != value)
                {
                    this._IntelliOpen = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private List<string> _IntelliWords { get; set; }
        public List<string> IntelliWords
        {
            get
            {
                return this._IntelliWords;
            }
            set
            {
                if (this.IntelliWords != value)
                {
                    this._IntelliWords = value;
                    this.OnPropertyChanged("IntelliWords");
                }
            }
        }

        private double _CheckMaximum { get; set; }
        public double CheckMaximum
        {
            get
            {
                return this._CheckMaximum;
            }
            set
            {
                if (this.CheckMaximum != value)
                {
                    this._CheckMaximum = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private double _CheckValue { get; set; }
        public double CheckValue
        {
            get
            {
                return this._CheckValue;
            }
            set
            {
                if (this.CheckValue != value)
                {
                    this._CheckValue = value;
                    this.OnPropertyChanged();

                }
            }
        }



        private string _SpellCheckerHeaderName { get; set; }
        public string SpellCheckerHeaderName
        {
            get
            {
                return this._SpellCheckerHeaderName;
            }
            set
            {
                if (this.SpellCheckerHeaderName != value)
                {
                    this._SpellCheckerHeaderName = value;
                    this.OnPropertyChanged("SpellCheckerHeaderName");
                }
            }
        }
        private bool _SpellcheckFlyoutIsOpen { get; set; }
        public bool SpellcheckFlyoutIsOpen
        {
            get
            {
                return this._SpellcheckFlyoutIsOpen;
            }
            set
            {
                if (this.SpellcheckFlyoutIsOpen != value)
                {
                    this._SpellcheckFlyoutIsOpen = value;
                    this.OnPropertyChanged("SpellcheckFlyoutIsOpen");
                }
            }
        }
        private string _SpellCheckerTextblock { get; set; }
        public string SpellCheckerTextblock
        {
            get
            {
                return this._SpellCheckerTextblock;
            }
            set
            {
                if (this.SpellCheckerTextblock != value)
                {
                    this._SpellCheckerTextblock = value;
                    this.OnPropertyChanged("SpellCheckerTextblock");
                }
            }
        }
        private bool _AutoSpellCheck { get; set; }
        public bool AutoSpellCheck
        {
            get
            {
                return this._AutoSpellCheck;
            }
            set
            {
                if (this.AutoSpellCheck != value)
                {
                    this._AutoSpellCheck = value;
                    this.OnPropertyChanged("AutoSpellCheck");
                    if (value)
                    {
                        timer.Start();
                    }
                    else
                    {
                        timer.Stop();
                    }
                }

            }
        }

        private TextPointer CarretPosition { get; set; }
        private bool FirstCharIsUpperCase { get; set; }
        #endregion

        #region проверка выделяемого слова в словаре, запуск алгоритма расстояния Левинштейна    
        //public ICommand SelectAndGetTextFromDependencyRichTextBoxCommand { get { return new RelayCommand(arg => SelectAndGetTextFromDependencyRichTextBox()); } }
        bool needWhiteSpace = false;
        private void SelectAndGetTextFromDependencyRichTextBox()
        {
            try
            {
                CarretPosition = rtb.CaretPosition;
                string txt = rtb.Selection.Text;
                TextPointer tp1 = rtb.Selection.Start;
                var end = rtb.Selection.End.GetPositionAtOffset(0, LogicalDirection.Forward);
                TextRange tr = new TextRange(tp1, end);
                string whitespaceornot = tr.Text;
                if (char.IsWhiteSpace(whitespaceornot[whitespaceornot.Count() - 1]))
                {
                    needWhiteSpace = true;
                }
                else
                {
                    needWhiteSpace = false;
                }
                if (txt.Length > 0)
                {
                    if (Char.IsUpper(txt[0]))
                    {
                        FirstCharIsUpperCase = true;
                    }
                    else
                    {
                        FirstCharIsUpperCase = false;
                    }
                }
                if (txt.Length > 0 && !txt.Contains("\r"))
                {
                    txt = txt.ToLower().Trim();
                    SpellCheckerHeaderName = txt;
                    var levT = new LevenshteinAutomaton.LevTAutomataImitation(txt, 2);
                    WordsAutomation = tags.Where(str => levT.AcceptWord(str)).ToList();
                    SpellCheckerListBoxSelectionChanged(txt);
                    SpellcheckFlyoutIsOpen = true;
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }
        #endregion

        #region Событие изменения ввода в SpellTextBox
        //public ICommand TextChangedInSpellChekerTextBoxCommand { get { return new RelayCommand(a => TextChangedInSpellChekerTextBox()); } }
        private void TextChangedInSpellChekerTextBox()
        {
            try
            {
                var levT = new LevenshteinAutomaton.LevTAutomataImitation(SpellCheckerTextblock.Trim(), 2);
                WordsAutomation = tags.Where(str => levT.AcceptWord(str)).ToList();
            }
            catch (Exception ex) { logger.Fatal(ex); }
        }
        #endregion

        #region Событие изменения выделения в листбоксе спеллчекера
        //public ICommand SpellCheckerListBoxSelectionChangedCommand { get { return new RelayCommand<string>(SpellCheckerListBoxSelectionChanged); } }
        private void SpellCheckerListBoxSelectionChanged(string select)
        {
            try
            {
                string spell = select;
                if (!string.IsNullOrEmpty(spell))
                {
                    if (FirstCharIsUpperCase)
                    {
                        char[] ch = spell.ToCharArray();
                        ch[0] = Char.ToUpper(ch[0]);
                        spell = new string(ch);
                    }
                    SpellCheckerTextblock = spell;
                }
            }
            catch (Exception ex) { logger.Fatal(ex); }
        }
        #endregion

        #region Изменение слова в RichTextBox на слово из словаря или добавление слова в словарь
        //public ICommand ChangeWordCommand { get { return new RelayCommand(arg => ChangeWord()); } }
        private void ChangeWord()
        {
            try
            {
                rtb.Selection.Text = "";
                if (needWhiteSpace)
                {
                    CarretPosition.InsertTextInRun(SpellCheckerTextblock + " ");
                }
                else
                {
                    CarretPosition.InsertTextInRun(SpellCheckerTextblock);
                }
                SpellcheckFlyoutIsOpen = false;
                if (!WordsAutomation.Contains(SpellCheckerTextblock.Trim()))
                {
                    AddToDictionary(SpellCheckerTextblock);
                    tags.Add(SpellCheckerTextblock);
                }
                WordsAutomation.Clear();
                DocumentText = getText;
                TempText = string.Empty;
            }
            catch (Exception ex) { logger.Fatal(ex); }

        }
        #endregion

        #region Открытие\закрытие интерфейса проверки орфографии
        //public ICommand SpellCheckFlyoutIsOpenCommand { get { return new RelayCommand(arg => SpellCheckFlyoutIsOpen()); } }
        private void SpellCheckFlyoutIsOpen()
        {
            try
            {
                SpellcheckFlyoutIsOpen = !SpellcheckFlyoutIsOpen;
            }
            catch (Exception ex) { logger.Fatal(ex); }

        }
        #endregion

        #region смена регистра в наименовании
        //Сменить регистр букв в наименовании на маленький
        //public ICommand ActTextToLowerCaseCommand { get { return new RelayCommand(arg => SetLowerCase()); } }
        private void SetLowerCase()
        {
            try
            {
                DocumentText = DocumentText.ToLower();
            }
            catch (Exception ex) { logger.Fatal(ex); }

        }
        #endregion


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (TempText != getText)
            {
                TempText = getText;
                // DocumentText = TempText;
                CheckText();
            }
        }



        private async void AsyncLoad()
        {
            await Task.Factory.StartNew(() =>
            {
                tags = new List<string>(File.ReadAllLines(DictionaryPath).ToList());
                RefreshWords();
                timer.Interval = new TimeSpan(0, 0, 0, 2);
                timer.Tick += Timer_Tick;
                AutoSpellCheck = true;
              //this.LostKeyboardFocus += DocumentNameEditorRichTextBox_LostKeyboardFocus;

                });
        }

        //private void DocumentNameEditorRichTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    DocumentText = TempText;
        //}

        //private void DocumentNameEditorRichTextBox_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (TempText != getText)
        //    {
        //        DocumentText = TempText;
        //    }
        //}

        public string getText
        {
            get
            {
                try
                {
                    var range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    return range.Text;
                }
                catch (System.Exception ex)
                {
                    logger.Fatal(ex);
                    return ex.Message;
                }

            }
        }

        public async void setText(string value)
        {
            Dispatcher disp = Dispatcher;
            await disp.BeginInvoke(new Action(() =>
            {
                var doc = new FlowDocument();
                var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
                range.Load(stream, DataFormats.Text);
                rtb.Document = doc;
            }));
        }

        public void Dispose()
        {
            //timer = null;

        }



        #region Проверка орфографии

        delegate void checkText();
        private async void CheckText()
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    if (rtb.Document == null)
                        return;
                    m_tags.Clear();
                    TextRange documentRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

                    string txt = documentRange.Text;

                    await Dispatcher.BeginInvoke(DispatcherPriority.Background, new checkText(() =>
                    {
                        documentRange.ClearAllProperties();
                        documentRange.ApplyPropertyValue(Paragraph.MarginProperty, new Thickness(0, 7, 0, 0));
                        rtb.Document.Foreground = new SolidColorBrush(Colors.Black);
                    }));
                        //Now let's create navigator to go though the text, find all the keywords but do not hightlight
                        TextPointer navigator = rtb.Document.ContentStart;

                    while (navigator.CompareTo(rtb.Document.ContentEnd) < 0)
                    {
                        TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);

                        if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                        {
                            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new checkText(() =>
                            {
                                text = ((Run)navigator.Parent).Text; //fix 2
                                }));
                            if (text != "")
                            {
                                CheckWordsInRun((Run)navigator.Parent);
                            }
                        }
                        navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
                    }
                    CheckMaximum = m_tags.Count - 1;
                        //only after all keywords are found, then we highlight them
                        for (int i = 0; i < m_tags.Count; i++)
                    {
                        try
                        {
                            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new checkText(() =>
                            {
                                TextRange range = new TextRange(m_tags[i].StartPosition, m_tags[i].EndPosition);
                                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
                                range.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
                                CheckValue = i;
                            }));
                        }
                        catch (Exception ex) { logger.Fatal(ex); }
                    }
                });

            }
            catch (Exception ex) { logger.Fatal(ex); };
        }

        #region проверка орфографии
        //Выделяются все слова которые содержатся в словаре
        public bool IsKnownTag(string tag)
        {
            return tags.Exists(delegate (string s) { return s.ToLower().Equals(tag.ToLower()); });
        }
        //Выделяются все слова которые НЕ содержатся в словаре
        public bool IsUnknownTag(string tag)
        {
            return !tags.Exists(delegate (string s) { return s.ToLower().Equals(tag.ToLower()); });
        }

        private bool GetSpecials(char i)
        {
            foreach (var item in specials)
            {
                if (item.Equals(i))
                {
                    return true;
                }
            }
            return false;
        }
        new struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;
        }
        private void RefreshWords()
        {

            char[] chrs = {
                     '.',
                     ')',
                     '(',
                     '[',
                     ']',
                     '>',
                     '<',
                     ':',
                     ';',
                     '\n',
                     '\t',
                     '\r',
                     '"',
                     ',',
                     '|',
                     '-'
                              };
            specials = new List<char>(chrs);

        }

        delegate void CheckWordsInRunDelegate(Run theRun);
        public static List<string> tags = new List<string>();
        List<char> specials = new List<char>();
        string text;


        public void ChangeEditedWord(TextPointer point, string txt)
        {
            TextRange tr = new TextRange(point, point);
            ChangeWordOnPointer(point, txt);
            rtb.CaretPosition = point;
        }

        public void AddToDictionary(string word)
        {
            using (StreamWriter sw = new StreamWriter(DictionaryPath, true, Encoding.Unicode))
            {
                sw.WriteLine(word.ToLower());
                tags.Add(word.ToLower());
            }
        }

        private void ChangeWordOnPointer(TextPointer textPointer, string Changeword)
        {
            textPointer.DeleteTextInRun(-GetWordCharactersBefore(textPointer).Count());
            textPointer.DeleteTextInRun(GetWordCharactersAfter(textPointer).Count());
            textPointer.InsertTextInRun(Changeword);
        }
        private string GetWordAtPointer(TextPointer textPointer)
        {
            return string.Join(string.Empty, GetWordCharactersBefore(textPointer), GetWordCharactersAfter(textPointer)).ToLower();
        }

        private string GetWordCharactersBefore(TextPointer textPointer)
        {
            var backwards = textPointer.GetTextInRun(LogicalDirection.Backward);
            var wordCharactersBeforePoint = new string(backwards.Reverse().TakeWhile(c => !char.IsSeparator(c) && !char.IsPunctuation(c)).Reverse().ToArray());
            return wordCharactersBeforePoint;
        }
        private string GetWordCharactersAfter(TextPointer textPointer)
        {
            var forwards = textPointer.GetTextInRun(LogicalDirection.Forward);
            var wordCharactersAfterPoint = new string(forwards.TakeWhile(c => !char.IsSeparator(c) && !char.IsPunctuation(c)).ToArray());
            return wordCharactersAfterPoint;
        }
        List<Tag> m_tags = new List<Tag>();
        internal void CheckWordsInRun(Run theRun) //do not hightlight keywords in this method
        {
            //How, let's go through our text and save all tags we have to save.               
            int sIndex = 0;
            int eIndex = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | GetSpecials(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | GetSpecials(text[i - 1])))
                    {
                        eIndex = i - 1;
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);
                        //Заменить метод для слов входящих в словарь на IsKnownTag
                        if (IsUnknownTag(word))
                        {
                            Tag t = new Tag();
                            t.StartPosition = theRun.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                            t.EndPosition = theRun.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                            t.Word = word;
                            m_tags.Add(t);
                        }
                    }
                    sIndex = i + 1;
                }
            }
            //How this works. But wait. If the word is last word in my text I'll never hightlight it, due I'm looking for separators. Let's add some fix for this case
            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (IsUnknownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = theRun.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = theRun.ContentStart.GetPositionAtOffset(text.Length, LogicalDirection.Backward); //fix 1
                t.Word = lastWord;
                m_tags.Add(t);
            }
        }

        #endregion

        #endregion

        private void RefreshSpellCheck(object sender, RoutedEventArgs e)
        {
            setText(getText);
            TempText = string.Empty;
        }
    }

}

