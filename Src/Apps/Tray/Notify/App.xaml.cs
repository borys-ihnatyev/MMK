using System.Diagnostics.Contracts;
using System.Threading;
using System.Windows;
using MMK.ApplicationServiceModel.Locator;
using MMK.Notify.Model;
using MMK.Notify.Model.Service;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Remoting;
using MMK.Notify.Observer.Tasking.Observing;
using MMK.Notify.Properties;
using MMK.Notify.Services;
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

#if !DEBUG
        static App()
        {
            AppDomain.CurrentDomain.UnhandledException += DomainUnhandledException;
        }

        private static void DomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TryLogException(e.ExceptionObject);
            Current.Shutdown();
        }

        private static void TryLogException(object error)
        {
            if (!(error is Exception))
                return;
            LogException((Exception) error);
        }

        private static void LogException(Exception exception)
        {
            using (var log = new StreamWriter(typeof (App).FullName + ".Error.log"))
            {
                log.WriteLine(exception.Message);
                log.WriteLine(exception.StackTrace);
            }
        }
#endif

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!AppGuard.IsSingleInstance())
            {
                Shutdown();
                return;
            }

            Initialize();
            StartServices();
        }

        private void Initialize()
        {
            InitializeServices();
        }

        private static void InitializeServices()
        {
            Contract.Assume(ServiceLocator != null);

            ServiceLocator.Bind<TaskObserver>().ToSelf().InSingletonScope();

            ServiceLocator.Bind<INotifyObserver>()
                .To<NotifyObserver>()
                .InSingletonScope();

            ServiceLocator.Bind<IDownloadsWatcher>()
                .To<ChromeDownloadsWatcherService>()
                .InSingletonScope();

            ServiceLocator.Bind<HashTagFolderCollection>()
                .ToMethod(c => Settings.Default.FolderCollection)
                .InSingletonScope();

            ServiceLocator.Bind<NotificationService>().ToSelf().InSingletonScope();
            ServiceLocator.Bind<TaskProgressService>().ToSelf().InSingletonScope();

            ServiceLocator.Bind<GlobalShortcutService>().ToSelf().InSingletonScope();
            ServiceLocator.Bind<TrayMenuService>().ToSelf().InSingletonScope();

            ServiceLocator.Get<TrayMenuService>().Initialize();
            ServiceLocator.Get<TaskProgressService>().Initialize();

            Current.MainWindow = ServiceLocator.Get<TrayMenuService>().TrayMenuView;
        }

        private static void StartServices()
        {
            Contract.Assume(ServiceLocator != null);

            ServiceLocator.Get<TaskObserver>().Start();
            ServiceLocator.Get<NotifyObserver>().Start();
            ServiceLocator.Get<TrayMenuService>().Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Contract.Assume(ServiceLocator != null);

            ServiceLocator.Get<TrayMenuService>().Stop();
            ServiceLocator.Get<TaskProgressService>().Stop();
            ServiceLocator.Get<NotifyObserver>().Stop();
            ServiceLocator.Get<TaskObserver>().Cancell();
            base.OnExit(e);
        }
    }
}