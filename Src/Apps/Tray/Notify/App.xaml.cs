using System.Diagnostics.Contracts;
using System.Threading;
using System.Windows;
using MMK.ApplicationServiceModel.Locator;
using MMK.KeyDrive.Services;
using MMK.Notify.Model;
using MMK.Notify.Model.Service;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Remoting;
using MMK.Notify.Observer.Tasking.Observing;
using MMK.Notify.Properties;
using MMK.Notify.Services;
using MMK.Notify.Services.DownloadWatcher;
using MMK.Presentation.Windows.Input;
using MMK.Presentation.Windows.Interop;
using MMK.Processing.AutoFolder;
using Ninject;

namespace MMK.Notify
{
    [ServiceLocatorOwner]
    public partial class App
    {
        private static ServiceLocator serviceLocator;

        [ServiceLocator]
        public static ServiceLocator ServiceLocator
        {
            get
            {
                Contract.Ensures(Contract.Result<ServiceLocator>() != null);
                Contract.EndContractBlock();
                return (serviceLocator ?? (serviceLocator = new ServiceLocator()));
            }
        }

        static App()
        {
            ServiceLocator.Bind<AppDomainErrorService>().ToSelf().InSingletonScope();
            ServiceLocator.Get<AppDomainErrorService>().Start();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!AppGuard.IsSingleInstance())
            {
                Shutdown();
                return;
            }

            BindServices();
            InitializeServices();
            StartServices();
        }

        private static void BindServices()
        {
            ServiceLocator.Bind<HwndSourceService>().ToSelf().InSingletonScope();
            ServiceLocator.Bind<IHwndSource>().ToMethod(c => c.Kernel.Get<HwndSourceService>()).InSingletonScope();
            ServiceLocator.Bind<NotificationService>().ToSelf().InSingletonScope();

            ServiceLocator.Bind<TaskObserver>().ToSelf().InSingletonScope();
            ServiceLocator.Bind<INotifyObserver>().To<NotifyObserver>().InSingletonScope();
            ServiceLocator.Bind<IDownloadsWatcher>().To<ChromeDownloadsWatcherService>().InSingletonScope();
            ServiceLocator.Bind<DownloadsObserverService>().ToSelf().InSingletonScope();

            ServiceLocator.Bind<DriveDetectorServiceBase>().To<DriveWatcherService>().InSingletonScope();

            ServiceLocator.Bind<HashTagFolderCollection>()
                .ToMethod(c => Settings.Default.FolderCollection)
                .InSingletonScope();

            ServiceLocator.Bind<TaskProgressService>().ToSelf().InSingletonScope();

            ServiceLocator.Bind<GlobalShortcutProviderCollection>().ToSelf().InSingletonScope();
            ServiceLocator.Bind<GlobalShortcutService>().ToSelf().InSingletonScope();
            ServiceLocator.Bind<TrayMenuService>().ToSelf().InSingletonScope();
        }

        private static void InitializeServices()
        {
            ServiceLocator.Get<TaskProgressService>().Initialize();
            ServiceLocator.Get<IDownloadsWatcher>().Initialize();

            var hwndSourceService = ServiceLocator.Get<HwndSourceService>();
            hwndSourceService.Initialized += (s, e) =>
            {
                ServiceLocator.Get<GlobalShortcutService>().Initialize();
                ServiceLocator.Get<TrayMenuService>().Initialize();
                ServiceLocator.Get<TrayMenuService>().Start();
                ServiceLocator.Get<DriveDetectorServiceBase>().Start();
            };
        }

        private static void StartServices()
        {
            ServiceLocator.Get<HwndSourceService>().Start();
            ServiceLocator.Get<TaskObserver>().Start();
            ServiceLocator.Get<NotifyObserver>().Start();
            ServiceLocator.Get<DownloadsObserverService>().Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            ServiceLocator.Get<DownloadsObserverService>().Stop();
            ServiceLocator.Get<TrayMenuService>().Stop();
            ServiceLocator.Get<TaskProgressService>().Stop();
            ServiceLocator.Get<NotifyObserver>().Stop();
            ServiceLocator.Get<TaskObserver>().Stop();
            ServiceLocator.Get<HwndSourceService>().Stop();
        }
    }
}