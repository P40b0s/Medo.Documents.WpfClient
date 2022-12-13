using Medo.Core.Collections;
using Medo.Core.Models;
using NLog;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Medo.Client.Notifications.Interfaces;
using Prism.Regions;

namespace Medo.Client.Notifications.Models
{
    public class ClientUpdaterRequestModel : Confirmation, INotifyPropertyChanged, IClientUpdaterRequestInterface
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
      
        public ClientUpdaterRequestModel()
        {
           
        }
    }

}
