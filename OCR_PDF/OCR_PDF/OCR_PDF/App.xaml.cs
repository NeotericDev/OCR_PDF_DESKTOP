using Prism.Ioc;
using OCR_PDF.Views;
using System.Windows;
using Prism.Modularity;
using OCR_PDF.Modules.ModuleName;
using OCR_PDF.Services.Interfaces;
using OCR_PDF.Services;
//using System.ComponentModel;
using Prism.Regions;
using OCR_PDF.Core;
using Akka.Actor;
using DryIoc;
using OCR_PDF.Actors.UI;
using Akka.Routing;
using OCR_PDF.Actors.Workers;

namespace OCR_PDF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static ActorSystem OCRActorSystem;
        public static IActorRef UiActor;
        protected override Window CreateShell()
        {
            SharedData.TessDataPath = @"C:\Users\Dev\Downloads\syncfusionocrprocessor\tessdata";
            IContainer container = Container.Resolve<IContainer>();
            IRegionManager regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.HeaderRegion, typeof(HeaderControl));
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(MainContentControl));
            //regionManager.RegisterViewWithRegion(RegionNames.TasksRegion, typeof(TaskControl));
            OCRActorSystem = ActorSystem.Create("OCRActorSystem").UseDryIoC(container);
            UiActor = OCRActorSystem.ActorOf(Props.Create<TaskDispatcher>(), "TaskDispatcher");
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            //containerRegistry.RegisterForNavigation<HeaderControl>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
        }

    }
}
