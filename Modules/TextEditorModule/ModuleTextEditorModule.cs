using Medo.Modules.TextEditorModule.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medo.Modules.TextEditorModule
{
    public class ModuleTextEditorModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleTextEditorModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("TextEditorRegion", typeof(ViewTextEditorModule));
        }
    }
}
