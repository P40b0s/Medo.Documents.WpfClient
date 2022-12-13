using Medo.Core.EventsAggregator;
using Medo.Core.Interfaces.ReportsSenderInterfaces;
using Medo.Core.Models.ReportsSenderModel;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.ReportsListModule
{
    public class Collections : INotifyPropertyChanged
    {
        #region iNotifyPropertyChanged       
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        IEventAggregator eventAggregator;
        public Collections(IEventAggregator _eventAggregator)
        {
            ReportsCollection = new ObservableCollection<ReportModel>();
            NotificationAdressList = new List<NotificationSubjectsModel>();
            eventAggregator = _eventAggregator;
            eventAggregator.GetEvent<GetReportsListFromServerEvent>().Subscribe(GetReportsCollection);
            eventAggregator.GetEvent<SendReportCallbackEvent>().Subscribe(UpdateReportsCallback);
            eventAggregator.GetEvent<GetNotificationAdressListFromServerEvent>().Subscribe(GetNotificationsListCollection);
           
        }
        private void GetReportsCollection(List<IReportModelInterface> list)
        {
            ReportsCollection.Clear();
            foreach (var item in list)
            {
                item.IsSelected = true;
                ReportsCollection.Add((ReportModel)item);
            }
            IsSelectAll = true;
        }
        private void UpdateReportsCallback(IReportModelInterface report)
        {
           ReportsCollection.Where(n => n.NotificationGuid == report.NotificationGuid).FirstOrDefault().UpdateNotificationList(report);
        }

        private void GetNotificationsListCollection(List<INotificationAdresseChoiserInterface> list)
        {
            NotificationAdressList.Clear();
            foreach (var item in list)
            {
                NotificationAdressList.Add((NotificationSubjectsModel)item);
            }
        }


        public  ObservableCollection<ReportModel> ReportsCollection { get; set; }
        public  List<NotificationSubjectsModel> NotificationAdressList { get; set; }

        #region Property
        public bool IsSelectAll
        {
            get
            {
                return this._IsSelectAll;
            }
            set
            {
                if (this.IsSelectAll != value)
                {
                    this._IsSelectAll = value;
                    if (value)
                    {
                        SelectAllReports(true);
                    }
                    else
                    {
                        SelectAllReports(false);
                    }
                    OnPropertyChanged();
                }
            }
        }
        private bool _IsSelectAll { get; set; }

        public void SelectAllReports(bool isSelect)
        {
            bool sel = (bool)isSelect;
            if (sel)
            {
                foreach (ReportModel rm in ReportsCollection)
                {
                    rm.IsSelected = true;
                }
            }
            else
            {
                foreach (ReportModel rm in ReportsCollection)
                {
                    rm.IsSelected = false;
                }
            }

        }
        #endregion
    }
}
