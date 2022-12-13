using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Commands;
using Medo.Core.EventsAggregator;
using Medo.Core.Models.ReportsSenderModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Medo.Modules.ReportsListModule.ViewModels
{
    public class ViewReportsListViewModel : ReportsSender
    {

        IEventAggregator _aggregator;
        public ViewReportsListViewModel(IEventAggregator aggregator) : base(aggregator)
        {
            _aggregator = aggregator;
            //_aggregator.GetEvent<DateSelectedEvent>().Subscribe(d=> SelectAllReports(true));
            InitializeCommand();
        }

        #region 
        /// <summary>
        /// Выбор или отмена выбора всех документов в списке object - bool
        /// </summary>
        public DelegateCommand<object> SelectAllReportsCommand { get; set; }
        /// <summary>
        /// Выбор документа object = NotificationGuid
        /// </summary>
        public DelegateCommand<object> SelectReportCommand { get; set; }
        private void InitializeCommand()
        {
            //SelectAllReportsCommand = new DelegateCommand<object>(SelectAllReports);
            SelectReportCommand = new DelegateCommand<object>(SelectReport);
        }
        #endregion

        private void SelectReport(object nguid)
        {
            Guid notify = (Guid)nguid;
            ReportModel item = ReportsCollection.Where(g => g.NotificationGuid == notify).FirstOrDefault();
            item.IsSelected = !item.IsSelected;
        }
       
    }
}
