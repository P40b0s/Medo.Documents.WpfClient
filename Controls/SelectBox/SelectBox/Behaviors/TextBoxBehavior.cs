using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Medo.Core.Models;
using System.Windows.Input;

namespace Medo.Controls.SelectBox.Behaviors
{
    class TextBoxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            this.AssociatedObject.GotFocus += AssociatedObject_GotFocus;
            this.AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
            this.AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
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
                    var data = textBox.DataContext as IncrementSelectBox;
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

        private void AssociatedObject_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox textBox = (sender as TextBox);
                textBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //var data = textBox.DataContext as IncrementSelectBox;
                    //data.PopUpIsOpen = true;
                }));
            }
        }
      
        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox textBox = (sender as TextBox);
                var data = textBox.DataContext as IncrementSelectBox;
                IncrementSearch increment = new IncrementSearch();
                textBox.Dispatcher.BeginInvoke(new Action(() =>
                {             
                    if (data.Collection != null)
                    {
                        data.PopUpIsOpen = true;
                        increment.Search(textBox.Text, ((IEnumerable<TextInlineSelection>)data.Collection));
                        var item = ((IEnumerable<TextInlineSelection>)data.Collection).Where(v => v.Visible == true);

                        if (item.Count() == 1)
                        {
                            data.SelectedItem = item.FirstOrDefault();
                            data.PopUpIsOpen = false;
                            Keyboard.Focus(data.button);
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
