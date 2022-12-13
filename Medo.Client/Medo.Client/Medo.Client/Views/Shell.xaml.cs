using MahApps.Metro.Controls;
using Medo.Core.EventsAggregator;
using Prism.Commands;
using Prism.Events;
using System.Reflection;
using System.IO;
using NLog;
using System.Diagnostics;
using System;

namespace Medo.Client.Views
{
    /// <summary>
    /// Логика взаимодействия для Shell.xaml
    /// </summary>
    public partial class Shell : MetroWindow
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        private string LaunchDir { get; set; }
        IEventAggregator _eventAggregator { get; set; }
        public Shell(IEventAggregator eventAggregator)
        {
            try
            {
                _eventAggregator = eventAggregator;
                InitializeComponent();
                Assembly exe = Assembly.Load(File.ReadAllBytes("Medo.Client.exe"));
                LaunchDir = new Uri(exe.CodeBase).LocalPath.Replace("Medo.Client.exe", null);
                this.Title = string.Format("Система предварительной обработки документов \"Звено\" {0}", exe.FullName.Split(',')[1].Replace("Version=", ""));
                exe = null;
                eventAggregator.GetEvent<StartUpdateEvent>().Subscribe(shutdown);
                eventAggregator.GetEvent<PdfViewerIsOpenEvent>().Subscribe(() => DocFlyOut.IsOpen = !DocFlyOut.IsOpen);
                eventAggregator.GetEvent<SelectFileEvent>().Subscribe(s => DocFlyOut.Header = $"Выбран файл: {s.File.Name} - {s.File.Length / 1000} Кб.");
            
               
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
            GlobalCommands.Commands.InitializeCommands(eventAggregator);
            Collections.StaticCollections.InitializeStaticCollections(eventAggregator);
        }

        public void shutdown()
        {
            try
            {
                Process updater = new Process();
                updater.StartInfo.FileName = "Medo.Client.Updater.exe";
                updater.StartInfo.Arguments = "-client";
                updater.StartInfo.WorkingDirectory = LaunchDir + "Updater\\";
                updater.Start();
                App.Current.Shutdown(1);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
    }
}
