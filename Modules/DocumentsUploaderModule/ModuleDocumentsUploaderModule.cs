using Medo.Modules.DocumentsUploaderModule.Views;
using Prism.Modularity;
using Prism.Regions;

namespace Medo.Modules.DocumentsUoloaderModule
{
    public class ModuleDocumentsUploaderModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleDocumentsUploaderModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("DocumentsUploaderRegion", typeof(ViewDocumentsUploaderModule));
        }
    }
}
