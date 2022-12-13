using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Client
{
    class DllsPaths
    {     
        public const string UpdatePath = "\\\\182.5.202.220\\Софт для работы\\МЭДО 2.0\\";
        /// <summary>
        /// Библиотеки Prism
        /// </summary>
        public struct PrismPath
        {
            public const string Prism = "Prism\\Prism.dll";
            public const string Prism_Wpf = "Prism\\Prism.Wpf.dll";
            public const string Prism_Unity_Wpf = "Prism\\Prism.Unity.Wpf.dll";
            public const string Microsoft_Practices_ServiceLocation = "Prism\\Microsoft.Practices.ServiceLocation.dll";
            public const string Microsoft_Practices_Unity = "Prism\\Microsoft.Practices.Unity.dll";
        }
        /// <summary>
        /// Библиотеки модулей программы
        /// </summary>
        public struct ModulesPath
        {
            public const string CardEditorModule =  "Modules\\CardEditorModule.dll";
            public const string DocumentsUploaderModule =  "Modules\\DocumentsUploaderModule.dll";
            public const string ListViewModule =  "Modules\\ListViewModule.dll";
            public const string PdfViewerModule =  "Modules\\PdfViewerModule.dll";
            public const string TabViewModule =  "Modules\\TabViewModule.dll";
            public const string TrayInfoModule =  "Modules\\TrayInfoModule.dll";
            public const string WcfModule =  "Modules\\WcfModule.dll";
        }

        /// <summary>
        /// Контролы программы
        /// </summary>
        public struct ControlsPath
        {
            public const string PdfRecognitionViewer =  "Controls\\PdfRecognitionViewer.dll";
            public const string RectangleCanvas =  "Controls\\RectangleCanvas.dll";
            public const string SelectBox =  "Controls\\SelectBox.dll";
            public const string TextEditor =  "Controls\\TextEditor.dll";
        }

        /// <summary>
        /// Библиотеки ядра программы
        /// </summary>
        public struct CorePath
        {
            public const string Hardcodet_Wpf_TaskbarNotification =  "Core\\Hardcodet.Wpf.TaskbarNotification.dll";
            public const string LevenshteinAutomaton =  "Core\\LevenshteinAutomaton.dll";
            public const string Medo_Core =  "Core\\Medo.Core.dll";
            public const string Medo_Helpers = "Core\\Medo.Helpers.dll";
            public const string Medo_ImageResources =  "Core\\Medo.ImageResources.dll";
            public const string Swordfish_NET_General =  "Core\\Swordfish.NET.General.dll";
            public const string System_Windows_Interactivity =  "Core\\System.Windows.Interactivity.dll";
            public const string XmlCardCreator =  "Core\\XmlCardCreator.dll";
            public const string Medo_Client_Collections = "Core\\Medo.Client.Collections.dll";
        }

        /// <summary>
        /// Библиотеки EmguCV
        /// </summary>
        public struct EmguCVPath
        {
            public const string Emgu_CV =  "EmguCV\\Emgu.CV.dll";
            public const string Emgu_Util =  "EmguCV\\Emgu.Util.dll";
        }

        /// <summary>
        /// Компонент для просмотра PDF
        /// </summary>
        public struct PdfViewerPath
        {
            public const string libmupdf =  "Pdf\\libmupdf.dll";
            public const string MouseKeyboardActivityMonitor =  "Pdf\\MouseKeyboardActivityMonitor.dll";
            public const string MoonPdfLib =  "Pdf\\MoonPdfLib.dll";
        }

        /// <summary>
        /// Компоненты Metro
        /// </summary>
        public struct MetroPath
        {
            public const string MahApps_Metro = "Metro\\MahApps.Metro.dll";
            public const string MahApps_Metro_SimpleChildWindow = "Metro\\MahApps.Metro.SimpleChildWindow.dll";
        }
    }
}
