using Medo.Modules.TrayInfoModule.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Medo.Modules.TrayInfoModule.Converters
{
    class ByteToImageConverter : IValueConverter
    {

        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public object Convert(object value, Type TypeTarget, object param, System.Globalization.CultureInfo cult)
        {
            try
            {
                if (value != null && value != DBNull.Value && Client.Collections.StaticCollections.Images != null)
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    var tmp = Client.Collections.StaticCollections.Images[Guid.Empty];
                    var image = Client.Collections.StaticCollections.Images[(Guid)value];
                    if (image != null)
                    {
                        MemoryStream ms = new MemoryStream(image);

                        bmp.StreamSource = new MemoryStream(ms.ToArray());
                        bmp.EndInit();
                        return bmp;
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(tmp);
                        bmp.StreamSource = new MemoryStream(ms.ToArray());
                        bmp.EndInit();
                        return bmp;
                    }

                }
                else
                {
                    throw new ArgumentNullException(string.Format("Для органа {0} не указано изображение", (Guid)value));
                }
            }
            catch (System.Exception ex)
            {
                //logger.Fatal(ex);
                return null;
            }
        }

        public object ConvertBack(object value, Type TypeTarget, object param, System.Globalization.CultureInfo cult)
        {
            return value;
        }

    }

}
