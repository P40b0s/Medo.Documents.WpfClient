using System.Windows.Controls;
using MoonPdfLib;
using Medo.Modules.PdfViewerModule.ViewModels;

namespace Medo.Modules.PdfViewerModule.Views
{
    /// <summary>
    /// Логика взаимодействия для DocumentsViewer.xaml
    /// </summary>
    public partial class ViewContentViewer : UserControl, IMoonPdfInjection
    {
        public MoonPdfPanel MoonPanel { get; set; }
        public ViewContentViewer()
        {
            InitializeComponent();
            MoonPanel = MoonPdf;
            (DataContext as ViewContentViewerViewModel).Moon = this as IMoonPdfInjection;
        }
    }
}
