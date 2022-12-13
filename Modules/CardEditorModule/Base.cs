using Medo.Core.Collections;
using Medo.Core.Enums;
using Medo.Core.EventsAggregator;
using Medo.Core.Models;
using NLog;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.CardEditorModule
{
    public abstract class Base : INotifyPropertyChanged
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        IEventAggregator eventAggregator;
        public Base(IEventAggregator _eventAggregator) { eventAggregator = _eventAggregator; }
        #region PropertyChanged realization
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
       

        public Base()
        {
            try
            {
              
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

    }
}
