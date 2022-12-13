using Medo.Core.Models;
using System.IO;
using System.Threading.Tasks;

namespace Medo.Client.Notifications.Interfaces
{
    public interface IPdfConverterInterface
    {
        FileInfo PdfForConvert { get; set; }
        bool IsColor { get; set; }
        Task<bool> ConvertPdf();
        Document doc { get; set; }
    }
}
