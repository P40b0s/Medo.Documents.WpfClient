using Medo.Modules.PdfViewerModule.Views;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.PdfViewerModule
{
    public class ModulePdfViewerModule : IModule
    {
        IRegionManager _regionManager;
        IUnityContainer container;
        public ModulePdfViewerModule(IRegionManager regionManager, IUnityContainer _container)
        {
            _regionManager = regionManager;
            container = _container;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("PdfViewerRegion", typeof(ViewPdfViewerModule));
            _regionManager.RegisterViewWithRegion("ContentViewerRegion", typeof(ViewContentViewer));
        }
    }
}
