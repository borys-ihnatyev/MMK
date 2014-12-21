using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using MMK.ApplicationServiceModel;
using MMK.Notify.Properties;
using MMK.Notify.ViewModels.TrayMenu;
using MMK.Notify.Views.TrayMenu;
using Application = System.Windows.Application;

namespace MMK.Notify.Services
{
    public class TrayMenuService : Service, IDisposable
    {
        private TrayMenuWindow trayMenuWindow;
        private readonly TrayMenuViewModel trayMenuViewModel;
        private readonly NotifyIcon trayIcon;
        private readonly GlobalShortcutService shortcutService;

        public TrayMenuService(GlobalShortcutService shortcutService)
        {
            trayIcon = new NotifyIcon
            {
                Icon = Resources.NotifyLogo,
                Text = @"MMK Notify"
            };

            trayMenuViewModel = new TrayMenuViewModel();

            this.shortcutService = shortcutService;
        }

        public Window TrayMenuWindow
        {
            get { return trayMenuWindow; }
        }


        protected override void OnInitialize()
        {
            InitializeTrayIcon();
            InitializeTrayWindow();
        }

        private void InitializeTrayWindow()
        {
            trayMenuWindow = new TrayMenuWindow();
            trayMenuWindow.BeginInit();

            trayMenuWindow.DataContext = trayMenuViewModel;

            trayMenuWindow.Loaded += (sender, args) => shortcutService.Initialize();
            trayMenuWindow.Loaded += (sender, args) => trayMenuViewModel.LoadData();

            trayMenuWindow.EndInit();
        }

        private void InitializeTrayIcon()
        {
            trayIcon.Click += OnTrayIconClick;
            trayIcon.Visible = true;
            Application.Current.Deactivated += OnAppDeactivated;
        }


        private void OnTrayIconClick(object sender, EventArgs eventArgs)
        {
            trayMenuViewModel.IsVisible = true;
            trayMenuWindow.Activate();
        }

        private void OnAppDeactivated(object sender, EventArgs eventArgs)
        {
            trayMenuViewModel.IsVisible = false;
            trayIcon.Click -= OnTrayIconClick;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(210),
                IsEnabled = false
            };

            timer.Tick += (s, a) =>
            {
                trayIcon.Click += OnTrayIconClick;
                timer.Stop();
            };

            timer.Start();
        }


        public override void Start()
        {
            if (!IsInitialized)
                throw new Exception("Service was not Initialized");

            trayIcon.Visible = true;
            trayMenuWindow.Show();
        }

        public override void Stop()
        {
            trayMenuWindow.Hide();
            trayIcon.Visible = false;
        }

        public void Dispose()
        {
            trayMenuWindow.Close();
            trayIcon.Dispose();
        }
    }
}