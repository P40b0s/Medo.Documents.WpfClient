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
using Prism.Regions;
using Prism.Events;

namespace Medo.Client.Notifications.ViewModels
{
    public class ClientUpdaterRequestViewModel : IInteractionRequestAware, INotifyPropertyChanged
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
        //IEventAggregator eventAggregator;

        private ClientUpdaterRequestModel notification;
        //public ClientUpdaterRequestViewModel(IEventAggregator _event)
        //{
        //    eventAggregator = _event;
        //    CancelCommand = new DelegateCommand(Cancel);
        //    OkCommand = new DelegateCommand(Accepted);
        //}
        public ClientUpdaterRequestViewModel()
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
                if (value is ClientUpdaterRequestModel)
                {
                    this.notification = value as ClientUpdaterRequestModel;
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
