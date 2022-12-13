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
    class UpdateDocumentInProcessToBoolConverter : IValueConverter
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public object Convert(object value, Type TypeTarget, object param, System.Globalization.CultureInfo cult)
        {
            try
            {
                if (param != null)
                {
                    Guid headerGuid = (Guid)param;
                    if (Client.Collections.StaticCollections.MainCollection.DocumentsInUpdateProcess.ContainsKey(headerGuid))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type TypeTarget, object param, System.Globalization.CultureInfo cult)
        {
            return value;
        }

    }
}
