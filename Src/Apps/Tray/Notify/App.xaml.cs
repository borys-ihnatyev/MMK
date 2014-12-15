using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using MMK.Notify.Controls;
using MMK.Notify.Model;
using MMK.Notify.Model.Launchers;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Remoting;
using MMK.Notify.Observer.Tasking.Observing;
using MMK.Notify.Properties;
using MMK.Notify.View.TrayMenu;
using MMK.Notify.ViewModel.TrayMenu;
using MMK.Processing.AutoFolder;
using MMK.Wpf.Providers;
using Application = System.Windows.Application;
using Timer = System.Timers.Timer;

namespace MMK.Notify
{
    public partial class App : INotifyObserverOwner
    {
        private static TrayMenuViewModel viewModel;

        private readonly NotifyIcon notifyIcon;

        private readonly GroupTaskObserver taskObserver;
        private readonly NotifyObserver notifyObserver;
        private readonly BalloonTip notification;

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

            taskObserver = new GroupTaskObserver();
            notifyObserver = new NotifyObserver(taskObserver);

            notification = new BalloonTip();

            MusicDownloadsWatcher = new MusicDownloadsWatcher();
            notifyIcon = new NotifyIcon
            {
                Icon = Notify.Properties.Resources.NotifyLogo,
                Text = @"MMK Notify",
                Visible = true
            };

            shortcutProviders = new GlobalShortcutProviderCollection();
        }

        public HashTagFolderCollection FolderCollection { get; set; }

        public MusicDownloadsWatcher MusicDownloadsWatcher { get; private set; }

        public GroupTaskObserver TaskObserver
        {
            get { return taskObserver; }
        }

        public INotifyObserver NotifyObserver
        {
            get { return notifyObserver; }
        }

        public BalloonTip Notification
        {
            get { return notification; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Initialize();

            MainWindow = new TrayMenuWindow(ViewModel);

            if (MainWindow.IsLoaded)
            {
                LoadShortcuts();
                ViewModel.LoadData();
            }
            else
            {
                MainWindow.Loaded += (sender, args) => LoadShortcuts();
                MainWindow.Loaded += (sender, args) => ViewModel.LoadData();
            }

            MainWindow.Show();
        }

        private void Initialize()
        {
            taskObserver.TaskObserved += TaskObserved;
            MusicDownloadsWatcher.FileDownloaded += notifyObserver.NormalizeTrackName;

            notifyIcon.Click += NotifyIcon_Click;
            notifyIcon.Visible = true;

            StartNotifyObserver();
            taskObserver.Start();

            InitializeFolderCollection();
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

        private void InitializeFolderCollection()
        {
            FolderCollection = Settings.Default.FolderCollection;
        }

        public new static App Current
        {
            get { return (App) Application.Current; }
        }

        public static TrayMenuViewModel ViewModel
        {
            get { return viewModel ?? (viewModel = new TrayMenuViewModel()); }
        }

        #region Behaviors

        protected override void OnDeactivated(EventArgs e)
        {
            ViewModel.IsVisible = false;
            notifyIcon.Click -= NotifyIcon_Click;

            var timer = new Timer(210);

            timer.Elapsed += (s, a) =>
            {
                notifyIcon.Click += NotifyIcon_Click;
                timer.Stop();
                timer.Dispose();
            };

            timer.Start();

            base.OnDeactivated(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            taskObserver.Dispose();
            base.OnExit(e);
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            ViewModel.IsVisible = true;
            MainWindow.Activate();
        }

        #endregion

        #region Task Observing Notify

        private void TaskObserved(INotifyable info)
        {
            notification.Push(info);
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