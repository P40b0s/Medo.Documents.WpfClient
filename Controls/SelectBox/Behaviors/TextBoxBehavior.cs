using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Medo.Core.Models;
using System.Windows.Input;

namespace Medo.Modules.SelectBox.Behaviors
{
    class TextBoxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            this.AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
            this.AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }

        private TraversalRequest moveFocus
        {
            get
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
                request.Wrapped = true;
                return request;
            }
        }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox textBox = (sender as TextBox);
                textBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var data = textBox.DataContext as ViewIncrementSelectBox;
                    //textBox.MoveFocus(moveFocus);
                    System.Windows.Point point = new System.Windows.Point();
                    point = e.GetPosition(textBox);
                    if (point.X < 0 || point.Y < 0 || point.X > textBox.ActualWidth)
                    {
                        data.PopUpIsOpen = false;
                    }
                }));
            }
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox textBox = (sender as TextBox);
                var data = textBox.DataContext as ViewIncrementSelectBox;
                IncrementSearch increment = new IncrementSearch();
                textBox.Dispatcher.BeginInvoke(new Action(() =>
                {             
                    if (data.ItemsCollection != null)
                    {
                        var fullcollection = ((IEnumerable<TextInlineSelection>)data.ItemsCollection);
                        increment.Search(textBox.Text, ref fullcollection);
                        var items = fullcollection.Where(v => v.Visible == true);
                        if(items.Count() > 1 && textBox.Text.Length > 1)
                        {
                            data.PopUpIsOpen = true;
                            if (items.FirstOrDefault().SelectedText.Length == items.FirstOrDefault().SourceText.Length)
                            {
                                data.SelectedItem = items.FirstOrDefault().SourceText;
                                data.PopUpIsOpen = false;
                                Keyboard.Focus(data.button);
                            }
                        }
                       
                        if (items.Count() == 1)
                        {
                            data.SelectedItem =  items.FirstOrDefault().SourceText;
                            data.PopUpIsOpen = false;
                            Keyboard.Focus(data.button);
                        }
                        if (items.Count() == 0)
                        {
                            data.PopUpIsOpen = false;
                        }
                    }
                    else
                    {
                        data.PopUpIsOpen = false;
                    }
                }));
            }
        }
    }
}
