using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace AvallonEditTest.Models
{
    class TextCompletionData : ICompletionData
    {
        public TextCompletionData(string text)
        {
            this.Text = text;
        }
        public object Content
        {
            get
            {
                return this.Text;
            }
        }

        public object Description
        {
            get
            {
                return $"Описание к {Text}";
            }
        }

        public ImageSource Image
        {
            get
            {
                return null;
            }
        }

        public double Priority
        {
            get
            {
                return 1;
            }
        }

        public string Text { get; private set; }
      

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
