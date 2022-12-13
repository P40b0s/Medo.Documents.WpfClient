using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Medo.Modules.SelectBox.Behaviors
{
    class ListBoxBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
            this.AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
            this.AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is ListBox)
            {
                ListBox listBox = (sender as ListBox);
                listBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var data = listBox.DataContext as ViewIncrementSelectBox;
                    data.PopUpIsOpen = false;
                }));
            }
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
            {
                ListBox listBox = (sender as ListBox);
                var data = listBox.DataContext as ViewIncrementSelectBox;
                if (e.AddedItems.Count > 0)
                {
                    listBox.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        data.PopUpIsOpen = false;
                    }));
                }
               
            }
        }
    }
}
