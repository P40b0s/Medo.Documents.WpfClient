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
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Events;
using Medo.Core.EventsAggregator;

namespace Medo.Client.Notifications.ViewModels
{
    public class CardEditorRequestViewModel : IInteractionRequestAware, INotifyPropertyChanged
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
        IEventAggregator eventAggregator;
        public DelegateCommand OkCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        private CardEditorRequestModel notification;
        public CardEditorRequestViewModel(IEventAggregator _event)
        {
            eventAggregator =  _event;
            CancelCommand = new DelegateCommand(Cancel);
            OkCommand = new DelegateCommand(Accepted,  CanSaveCard);
            eventAggregator.GetEvent<UpdateICardEditorInterfaceEvent>().Subscribe(s => FinishInteraction());
        }



        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                if (value is CardEditorRequestModel)
                {
                    this.notification = value as CardEditorRequestModel;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool CanSaveCard()
        {
            return CanSave;
        }
        public bool CanSave
        {
            get { return _CanSave; }
            set
            {
                if (CanSave != value)
                {
                    _CanSave = value;
                    OkCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private bool _CanSave {get;set;}
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
                eventAggregator.GetEvent<CancelSaveCardEditorEvent>().Publish();
            }
            this.FinishInteraction();
        }
    }
}
