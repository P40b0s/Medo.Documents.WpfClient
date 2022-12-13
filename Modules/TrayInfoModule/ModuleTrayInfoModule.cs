using Medo.Modules.TrayInfoModule.Views;
using Prism.Modularity;
using Prism.Regions;

namespace Medo.Modules.TrayInfoModule
{
    public class ModuleTrayInfoModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleTrayInfoModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("TrayInfoRegion", typeof(ViewTrayInfoModule));
        }
    }
}
