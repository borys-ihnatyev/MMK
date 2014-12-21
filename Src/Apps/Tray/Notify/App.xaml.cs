using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using MMK.Notify.Controllers;
using MMK.Notify.Model;
using MMK.Notify.Model.Launchers;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Remoting;
using MMK.Notify.Observer.Tasking.Observing;
using MMK.Notify.Properties;
using MMK.Notify.Views.TrayMenu;
using MMK.Processing.AutoFolder;
using MMK.Wpf.Providers;
using Application = System.Windows.Application;

namespace MMK.Notify
{
    public partial class App : INotifyObserverOwner
    {
        private readonly TaskObserver taskObserver;
        private readonly NotifyObserver notifyObserver;
        private readonly NotificationController notification;
        private readonly TrayMenuController trayMenuController;

        private readonly GlobalShortcutProviderCollection shortcutProviders;

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

        public App()
        {
            if (!AppGuard.IsSingleInstance())
                Shutdown();

            taskObserver = new TaskObserver();
            taskObserver.TaskObserved += TaskObserved;

            notifyObserver = new NotifyObserver(taskObserver);

            notification = new NotificationController();

            MusicDownloadsWatcher = new MusicDownloadsWatcher();
            MusicDownloadsWatcher.FileDownloaded += notifyObserver.NormalizeTrackName;

            shortcutProviders = new GlobalShortcutProviderCollection();

            trayMenuController = new TrayMenuController();
            trayMenuController.WindowInitialize += OnWindowInitialize;
        }

        public HashTagFolderCollection FolderCollection { get; set; }

        public MusicDownloadsWatcher MusicDownloadsWatcher { get; private set; }

        public TaskObserver TaskObserver
        {
            get { return taskObserver; }
        }

        public INotifyObserver NotifyObserver
        {
            get { return notifyObserver; }
        }

        public NotificationController Notification
        {
            get { return notification; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Initialize();
            StartServices();
        }

        private void Initialize()
        {
            trayMenuController.Initialize();
            InitializeFolderCollection();
        }

        private void OnWindowInitialize(object sender, EventArgs<Window> e)
        {
            MainWindow = e.Arg;
            MainWindow.Loaded += (s, a) => LoadShortcuts();
        }

        private void InitializeFolderCollection()
        {
            FolderCollection = Settings.Default.FolderCollection;
        }

        private void StartServices()
        {
            taskObserver.Start();
            StartNotifyObserver();

            MainWindow = trayMenuController.TrayMenuWindow;
            trayMenuController.Start();
        }

        private void StartNotifyObserver()
        {
#if DEBUG
            notifyObserver.Start();
#else
            if (!notifyObserver.TryStart())
                Shutdown();
#endif
        }

        public new static App Current
        {
            get { return (App) Application.Current; }
        }

        #region Behaviors

        protected override void OnExit(ExitEventArgs e)
        {
            taskObserver.Dispose();
            base.OnExit(e);
        }

        #endregion

        #region Task Observing Notify

        private void TaskObserved(object sender, TaskObserver.NotifyEventArgs e)
        {
            notification.Push(e.Message);
        }

        #endregion

        #region Shortcuts

        private void LoadShortcuts()
        {
            (shortcutProviders as IGlobalShortcutProvider).SetWindow(MainWindow);

            shortcutProviders.Add(new HotMarkLauncher(KeyModifyers.Ctrl, (int) Keys.T));
            shortcutProviders.Add(KeyModifyers.Ctrl | KeyModifyers.Shift, (int) Keys.T, NormalizeHotKey_Pressed);
            shortcutProviders.Add(KeyModifyers.Ctrl | KeyModifyers.Shift, (int) Keys.M, MoveFileToCollection_Pressed);

            var swiftSearchLauncher = new SwiftSearchLauncher(MainWindow);

            swiftSearchLauncher.SetStartShortcut(KeyModifyers.Ctrl, Keys.Space);
            swiftSearchLauncher.SetStartFromClipboardShortcut(KeyModifyers.Ctrl | KeyModifyers.Shift, Keys.V);

            shortcutProviders.Add(swiftSearchLauncher);
        }

        private void NormalizeHotKey_Pressed()
        {
            notifyObserver.NormalizeTrackName(
                Explorer.GetForegroundSelectedItemsPaths().Where(File.Exists)
                );
        }

        private void MoveFileToCollection_Pressed()
        {
            notifyObserver.MoveToCollectionFolder(
                Explorer.GetForegroundSelectedItemsPaths().Where(File.Exists),
                FolderCollection
                );
        }

        #endregion

        public void StartListenShortcuts()
        {
            shortcutProviders.StartListen();
        }

        public void StopListenShortcuts()
        {
            shortcutProviders.StopListen();
        }
    }
}