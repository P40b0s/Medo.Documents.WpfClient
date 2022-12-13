using Medo.Modules.TabViewModule.Views;
using Prism.Modularity;
using Prism.Regions;

namespace Medo.Modules.TabViewModule
{
    public class ModuleTabViewModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleTabViewModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("TabPanelRegion", typeof(ViewTabViewModule));
        }
    }
}
