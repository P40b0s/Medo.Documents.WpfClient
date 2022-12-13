using Prism.Unity;
using System.Windows;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Medo.Client.Views;
using Medo.Modules.TabViewModule;
using Medo.Modules.WcfModule;
using Medo.Modules.TrayInfoModule;
using Medo.Modules.CardEditorModule;
using Medo.Modules.PdfViewerModule;
using Medo.Modules.DocumentsUoloaderModule;
using Medo.Modules.ReportsListModule;

namespace Medo.Client
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog catalog = (ModuleCatalog)ModuleCatalog;        
                         
            catalog.AddModule(typeof(ModuleTrayInfoModule));
            catalog.AddModule(typeof(ModuleDocumentsUploaderModule));
            catalog.AddModule(typeof(ModuleTabViewModule));
            catalog.AddModule(typeof(ModuleWcfModule));
            catalog.AddModule(typeof(ModuleCardEditorModule));
            catalog.AddModule(typeof(ModulePdfViewerModule));
            catalog.AddModule(typeof(ModuleClientUpdateModule));
            catalog.AddModule(typeof(ReportsListModuleLoader));
            catalog.Initialize();
        }

        protected override void ConfigureContainer()
        {
            //Container.RegisterTypeForNavigation<ModuleCardCreatorModule>("CardCreatorNavigation");
            base.ConfigureContainer();
        }

        //protected override IModuleCatalog CreateModuleCatalog()
        //{
        //    return new DirectoryModuleCatalog() {ModulePath = @".\Modules" };
        //}
    }
}
