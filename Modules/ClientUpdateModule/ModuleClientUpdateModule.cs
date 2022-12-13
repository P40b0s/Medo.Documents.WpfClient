using Medo.Modules.ClientUpdateModule.Views;
using Prism.Modularity;
using Prism.Regions;

namespace Medo.Modules.DocumentsUoloaderModule
{
    public class ModuleClientUpdateModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleClientUpdateModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("ClientUpdateModuleRegion", typeof(ViewClientUpdateModule));
        }
    }
}
