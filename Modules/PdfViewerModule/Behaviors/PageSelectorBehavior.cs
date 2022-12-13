using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Medo.Modules.PdfViewerModule.ViewModels;

namespace Medo.Modules.PdfViewerModule.Behaviors
{
    class TextSelectorBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (sender as TextBox);
            var data = txt.DataContext as ViewContentViewerViewModel;
            txt.Dispatcher.BeginInvoke(new Action(() =>
            {
                txt.ToolTip = "";
                int i = 1;
                bool ok = int.TryParse(txt.Text, out i);
                if (!ok)
                {
                    txt.Clear();
                    txt.ToolTip = "Значение может быть только числом!";
                    e.Handled = false;
                }
                else
                {
                    if (i> data.TotalPages || i < 1)
                    {
                        txt.Clear();
                        txt.ToolTip = "Значение не может быть больше или меньше, чем количество страниц в документе!";
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                        data.Moon.MoonPanel.GotoPage(i);
                    }
                }
            }));
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }
    }
}
