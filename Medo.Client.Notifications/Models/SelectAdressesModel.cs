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

namespace Medo.Client.Notifications
{
    public class SelectAdressesModel : Confirmation, INotifyPropertyChanged
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
        public SelectAdressesModel(){}
        public List<NotificationSubjectsModel> AdressList { get; set; }

    }
}
