using Medo.Core.Collections;
using Medo.Core.Models;
using NLog;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Medo.Client.Notifications.Interfaces;

namespace Medo.Client.Notifications.Models
{
    public class ChangeDocumentNotificationModel : Confirmation, INotifyPropertyChanged, DocumentOperationInterface
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ChangeDocumentNotificationModel()
        {
            RegisterDocumentInSED = true;
            RejectStatuses = new List<string>();
            RejectStatuses.Add("Ошибка адресации");
            RejectStatuses.Add("Несоотвествие вложения (PDF) техническийм требованиям");
            RejectStatuses.Add("Неверно заполнена XML карточка документа");
            RejectStatuses.Add("Данный вид документов не подлежит опубликованию");
            RejectStatuses.Add("Документ прислан повторно");
            RejectStatus = RejectStatuses[3];
            MountingFlashDisks = new AsyncObservableCollection<FlashSelectorCollectionModel>();
            flashTimer = new System.Timers.Timer();
            flashTimer.Interval = 2000;
            flashTimer.Elapsed += FlashTimer_Elapsed;
            flashTimer.Start();
           
   
        }

        private void FlashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GetUsbDriveInfo();
        }

        public System.Timers.Timer flashTimer { get; set; }

        private AsyncObservableCollection<FlashSelectorCollectionModel> _MountingFlashDisks { get; set; }
        public AsyncObservableCollection<FlashSelectorCollectionModel> MountingFlashDisks
        {
            get
            {
                return this._MountingFlashDisks;
            }
            set
            {
                if (this.MountingFlashDisks != value)
                {
                    this._MountingFlashDisks = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private FlashSelectorCollectionModel _SelectedFlashDisk { get; set; }
        public FlashSelectorCollectionModel SelectedFlashDisk
        {
            get
            {
                return this._SelectedFlashDisk;
            }
            set
            {
                if (this.SelectedFlashDisk != value)
                {
                    this._SelectedFlashDisk = value;
                    this.OnPropertyChanged();
                }
            }
        }

        //Поиск подключенных флэш носителей (выбираем первый попавшийся)
        private async void GetUsbDriveInfo()
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    DriveInfo[] allD = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable && d.Name != "a" && d.IsReady == true).ToArray();
                    if (allD.Count() == 0)
                        MountingFlashDisks.Clear();
                    if (allD.Count() < MountingFlashDisks.Count)
                        MountingFlashDisks.Clear();

                    foreach (DriveInfo d in allD)
                    {
                        FlashSelectorCollectionModel disk = new FlashSelectorCollectionModel(d);
                        if (!MountingFlashDisks.Contains(disk))
                        {
                            MountingFlashDisks.Add(disk);
                            SelectedFlashDisk = MountingFlashDisks[0];
                        }

                    }
                });
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        #region ChangeDocument
        private bool _RegisterDocumentInSED { get; set; }
        public bool RegisterDocumentInSED
        {
            get
            {
                return this._RegisterDocumentInSED;
            }
            set
            {
                if (this.RegisterDocumentInSED != value)
                {
                    this._RegisterDocumentInSED = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private bool _DeleteDocumentFromSED { get; set; }
        public bool DeleteDocumentFromSED
        {
            get
            {
                return this._DeleteDocumentFromSED;
            }
            set
            {
                if (this.DeleteDocumentFromSED != value)
                {
                    this._DeleteDocumentFromSED = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private bool _RejectRegistrationInSED { get; set; }
        public bool RejectRegistrationInSED
        {
            get
            {
                return this._RejectRegistrationInSED;
            }
            set
            {
                if (this.RejectRegistrationInSED != value)
                {
                    this._RejectRegistrationInSED = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private List<string> _RejectStatuses { get; set; }
        public List<string> RejectStatuses
        {
            get
            {
                return this._RejectStatuses;
            }
            set
            {
                if (this.RejectStatuses != value)
                {
                    this._RejectStatuses = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private string _RejectStatus { get; set; }
        public string RejectStatus
        {
            get
            {
                return this._RejectStatus;
            }
            set
            {
                if (this.RejectStatus != value)
                {
                    this._RejectStatus = value;
                    this.OnPropertyChanged();

                }
            }
        }



        private Document _OperationDocument { get; set; }
        public Document OperationDocument
        {
            get
            {
                return this._OperationDocument;
            }
            set
            {
                if (this.OperationDocument != value)
                {
                    this._OperationDocument = value;
                    this.OnPropertyChanged();

                }
            }
        }
        #endregion


    }




    public class FlashSelectorCollectionModel
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public FlashSelectorCollectionModel(){ }
        public FlashSelectorCollectionModel(DriveInfo drive)
        {
            try
            {
                SelectedFlashDisk = drive;
                AvaliableSpace = (int)(drive.AvailableFreeSpace / 1000000);
                TotalSpace = (int)(drive.TotalSize / 1000000);
                TakenSpace = TotalSpace - AvaliableSpace;
                IsReady = drive.IsReady;
                Name = string.Format("{0} - {1} - Свободно: {2} Мб.", drive.Name, drive.VolumeLabel, AvaliableSpace);
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private DriveInfo _SelectedFlashDisk { get; set; }
        public DriveInfo SelectedFlashDisk
        {
            get
            {
                return this._SelectedFlashDisk;
            }
            set
            {
                if (this.SelectedFlashDisk != value)
                {
                    this._SelectedFlashDisk = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Name { get; set; }
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                if (this.Name != value)
                {
                    this._Name = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private bool _IsSelected { get; set; }
        public bool IsSelected
        {
            get
            {
                return this._IsSelected;
            }
            set
            {
                if (this.IsSelected != value)
                {
                    this._IsSelected = value;
                    this.OnPropertyChanged();

                }
            }
        }


        private int _AvaliableSpace { get; set; }
        public int AvaliableSpace
        {
            get
            {
                return this._AvaliableSpace;
            }
            set
            {
                if (this.AvaliableSpace != value)
                {
                    this._AvaliableSpace = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private int _TotalSpace { get; set; }
        public int TotalSpace
        {
            get
            {
                return this._TotalSpace;
            }
            set
            {
                if (this.TotalSpace != value)
                {
                    this._TotalSpace = value;
                    this.OnPropertyChanged();

                }
            }
        }
        private int _TakenSpace { get; set; }
        public int TakenSpace
        {
            get
            {
                return this._TakenSpace;
            }
            set
            {
                if (this.TakenSpace != value)
                {
                    this._TakenSpace = value;
                    this.OnPropertyChanged();

                }
            }
        }

        private bool _IsReady { get; set; }
        public bool IsReady
        {
            get
            {
                return this._IsReady;
            }
            set
            {
                if (this.IsReady != value)
                {
                    this._IsReady = value;
                    this.OnPropertyChanged();

                }
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as FlashSelectorCollectionModel;
            if (item == null)
            {
                return false;
            }
            return this.Name.Equals(item.Name);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public override string ToString()
        {
            return this.Name;
        }
    }

}
