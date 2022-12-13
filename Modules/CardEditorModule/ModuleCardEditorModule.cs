using Medo.Modules.CardEditorModule.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.CardEditorModule
{
    public class ModuleCardEditorModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleCardEditorModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("CardEditorRegion", typeof(ViewCardEditorModule));
        }
    }
}
