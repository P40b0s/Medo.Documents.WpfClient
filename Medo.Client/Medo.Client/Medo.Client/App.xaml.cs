using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Medo.Client
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        AppDomain currentDomain = AppDomain.CurrentDomain;
        //public static AppDomain dom = AppDomain.CreateDomain("Modules");
        private string BaseDir;
        private const string UpdatePath = "\\\\182.5.202.220\\Софт для работы\\МЭДО 2.0\\";

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                BaseDir = currentDomain.BaseDirectory;
                App.Current.Exit += Current_Exit;
                base.OnStartup(e);
                logger.Info("Подключение PAK: " + Helpers.UserSecurity.connectToRemote(Helpers.Paths.PakMedoFolder, Helpers.Logins.PakLogin, Helpers.Logins.PakPassword));

                logger.Info("Подключение Издание импорт: " + Helpers.UserSecurity.connectToRemote(Helpers.Paths.IzdanieMedoFolder, Helpers.Logins.IzdanieLogin, Helpers.Logins.IzdaniePassword));
                logger.Info("Подключение директории для Системы: " + Helpers.UserSecurity.connectToRemote(Helpers.Paths.Systema, Helpers.Logins.SystemaLogin, Helpers.Logins.SystemaPassword));
                logger.Info("Подключение резервного сервера: " + Helpers.UserSecurity.connectToRemote("\\\\182.5.202.219\\MedoServices", Helpers.Logins.ReserveAdmLogin, Helpers.Logins.ReserveAdmPassword));
                Bootstrapper bs = new Bootstrapper();
                bs.Run();
                logger.Info("Модули загружены");
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                if (e.ApplicationExitCode == 1)
                {
                    logger.Info("Программа перезагружена для обновления");
                }

            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                e.ApplicationExitCode = 2;
            }

        }


        //private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    try
        //    {
        //        string dll = args.Name;
        //        string dllName = args.Name.Split(',')[0];
        //        var baseDir = currentDomain.BaseDirectory;
        //        string folderPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //        string assebly = new AssemblyName(args.Name).Name;
        //        if (assebly.EndsWith("resources"))
        //        {
        //            int startindex = assebly.IndexOf("resources");
        //            assebly = assebly.Remove(startindex, "resources".Length) + "dll";
        //        }
        //        string assemblyPath = System.IO.Path.Combine(folderPath, assebly);
        //        switch (dllName)
        //        {
        //            default:
        //                {
        //                    throw new NotImplementedException();
        //                }
        //            //Prism
        //            case "Prism.Unity.Wpf.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.PrismPath.Prism_Unity_Wpf);
        //                }
        //            case "Prism.Wpf.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.PrismPath.Prism_Wpf);
        //                }
        //            case "Prism.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.PrismPath.Prism);
        //                }
        //            case "Microsoft_Practices_ServiceLocation.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.PrismPath.Microsoft_Practices_ServiceLocation);
        //                }
        //            case "Microsoft_Practices_Unity.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.PrismPath.Microsoft_Practices_Unity);
        //                }
        //            //Модули
        //            case "CardEditorModule.resources":
        //                {
        //                    dom.Load(Assembly.LoadFile(baseDir + DllsPaths.ModulesPath.CardEditorModule).FullName);
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ModulesPath.CardEditorModule);
        //                }
        //            case "TrayInfoModule.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ModulesPath.TrayInfoModule);
        //                }
        //            case "DocumentsUploaderModule.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ModulesPath.DocumentsUploaderModule);
        //                }
        //            case "ListViewModule.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ModulesPath.ListViewModule);
        //                }
        //            case "PdfViewerModule.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ModulesPath.PdfViewerModule);
        //                }
        //            case "TabViewModule.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ModulesPath.TabViewModule);
        //                }
        //            case "WcfModule.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ModulesPath.WcfModule);
        //                }
        //            //Контролы
        //            case "PdfRecognitionViewer.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ControlsPath.PdfRecognitionViewer);
        //                }
        //            case "RectangleCanvas.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ControlsPath.RectangleCanvas);
        //                }
        //            case "SelectBox.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ControlsPath.SelectBox);
        //                }
        //            case "TextEditor.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.ControlsPath.TextEditor);
        //                }

        //            //Ядро
        //            case "Hardcodet.Wpf.TaskbarNotification.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.CorePath.Hardcodet_Wpf_TaskbarNotification);
        //                }
        //            case "LevenshteinAutomaton.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.CorePath.LevenshteinAutomaton);
        //                }
        //            case "Medo.Core.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.CorePath.Medo_Core);
        //                }
        //            case "Medo.Helpers.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.CorePath.Medo_Helpers);
        //                }
        //            case "Medo.ImageResources.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.CorePath.Medo_ImageResources);
        //                }
        //            case "Swordfish.NET.General.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.CorePath.Swordfish_NET_General);
        //                }
        //            case "System.Windows.Interactivity.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.CorePath.System_Windows_Interactivity);
        //                }
        //            case "XmlCardCreator.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.CorePath.XmlCardCreator);
        //                }
        //            case "Medo.Client.Collections.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.CorePath.Medo_Client_Collections);
        //                }
        //            //EmguCv
        //            case "Emgu.CV.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.EmguCVPath.Emgu_CV);
        //                }
        //            case "Emgu.Util.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.EmguCVPath.Emgu_Util);
        //                }
        //            //PdfViwer
        //            case "libmupdf.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.PdfViewerPath.libmupdf);
        //                }
        //            case "MoonPdfLib.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.PdfViewerPath.MoonPdfLib);
        //                }
        //            case "MouseKeyboardActivityMonitor.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.PdfViewerPath.MouseKeyboardActivityMonitor);
        //                }
        //            //Metro
        //            case "MahApps.Metro.resources":
        //                {
        //                    //Assembly.Load(System.IO.File.ReadAllBytes(baseDir + DllsPaths.MetroPath.MahApps_Metro));                         
        //                    return Assembly.Load(System.IO.File.ReadAllBytes(baseDir + DllsPaths.MetroPath.MahApps_Metro));

        //                }
        //            case "MahApps.Metro.SimpleChildWindow.resources":
        //                {
        //                    return Assembly.LoadFile(baseDir + DllsPaths.MetroPath.MahApps_Metro_SimpleChildWindow);
        //                }

        //        }
        //    }            
        //    catch (System.Exception ex)
        //    {
        //        logger.Fatal(ex);
        //        throw new NotImplementedException();
        //    }
        //}
    }
}
