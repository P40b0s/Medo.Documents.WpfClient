using NLog;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Medo.Client.Notifications.Models;

namespace Medo.Client.Notifications.ViewModels
{
    public class ChangeDocumentRequestViewModel : IInteractionRequestAware, INotifyPropertyChanged
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public DelegateCommand OkCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        private ChangeDocumentNotificationModel notification;
        public ChangeDocumentRequestViewModel()
        {
            CancelCommand = new DelegateCommand(Cancel);
            OkCommand = new DelegateCommand(Accepted, ()=> flashSelected);         
        }
        private bool flashSelected { get; set; }

        private void Notification_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedFlashDisk")
            {
                if (this.notification.SelectedFlashDisk != null)
                {
                    flashSelected = true;
                }
                else
                {
                    flashSelected = false;
                }
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                if (value is ChangeDocumentNotificationModel)
                {
                    this.notification = value as ChangeDocumentNotificationModel;
                    this.OnPropertyChanged();
                    if (value != null)
                    {
                        this.notification.PropertyChanged -= Notification_PropertyChanged;
                        this.notification.PropertyChanged += Notification_PropertyChanged;
                        
                    }
                      

                }

            }
        }

        private void Accepted()
        {
            if (this.notification != null)
            {
                this.notification.Confirmed = true;
                this.notification.flashTimer.Stop();
            }
            this.FinishInteraction();
        }

        private void Cancel()
        {
            if (this.notification != null)
            {
                this.notification.Confirmed = false;
                this.notification.flashTimer.Stop();
            }
            this.FinishInteraction();
        }
    }
}
