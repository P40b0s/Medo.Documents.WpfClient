using Prism.Interactivity;
using System.Windows;
using System.Windows.Media;
using Prism.Interactivity.InteractionRequest;

namespace Medo.Client.Notifications
{
    class MessageContainer : PopupWindowAction
    {
      
        /// <summary>
        /// The icon of the child window that is displayed as part of the popup.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
           DependencyProperty.Register(
            "Icon",
            typeof(ImageSource),
            typeof(MessageContainer),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Icon of the window.
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Returns the window to display as part of the trigger action.
        /// </summary>
        /// <param name="notification">The notification to be set as a DataContext in the window.</param>
        /// <returns>
        /// The popup window
        /// </returns>
        protected override Window GetWindow(INotification notification)
        {
            Window wrapperWindow;

            if (this.WindowContent != null)
            {
                wrapperWindow = new Window();

                // If the WindowContent does not have its own DataContext, it will inherit this one.
                wrapperWindow.DataContext = notification;

                if (string.IsNullOrEmpty(notification.Title))
                {
                    wrapperWindow.Title = string.Empty;
                }
                else
                {
                    wrapperWindow.Title = notification.Title;
                }              
                wrapperWindow.Icon = this.Icon;

                this.PrepareContentForWindow(notification, wrapperWindow);
            }
            else
            {
                wrapperWindow = this.CreateDefaultWindow(notification);
                wrapperWindow.Icon = this.Icon;
            }

            return wrapperWindow;
        }

    }
}
