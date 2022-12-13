using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Events;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Medo.Core.EventsAggregator;
using Prism.Commands;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;
using Medo.Core.Models;

namespace Medo.Modules.ClientUpdateModule.ViewModels
{
    class ViewClientUpdateModuleViewModel : INotifyPropertyChanged
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        IEventAggregator _aggregator;
        private System.Timers.Timer CheckTimer = new System.Timers.Timer();
        private Dispatcher disp = Dispatcher.CurrentDispatcher;
        BGFilesCopy BackgroundCopy = new BGFilesCopy();
        public ViewClientUpdateModuleViewModel(IEventAggregator eventAggregator)
        {
            this._aggregator = eventAggregator;
            currentVersionXml = new Client.Updater.Model.UpdaterModel();
            relaseVersionXml = new Client.Updater.Model.UpdaterModel();
            DeserializeVersions();
            CommandsInitialization();
            CheckTimer.Interval = 350000;
            CheckTimer.Enabled = true;
            CheckTimer.Elapsed += CheckTimer_Elapsed;
            CheckTimer.Start();
        }

        private void CheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            disp.BeginInvoke(new Action(() =>
            {
                DeserializeVersions();
            }));
        }

        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Команда выхода из подпрограммы центра обновления
        /// </summary>
        public DelegateCommand ExitCommand { get; set; }
        /// <summary>
        /// Команда выхода из подпрограммы центра обновления
        /// </summary>
        public DelegateCommand UpdateCommand { get; set; }


        private void CommandsInitialization()
        {
            ExitCommand = new DelegateCommand(() => _aggregator.GetEvent<UpdaterWindowIsOpenEvent>().Publish());
            UpdateCommand = new DelegateCommand(StartUpdate);
        }

        #endregion
        private bool _IsCriticalUpdate { get; set; }
        public bool IsCriticalUpdate
        {
            get
            {
                return this._IsCriticalUpdate;
            }
            set
            {
                if (this.IsCriticalUpdate != value)
                {
                    needNotificated = false;
                    this._IsCriticalUpdate = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _relaseVersion { get; set; }
        public string relaseVersion
        {
            get
            {
                return this._relaseVersion;
            }
            set
            {
                if (this.relaseVersion != value)
                {
                    this._relaseVersion = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _relaseVersionDateTime { get; set; }
        public string relaseVersionDateTime
        {
            get
            {
                return this._relaseVersionDateTime;
            }
            set
            {
                if (this.relaseVersionDateTime != value)
                {
                    this._relaseVersionDateTime = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _currentVersion { get; set; }
        public string currentVersion
        {
            get
            {
                return this._currentVersion;
            }
            set
            {
                if (this.currentVersion != value)
                {
                    this._currentVersion = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _currentVersionDateTime { get; set; }
        public string currentVersionDateTime
        {
            get
            {
                return this._currentVersionDateTime;
            }
            set
            {
                if (this.currentVersionDateTime != value)
                {
                    this._currentVersionDateTime = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _currentVersionHistory { get; set; }
        public string currentVersionHistory
        {
            get
            {
                return this._currentVersionHistory;
            }
            set
            {
                if (this.currentVersionHistory != value)
                {
                    this._currentVersionHistory = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsNeedUpdate { get; set; }
        public bool IsNeedUpdate
        {
            get
            {
                return this._IsNeedUpdate;
            }
            set
            {
                if (this.IsNeedUpdate != value)
                {
                    needNotificated = true;
                    this._IsNeedUpdate = value;
                    this.OnPropertyChanged();                
                    _aggregator.GetEvent<UpdateExistsEvent>().Publish(value);
                }
            }
        }

        private SolidColorBrush _currentVersionForeground { get; set; }
        public SolidColorBrush currentVersionForeground
        {
            get
            {
                return this._currentVersionForeground;
            }
            set
            {
                if (this.currentVersionForeground != value)
                {
                    this._currentVersionForeground = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool needNotificated = false;

        #region Десериализованные классы
        private Client.Updater.Model.UpdaterModel _currentVersionXml { get; set; }
        public Client.Updater.Model.UpdaterModel currentVersionXml
        {
            get
            {
                return this._currentVersionXml;
            }
            set
            {
                if (this.currentVersionXml != value)
                {
                    this._currentVersionXml = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private Client.Updater.Model.UpdaterModel _relaseVersionXml { get; set; }
        public Client.Updater.Model.UpdaterModel relaseVersionXml
        {
            get
            {
                return this._relaseVersionXml;
            }
            set
            {
                if (this.relaseVersionXml != value)
                {
                    this._relaseVersionXml = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Client.Updater.Model.UpdaterModel updaterVersionXml { get; set; }
        #endregion


        private void StartUpdate()
        {          
            _aggregator.GetEvent<StartUpdateEvent>().Publish();
        }

        private void DeserializeVersions()
        {
            try
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Client.Updater.Model.UpdaterModel));
                using (TextReader reader = new StreamReader(Path.Combine(currentVersionXml.UpdatePath, "version.xml")))
                {
                    relaseVersionXml = (Client.Updater.Model.UpdaterModel)ser.Deserialize(reader);
                }
                using (TextReader reader = new StreamReader("version.xml"))
                {
                    currentVersionXml = (Client.Updater.Model.UpdaterModel)ser.Deserialize(reader);
                }               
                getVersionsFields();
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private bool relaseVersionIsNewestThanCurrentVersion(string curversion, string relversion)
        {
            try
            {
                if (!string.IsNullOrEmpty(curversion) && !string.IsNullOrEmpty(relversion))
                {

                    double rvdouble = double.Parse(relaseVersion.Replace(".", ""));
                    double cvdouble = double.Parse(currentVersion.Replace(".", ""));
                    if (rvdouble > cvdouble)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
                return false;
            }
        }
        private void getVersionsFields()
        {
            try
            {
                currentVersion = currentVersionXml.Version;
                currentVersionDateTime = currentVersionXml.UpdateTime.ToString("dd.MM.yyyy");
                relaseVersion = relaseVersionXml.Version;
                relaseVersionDateTime = relaseVersionXml.UpdateTime.ToString("dd.MM.yyyy");
                IsCriticalUpdate = relaseVersionXml.IsCriticalUpdate;
                currentVersionHistory = relaseVersionXml.VersionHistory;
                if (relaseVersionIsNewestThanCurrentVersion(currentVersion, relaseVersion))
                {
                    currentVersionForeground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFF5754F");
                    IsNeedUpdate = true;
                    if (needNotificated)
                    {
                        NotificationModel notify = new NotificationModel();
                        if (IsCriticalUpdate)
                        {
                            notify.Name = "Получены критические обновления!";
                            notify.Notification = string.Format("Получены критические обновления системы версии {0} советуем немедленно их установить через центр обновлений", relaseVersion);
                            notify.Error = true;
                        }
                        else
                        {
                            notify.Name = "Получены обновления!";
                            notify.Notification = string.Format("Получены обновления системы версии {0} их можно установить через центр обновлений, или они установятся атоматически после перезапуска программы", relaseVersion);
                            notify.Error = false;
                        }
                        _aggregator.GetEvent<NewNotificationEvent>().Publish(notify);
                        needNotificated = false;
                    }
                }
                else
                {
                    currentVersionForeground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF66AE2A");
                    IsNeedUpdate = false;
                }
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
    }
}
