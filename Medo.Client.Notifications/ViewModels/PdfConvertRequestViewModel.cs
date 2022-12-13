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
    public class PdfConvertRequestViewModel : PdfConverterModel, IInteractionRequestAware
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        IEventAggregator eventAggregator;
        public DelegateCommand OkCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand StartConvertCommand { get; set; }

        private PdfConverterModel notification;
        public PdfConvertRequestViewModel(IEventAggregator _event)
        {
            eventAggregator =  _event;
            CancelCommand = new DelegateCommand(Cancel);
            OkCommand = new DelegateCommand(Accepted,  CanSaveCard);
            StartConvertCommand = new DelegateCommand(StartConvert);
        }


        private async void StartConvert()
        {
            CanSave = false;
           bool ok = await (Notification as PdfConverterModel).ConvertPdf();
            if (ok)
                CanSave = true;
        }

        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                if (value is PdfConverterModel)
                {
                    this.notification = value as PdfConverterModel;
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
                (Notification as PdfConverterModel).doc.ConvertPdf = true;
                eventAggregator.GetEvent<DocumentSelectionChangedEvent>().Publish((Notification as PdfConverterModel).doc);
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
