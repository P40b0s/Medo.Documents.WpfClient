using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCardCreator
{
    public sealed class MyDateTime
    {
        public enum DataFormat { XmlDateTime, XmlDate }
        public static string DateTime(DateTime dat, DataFormat format)
        {
            switch (format)
            {
                default:
                    {
                        return string.Empty;
                    }
                case DataFormat.XmlDate:
                    {
                        return dat.ToString("yyyy-MM-dd");
                    }
                case DataFormat.XmlDateTime:
                    {
                        return dat.ToString("yyyy-MM-ddTHH:mm:ss");

                    }

            }

        }

    }
}
