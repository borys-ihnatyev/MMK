using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using MMK.ApplicationServiceModel;
using MMK.Notify.Properties;
using MMK.Notify.ViewModels;
using MMK.Notify.Views;

namespace MMK.Notify.Services
{
    public class TrayMenuService : Service, IDisposable
    {
        private readonly GlobalShortcutService shortcutService;
        private readonly TaskProgressService taskProgressService;
        private readonly NotifyIcon trayIcon;
        private readonly TrayMenuViewModel trayMenuViewModel;
        private TrayMenuView trayMenuView;

        public TrayMenuService(GlobalShortcutService shortcutService, TaskProgressService taskProgressService)
        {
            trayIcon = new NotifyIcon
            {
                Icon = Resources.logo_normal,
                Text = @"MMK Notify"
            };

            trayMenuViewModel = new TrayMenuViewModel();
            this.shortcutService = shortcutService;
            this.taskProgressService = taskProgressService;
        }

        public Window TrayMenuView
        {
            get { return trayMenuView; }
        }

        public void Dispose()
        {
            taskProgressService.StateChanged -= TaskProgressStateChanged;

            trayMenuView.Close();
            trayIcon.Dispose();
        }

        public override void Start()
        {
            if (!IsInitialized)
                throw new Exception("Service was not Initialized");

            trayIcon.Visible = true;
            trayMenuView.Show();
            trayMenuView.Activate();
        }

        public override void Stop()
        {
            trayMenuView.Hide();
            trayIcon.Visible = false;
        }

        #region Init

        protected override void OnInitialize()
        {
            InitializeTrayWindow();
            InitializeTrayIcon();
            taskProgressService.StateChanged += TaskProgressStateChanged;
        }

        private void InitializeTrayWindow()
        {
            trayMenuView = new TrayMenuView {DataContext = trayMenuViewModel};

            trayMenuView.Loaded += (s, a) => shortcutService.Initialize();
            trayMenuView.Loaded += (s, a) => trayMenuViewModel.LoadData();
            trayMenuView.Closed += (s, a) => trayMenuViewModel.UnloadData();

            trayMenuView.Deactivated += OnTrayMenuViewDeactivated;

            trayMenuView.Show();
        }

        private void InitializeTrayIcon()
        {
            trayIcon.MouseClick += OnTrayIconMouseClick;
        }

        private readonly AutoResetEvent trayIconMouseClickEvent = new AutoResetEvent(false);

        private void OnTrayIconMouseClick(object sender, MouseEventArgs e)
        {
            trayIconMouseClickEvent.Set();

            if (trayMenuView.Visibility == Visibility.Hidden || trayMenuView.Visibility == Visibility.Collapsed)
            {
                trayMenuView.Show();
                trayMenuView.Activate();
            }
            else
            {
                trayMenuView.Hide();
            }
        }

        private void OnTrayMenuViewDeactivated(object sender, EventArgs eventArgs)
        {
            if(trayIconMouseClickEvent.WaitOne(TimeSpan.FromSeconds(0.5)))
                return;

            trayMenuView.Hide();
        }

        private void TaskProgressStateChanged(object sender, ChangedEventArgs<bool> e)
        {
            trayIcon.Icon = e.NewValue ? Resources.logo_processing : Resources.logo_normal;
        }

        #endregion
    }
}