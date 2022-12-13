using Medo.Core.Models;
using NLog;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Medo.Client.Notifications.Interfaces;

namespace Medo.Client.Notifications.Models
{
    public class DeleteDocumentNotificationModel : Confirmation, INotifyPropertyChanged, DocumentOperationInterface
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

        public DeleteDocumentNotificationModel()
        {
            RegisterDocumentInSED = true;
            RejectStatuses = new List<string>();
            RejectStatuses.Add("Ошибка адресации");
            RejectStatuses.Add("Несоотвествие вложения (PDF) техническийм требованиям");
            RejectStatuses.Add("Неверно заполнена XML карточка документа");
            RejectStatuses.Add("Данный вид документов не подлежит опубликованию");
            RejectStatuses.Add("Документ прислан повторно");
            RejectStatus = RejectStatuses[3];
        }
    
        private bool _RegisterDocumentInSED { get; set; }
        public bool RegisterDocumentInSED
        {
            get
            {
                return this._RegisterDocumentInSED;
            }
            set
            {
                if (this.RegisterDocumentInSED != value)
                {
                    this._RegisterDocumentInSED = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _DeleteDocumentFromSED { get; set; }
        public bool DeleteDocumentFromSED
        {
            get
            {
                return this._DeleteDocumentFromSED;
            }
            set
            {
                if (this.DeleteDocumentFromSED != value)
                {
                    this._DeleteDocumentFromSED = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private bool _RejectRegistrationInSED { get; set; }
        public bool RejectRegistrationInSED
        {
            get
            {
                return this._RejectRegistrationInSED;
            }
            set
            {
                if (this.RejectRegistrationInSED != value)
                {
                    this._RejectRegistrationInSED = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private List<string> _RejectStatuses { get; set; }
        public List<string> RejectStatuses
        {
            get
            {
                return this._RejectStatuses;
            }
            set
            {
                if (this.RejectStatuses != value)
                {
                    this._RejectStatuses = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private string _RejectStatus { get; set; }
        public string RejectStatus
        {
            get
            {
                return this._RejectStatus;
            }
            set
            {
                if (this.RejectStatus != value)
                {
                    this._RejectStatus = value;
                    this.OnPropertyChanged();

                }
            }
        }



        private Document _OperationDocument { get; set; }
        public Document OperationDocument
        {
            get
            {
                return this._OperationDocument;
            }
            set
            {
                if (this.OperationDocument != value)
                {
                    this._OperationDocument = value;
                    this.OnPropertyChanged();

                }
            }
        }
    }
}
