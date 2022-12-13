using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AvallonEditTest
{
    public class ColorizeAvalonEdit : DocumentColorizingTransformer
    {
        private const string DictionaryPath = @"\\182.5.202.220\Софт для работы\Словарь\RussianNEW.lex";
        List<string> Dictionary = new List<string>(File.ReadAllLines(DictionaryPath).ToList());
        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            if (text.Length == 0)
                return;
            List<string> txt = text.Split(splitArray).Where(s => s != string.Empty).ToList();
            List<Words> wordIndexes = new List<Words>();
            int index;
           
            for (int i = 0; i < txt.Count; i++)
            {
                string word = txt[i].ToLower();
                int startindex = 0;
                while ((index = text.ToLower().IndexOf(word, startindex)) >= 0)
                {
                    Words w = new Words();
                    w.IndexOfWord = index;
                    w.Word = word;
                    wordIndexes.Add(w);
                    startindex = index + word.Length;
                }
            }
            foreach (var word in wordIndexes)
            {
                if (!Dictionary.Contains(word.Word))
                {
                    base.ChangeLinePart(lineStartOffset + word.IndexOfWord, lineStartOffset + word.IndexOfWord + word.LenghtOfWord, (VisualLineElement element) =>
                        {
                            Typeface tf = element.TextRunProperties.Typeface;
                            //element.TextRunProperties.SetTypeface(new Typeface(tf.FontFamily, FontStyles.Italic, FontWeights.Bold, tf.Stretch));
                            element.TextRunProperties.SetForegroundBrush(Brushes.Red);
                            //element.TextRunProperties.TextDecorations.Add(new TextDecoration(TextDecorationLocation.Underline, new Pen(Brushes.Green, 1),0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended));
                        });
                }
                else
                {
                    //base.ChangeLinePart(lineStartOffset + word.IndexOfWord, lineStartOffset + word.IndexOfWord + word.LenghtOfWord, (VisualLineElement element) =>
                    //{
                    //    element.TextRunProperties.SetForegroundBrush(Brushes.Red);
                    //});
                }
            }
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

    public struct Words
    {
        public int IndexOfWord { get; set; }
        public int LenghtOfWord
        {
            get { return Word.Length; }
        }
        public string Word { get; set; }
    }
}
