using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonPdfLib;

namespace Medo.Modules.PdfViewerModule
{
    interface IMoonPdfInjection
    {
        MoonPdfPanel MoonPanel { get; set; }
    }
}
