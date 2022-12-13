using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Helpers
{
    public static class ConnectionsStrings
    {
        public const string ReserveServerIzdanieConnectionString = @"Data Source=" + "182.5.202.219" + @";
            Initial Catalog=Izdanie;
            Persist Security Info=True;
            User ID=xphobos;
            Password=iksar33664;
            connection timeout = 30;";
        public const string ReserveServerAutoReportsConnectionString = @"Data Source=" + "182.5.202.219" + @";
            Initial Catalog=AutoReports;
            Persist Security Info=True;
            User ID=xphobos;
            Password=iksar33664;
            connection timeout = 30;";

        public const string AutoReportsConnectionString = @"Data Source=" + "182.5.202.220" + @";
            Initial Catalog=AutoReports;
            Persist Security Info=True;
            User ID=phobos;
            Password=iksar;
            connection timeout = 30;";

        public const string IzdanieConnectionString = @"Data Source=" + "182.5.202.220" + @";
            Initial Catalog=Izdanie;
            Persist Security Info=True;
            User ID=phobos;
            Password=iksar;
            connection timeout = 30;";

        public const string PAKConnectionString = @"Data Source=" + "182.5.202.16" + @";
            Initial Catalog=clean;
            Persist Security Info=True;
            User ID=gadmin;
            Password=Sa1Sa2Sa3!;
            connection timeout = 30;";

        public const string AutoReportsConnectionStringLocal = "Data Source=(local);Initial Catalog=AutoReports;User ID=phobos;Password=iksar";

        public const string IzdanieConnectionStringLocal = "Data Source=(local);Initial Catalog=Izdanie;User ID=phobos;Password=iksar";
    }
}
