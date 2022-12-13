using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System.ComponentModel;
using NLog;
using System.Runtime.CompilerServices;
using Medo.Client.Notifications.Models;
using Medo.Core.Models;

namespace Medo.Client.Notifications.ViewModels
{
    public class UpdateDocumentsForIntervalRequestViewModel : IInteractionRequestAware, INotifyPropertyChanged
    {
        private UpdateDocumentsForIntervalRequestModel notification;
        public UpdateDocumentsForIntervalRequestViewModel()
        {
            InitializeCommand();
        }

        #region NotifyProperty
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region  Команды
        public DelegateCommand OkCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
       
        private void InitializeCommand()
        {
            CancelCommand = new DelegateCommand(Cancel);
            OkCommand = new DelegateCommand(Accepted);
        }
        #endregion
        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get { return this.notification; }
            set
            {
                if (value is UpdateDocumentsForIntervalRequestModel)
                {
                    this.notification = value as UpdateDocumentsForIntervalRequestModel;
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
