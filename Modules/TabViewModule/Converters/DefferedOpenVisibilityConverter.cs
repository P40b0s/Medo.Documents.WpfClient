using Medo.Core.Collections;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Medo.Modules.TabViewModule.Converters
{
    class DefferedOpenVisibilityConverter : IValueConverter
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public object Convert(object value, Type TypeTarget, object param, System.Globalization.CultureInfo cult)
        {
            try
            {
                if (((DateTime?)value).HasValue)
                {
                    if (((DateTime?)value).Value.Date < DateTime.Now.Date)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                //logger.Fatal(ex);
                return false;
            }
        }

        public object ConvertBack(object value, Type TypeTarget, object param, System.Globalization.CultureInfo cult)
        {
            return value;
        }

    }
}
