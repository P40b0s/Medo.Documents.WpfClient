using Medo.Modules.WcfModule.Views;
using Prism.Modularity;
using Prism.Regions;

namespace Medo.Modules.WcfModule
{
    public class ModuleWcfModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleWcfModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("WcfRegion", typeof(ViewWcf));
        }
    }
}
