using NLog;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Shapes;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using AvallonEditTest;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using AvallonEditTest.Models;
using ICSharpCode.AvalonEdit.Editing;

namespace Medo.Modules.PdfViewerModule.Behaviors
{
    public class AvalonBehavior : Behavior<TextEditor>
    {

        readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected override void OnAttached()
        {
            base.OnAttached();

            base.AssociatedObject.Loaded += AssociatedObject_Loaded;
            base.AssociatedObject.TextArea.TextEntered += TextArea_TextEntered;
            base.AssociatedObject.TextArea.TextEntering += TextArea_TextEntering;
            base.AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
            
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.AssociatedObject.TextArea.TextEntered -= TextArea_TextEntered;
            base.AssociatedObject.TextArea.TextEntering -= TextArea_TextEntering;
            base.AssociatedObject.MouseDoubleClick -= AssociatedObject_MouseDoubleClick;
        }

        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextEditor editor = (TextEditor)sender;
            if (editor.SelectionLength > 0)
            {
                
                completionWindow = new CompletionWindow(editor.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new TextCompletionData("asdfasdfasdf"));
                completionWindow.Show();
                //completionWindow.Closed += delegate { completionWindow = null; };
            }
        }
        CompletionWindow completionWindow;
        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {

                TextArea area = (TextArea)sender;
                completionWindow = new CompletionWindow(area);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new TextCompletionData("asdfasdfasdf"));
                completionWindow.Show();
                completionWindow.Closed += delegate { completionWindow = null; };
            }
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var editor = (TextEditor)sender;
            editor.TextArea.TextView.LineTransformers.Add(new ColorizeAvalonEdit());
        }


    }
}
