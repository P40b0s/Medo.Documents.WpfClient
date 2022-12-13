using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Modularity;
using Prism.Regions;
using Medo.Modules.ReportsListModule.Views;

namespace Medo.Modules.ReportsListModule
{
    public class ReportsListModuleLoader : IModule
    {
        IRegionManager _regionManager;
        public ReportsListModuleLoader(RegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("ReportsListRegion", typeof(ViewReportsList));
        }
    }
}
