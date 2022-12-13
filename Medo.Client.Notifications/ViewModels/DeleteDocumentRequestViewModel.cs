using Medo.Client.Notifications.Models;
using Medo.Core.Models;
using NLog;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client.Notifications.ViewModels
{
    public class DeleteDocumentRequestViewModel : IInteractionRequestAware, INotifyPropertyChanged
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

        private DeleteDocumentNotificationModel notification;
        public DeleteDocumentRequestViewModel()
        {
            CancelCommand = new DelegateCommand(Cancel);
            OkCommand = new DelegateCommand(Accepted);
        }
        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                if (value is DeleteDocumentNotificationModel)
                {
                    this.notification = value as DeleteDocumentNotificationModel;
                    this.OnPropertyChanged();
                }
              
            }
        }


        private void Accepted()
        {
            if (this.notification != null)
            {
                this.notification.Confirmed = true;                
            }
            this.FinishInteraction();
        }

        private void Cancel()
        {
            if (this.notification != null)
            {
                this.notification.Confirmed = false;
            }
            this.FinishInteraction();
        }

    }
}
