using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Medo.Modules.CardEditorModule.Models
{
    public class ColorizeAvalonEdit : DocumentColorizingTransformer
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public ColorizeAvalonEdit()
        {
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            if (text.Length == 0)
                return;
            List<string> txt = text.Split(splitArray).Where(s => s != string.Empty).Select(s=> s.Trim()).ToList();
            List<WordPositionStruct> wordIndexes = new List<WordPositionStruct>();
            int index;

            for (int i = 0; i < txt.Count; i++)
            {
                string word = txt[i].ToLower();
                int startindex = 0;
                while ((index = text.ToLower().IndexOf(word, startindex)) >= 0)
                {
                    bool fullWord = true;
                    try
                    {
                        if (index > 0 && (text.Length - 1 > (index + word.Length)))
                        {
                            //Прорверяем находиться ли выделяемый символ в составе другого слова или он отдельный
                            //До этого текст выделялся частями
                            char charBeforeWord = text[index - 1];
                            char charAfterWord = text[(index + word.Length)];
                            if (!splitArray.Contains(charBeforeWord) && !splitArray.Contains(charAfterWord))
                                fullWord = false;
                            if (splitArray.Contains(charBeforeWord) && !splitArray.Contains(charAfterWord))
                                fullWord = false;
                            if (!splitArray.Contains(charBeforeWord) && splitArray.Contains(charAfterWord))
                                fullWord = false;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        logger.Info(ex);
                    }
                    if (fullWord)
                    {
                        var w = new WordPositionStruct();
                        w.IndexOfWord = index;
                        w.Word = word;
                        wordIndexes.Add(w);
                    }
                   
                        startindex = index + word.Length;
                  
                }
            }
            // Foreground в XAML установлен в красный, здесь мы ошибки выделяем жирным, те что содержаться в словаре выделяем черным
            foreach (var word in wordIndexes)
            {
                if (!StaticProperty.WordsDictionary.Contains(word.Word)  && !IsNumber(word.Word))
                {
                    base.ChangeLinePart(lineStartOffset + word.IndexOfWord, lineStartOffset + word.IndexOfWord + word.GetLenghtOfWord, (VisualLineElement element) =>
                        {
                            Typeface tf = element.TextRunProperties.Typeface;
                            element.TextRunProperties.SetTypeface(new Typeface(tf.FontFamily, FontStyles.Normal, FontWeights.Bold, tf.Stretch));
                            //element.TextRunProperties.SetForegroundBrush(Brushes.Red);
                            //element.TextRunProperties.TextDecorations.Add(new TextDecoration(TextDecorationLocation.Underline, new Pen(Brushes.Green, 1),0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended));
                        });
                }
                else
                {
                    base.ChangeLinePart(lineStartOffset + word.IndexOfWord, lineStartOffset + word.IndexOfWord + word.GetLenghtOfWord, (VisualLineElement element) =>
                    {
                        Typeface tf = element.TextRunProperties.Typeface;
                        element.TextRunProperties.SetTypeface(new Typeface(tf.FontFamily, FontStyles.Normal, FontWeights.Normal, tf.Stretch));
                        element.TextRunProperties.SetForegroundBrush(Brushes.Black);
                        //element.TextRunProperties.TextDecorations.Add(new TextDecoration(TextDecorationLocation.Underline, new Pen(Brushes.Green, 1),0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended));
                    });
                }
            }
        }

        private bool IsNumber(string s)
        {
            int n = 0;
            return int.TryParse(s, out n);
        }

        readonly char[] splitArray = new char[] {
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
                 '-',
                 ' '};
    }


}
