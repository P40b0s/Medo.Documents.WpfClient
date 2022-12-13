using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Commands;
using NLog;
using Prism.Interactivity.InteractionRequest;
using Medo.Client.Notifications;
using Medo.Core.Models.ReportsSenderModel;
using Medo.Core.EventsAggregator;

namespace Medo.Modules.ReportsListModule
{
    public class ReportsSender : LocalReports
    {
        readonly Logger logger = LogManager.GetCurrentClassLogger();
        IEventAggregator _aggregator;
        public InteractionRequest<SelectAdressesModel> SelectAdressRequest { get; private set; }
        public ReportsSender(IEventAggregator aggregator) : base(aggregator)
        {
            _aggregator = aggregator;
            InitializeCommand();
            this.SelectAdressRequest = new InteractionRequest<SelectAdressesModel>();
        }

        #region 
        /// <summary>
        /// Выбор документа object = NotificationGuid
        /// </summary>
        public DelegateCommand SendReportsCommand { get; set; }
        public DelegateCommand SelectAdressCommand { get; set; }
        private void InitializeCommand()
        {
            SendReportsCommand = new DelegateCommand(SendReports);
            SelectAdressCommand = new DelegateCommand(SelectAdress);
        }
        #endregion

        private async void SendReports()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    IEnumerable<ReportModel> reports = ReportsCollection.Where(s => s.IsSelected);
                    SelectedReportsCount = reports.Count();
                    if (IsOldReport)
                    {
                        SendSZIReport();
                    }
                    else
                    {
                        foreach (ReportModel report in reports)
                        {
                            List<string> adresses = NotificationAdressList.Where(s => s.IsSelected).Select(s=>s.Adress).ToList();
                            if (adresses.Count() > 0)
                            {
                                report.AdressList = adresses;
                            }
                            _aggregator.GetEvent<SendReportEvent>().Publish(report);
                            SendedReportsCount = ReportsCollection.Where(s => s.IsSelected).Count();
                        }
                        foreach (var a in NotificationAdressList)
                        {
                            a.IsSelected = false;
                        }
                    }

                   

                }
                catch (System.Exception ex)
                {
                    logger.Fatal(ex);
                }
            });
        }


        private void SelectAdress()
        {
            SelectAdressesModel d = new SelectAdressesModel {AdressList = NotificationAdressList, Title = "Выбор адресатов отправки документов"};    
                 
            SelectAdressRequest.Raise(d, returned =>
            {
            });
        }
        #region Properties
      
        public bool IsOldReport
        {
            get
            {
                return this._IsOldReport;
            }
            set
            {
                if (this.IsOldReport != value)
                {
                    this._IsOldReport = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _IsOldReport { get; set; }


        #endregion
    }
}
