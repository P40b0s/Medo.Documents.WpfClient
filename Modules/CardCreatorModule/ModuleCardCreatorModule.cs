using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Modularity;
using Prism.Regions;
using Medo.Modules.CardCreatorModule.Views;

namespace CardCreatorModule
{
    public class ModuleCardCreatorModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleCardCreatorModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("CardCreatorRegion", typeof(CardCreatorModuleView));
        }
    }
}
