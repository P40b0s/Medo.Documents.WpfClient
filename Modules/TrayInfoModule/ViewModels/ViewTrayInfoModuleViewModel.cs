using System;
using Prism.Mvvm;
using NLog;
using Hardcodet.Wpf.TaskbarNotification;
using System.Media;
using System.Windows.Threading;
using Medo.Core.Models;
using Prism.Events;
using Medo.Core.EventsAggregator;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Collections.ObjectModel;
using Medo.Modules.TrayInfoModule.Views;
using System.Collections.Generic;

namespace Medo.Modules.TrayInfoModule.ViewModels
{
    class ViewTrayInfoModuleViewModel : BindableBase
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        IEventAggregator _aggregator;
        private SubscriptionToken ConnectToken;

        private SoundPlayer player = new SoundPlayer();
        private byte[] PrezSound { get; set; }
        private byte[] OtherSound { get; set; }

        private DispatcherTimer notificationCleared = new DispatcherTimer();
        public ObservableCollection<Document> NotificationsCollection { get; set; }
        private string ArrivedDocs { get; set; }
        //private NotificationControl control { get; set; }
        public TaskbarIcon notificationIcon = new TaskbarIcon();

        private Uri connectedIcon = new Uri("pack://application:,,,/TrayInfoModule;component/Icons/connected.ico");
        private Uri disconnectedIcon = new Uri("pack://application:,,,/TrayInfoModule;component/Icons/disconnected.ico");
        private Uri programIcon = new Uri("pack://application:,,,/TrayInfoModule;component/Icons/programicon.ico");

        public ViewTrayInfoModuleViewModel(IEventAggregator eventAggregator)
        {
            _aggregator = eventAggregator;
            ConnectToken = _aggregator.GetEvent<TrayConnectedEvent>().Subscribe(setIcon);
            NotificationsCollection = new ObservableCollection<Document>();
            notificationCleared.Interval = new TimeSpan(0, 0, 25);
            notificationCleared.Tick += notificationCleared_Tick;
            notificationCleared.IsEnabled = true;
            _aggregator.GetEvent<NewDocumentNotificationEvent>().Subscribe(ShowDocumentArrivalNotification);
            _aggregator.GetEvent<NewNotificationEvent>().Subscribe(ShowNotification);
            getStartUpArgs();
            logger.Info("Модуль TrayInfoModule загружен");
        }

        private void getStartUpArgs()
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                foreach (string s in args)
                {
                    logger.Info(s);
                    if (s.Contains("-updateok"))
                    {
                        NotificationModel nm = new NotificationModel();
                        nm.Name = "Программа обновлена до последней версии";
                        nm.Notification = string.Format("Обновление программы до версии {0} успешно завершено", s.Replace("-updateok", ""));
                        nm.Error = false;
                        ShowNotification(nm);
                        logger.Info(nm.Notification);
                    }
                    if (s.Contains("-updatefalse"))
                    {
                        NotificationModel nm = new NotificationModel();
                        nm.Name = "Ошибка обновления программы";
                        nm.Notification = "Произошла ошибка при обновлении программы, подробности ошибки можно посмотреть в лог файле.";
                        nm.Error = true;
                        ShowNotification(nm);
                        logger.Info(nm.Notification);
                    }

                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private void ShowDocumentArrivalNotification(Document doc)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                notificationCleared.Stop();
                if (doc != null)
                {
                    NotificationsCollection.Insert(0, doc);
                    notificationCleared.Start();
                    notificationIcon.ShowBalloonTip(String.Format("Поступило {0} новых документов", NotificationsCollection.Count), GetString(doc), BalloonIcon.Info);
                }
            }));
        }
        private void ShowNotification(NotificationModel not)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!string.IsNullOrEmpty(not.Name) && !string.IsNullOrEmpty(not.Notification))
                    {
                        BalloonIcon b = new BalloonIcon();
                        if (not.Error)
                        {
                            b = BalloonIcon.Error;
                        }
                        else
                        {
                            b = BalloonIcon.Info;
                        }
                        notificationIcon.ShowBalloonTip(not.Name, not.Notification, b);
                    }
                }));
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }



        string GetString(Document d)
        {
            string s = string.Empty;
            if (d.SourceGuid == new Guid(Helpers.SourceGuidOrgansNames.Минюст))
            {
                s = String.Format("{0} {1} {2} зарегистрирован {3}",
                    d.ActType,
                    d.OrganName,
                    d.DocumentNumber,
                    d.MJDate.HasValue ? d.MJDate.Value.ToString("dd.MM.yyyy") : null)
                    + Environment.NewLine +
                    string.Format("Всего документов: {0}", Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItemsCount);

            }
            else
            {
                s = String.Format("{0} {1} {2} от {3}",
                    d.ActType,
                    d.OrganName,
                    d.DocumentNumber,
                    d.SignDate.HasValue ? d.SignDate.Value.ToString("dd.MM.yyyy") : null)
                    + Environment.NewLine +
                     string.Format("Всего документов: {0}", Client.Collections.StaticCollections.MainCollection.ActiveFilters.FilteredItemsCount);

            }
            return s;
        }

        void notificationCleared_Tick(object sender, EventArgs e)
        {
            NotificationsCollection.Clear();
            ArrivedDocs = string.Empty;
            notificationCleared.Stop();
        }
        private void setIcon(bool connectIcon)
        {
            try
            {
                if (connectIcon)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        notificationIcon.ToolTipText = "Модуль синхронизирован с сервисом МЭДО";
                        //notificationIcon.ShowBalloonTip("Синхронизация успешна", "Модуль синхронизирован с сервисом МЭДО", BalloonIcon.Info);
                        using (var stream = Application.GetResourceStream(programIcon).Stream)
                        {
                            notificationIcon.Icon = new System.Drawing.Icon(stream);
                        }
                        logger.Info("Приложение успешно синхронизированно с сервисом WCF");
                    }));
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        notificationIcon.ToolTipText = "Ошибка синхронизации модуля, ожидаю подключения....";
                        notificationIcon.ShowBalloonTip("Ошибка синхронизации", "Ожидание повторного подключения...", BalloonIcon.Error);
                        using (var stream = Application.GetResourceStream(disconnectedIcon).Stream)
                        {
                            notificationIcon.Icon = new System.Drawing.Icon(stream);
                        }
                        logger.Info("Ошибка синхронизации с сервисом WCF");
                    }));
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

    }
}
