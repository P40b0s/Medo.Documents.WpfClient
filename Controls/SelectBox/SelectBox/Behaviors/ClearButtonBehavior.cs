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
    class ClearButtonBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.Click += AssociatedObject_Click;

        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.Click -= AssociatedObject_Click;

        }

        private void AssociatedObject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = (sender as Button);
            button.Dispatcher.BeginInvoke(new Action(() =>
            {               
                var data = button.DataContext as IncrementSelectBox;
                Keyboard.Focus(data.txt);
            }));
        }

      

        private TraversalRequest moveFocus
        {
            get
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                return request;
            }
        }
    }
}
