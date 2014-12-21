using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using MMK.ApplicationServiceModel;
using MMK.ApplicationServiceModel.Locator;
using MMK.Notify.Model;
using MMK.Notify.Model.Launchers;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Remoting;
using MMK.Notify.Observer.Tasking.Observing;
using MMK.Notify.Properties;
using MMK.Notify.Services;
using MMK.Processing.AutoFolder;
using MMK.Wpf.Providers;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Application = System.Windows.Application;

namespace MMK.Notify
{
    [ServiceLocatorOwner]
    public partial class App
    {
        private static ServiceLocator serviceLocator;

        [ServiceLocator]
        public static ServiceLocator ServiceLocator
        {
            get { return (serviceLocator ?? (serviceLocator = new ServiceLocator())); }
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
            ServiceLocator.Bind<TaskObserver>().ToSelf().InSingletonScope();
            ServiceLocator.Bind<INotifyObserver>().To<NotifyObserver>().InSingletonScope();
            
            ServiceLocator.Bind<HashTagFolderCollection>().ToMethod(c => Settings.Default.FolderCollection).InSingletonScope();

            ServiceLocator.Bind<NotificationService>().ToSelf().InSingletonScope();
            ServiceLocator.Bind<TaskObserverService>().ToSelf().InSingletonScope();
            ServiceLocator.Bind<GlobalShortcutService>().ToSelf().InSingletonScope();
            ServiceLocator.Bind<TrayMenuService>().ToSelf().InSingletonScope();

            ServiceLocator.Get<TrayMenuService>().Initialize();
        }

        private static void StartServices()
        {
            ServiceLocator.Get<TaskObserverService>().Start();
            ServiceLocator.Get<TrayMenuService>().Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceLocator.Get<TrayMenuService>().Stop();
            ServiceLocator.Get<TaskObserverService>().Stop();
            base.OnExit(e);
        }
    }
}