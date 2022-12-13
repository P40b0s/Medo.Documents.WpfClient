using NLog;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Medo.Core.Models.ReportsSenderModel;
using Medo.Core.Interfaces;

namespace Medo.Client.Notifications
{
    public class UpdateDocumentsForIntervalRequestModel : Confirmation, INotifyPropertyChanged, IUpdateFromBaseModelInterface
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
        public UpdateDocumentsForIntervalRequestModel() { }
        public DateTime? dateFrom { get; set; }
        private DateTime? _dateTo { get; set; }
        public DateTime? dateTo
        {
            get
            {
                return _dateTo;
            }
            set
            {
                if (value.HasValue)
                    _dateTo = value.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

        }
    }
}
