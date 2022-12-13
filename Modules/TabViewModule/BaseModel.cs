using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.TabViewModule
{
    public class BaseModel : INotifyPropertyChanged
    {
        [NonSerialized]
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
              
            }
        }








    }
}
