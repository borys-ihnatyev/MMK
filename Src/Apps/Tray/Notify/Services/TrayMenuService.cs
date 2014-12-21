using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using MMK.Notify.Properties;
using MMK.Notify.ViewModels.TrayMenu;
using MMK.Notify.Views.TrayMenu;
using Application = System.Windows.Application;

namespace MMK.Notify.Services
{
    public class TrayMenuService
    {
        private bool initialized;
        private TrayMenuWindow trayMenuWindow;
        private readonly TrayMenuViewModel trayMenuViewModel;
        private readonly NotifyIcon trayIcon;

        public event EventHandler<EventArgs<Window>> WindowInitialize;

        public TrayMenuService()
        {
            trayIcon = new NotifyIcon
            {
                Icon = Resources.NotifyLogo,
                Text = @"MMK Notify"
            };

            trayMenuViewModel = new TrayMenuViewModel();
        }

        public Window TrayMenuWindow
        {
            get { return trayMenuWindow; }
        }

        public void Initialize()
        {
            if (initialized)
                return;
            InitializeTrayIcon();
            InitializeTrayWindow();
            initialized = true;
        }

        private void InitializeTrayWindow()
        {
            trayMenuWindow = new TrayMenuWindow();
            trayMenuWindow.BeginInit();
            
            OnWindowInitialize();

            trayMenuWindow.DataContext = trayMenuViewModel;
            trayMenuWindow.Loaded += (sender, args) => trayMenuViewModel.LoadData();
        }

        protected virtual void OnWindowInitialize()
        {
            var handler = WindowInitialize;
            if (handler != null) 
                handler(this, new EventArgs<Window>(trayMenuWindow));
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

        public void Start()
        {
            if (!initialized)
                throw new Exception("instanse was not Initialized");

            trayMenuWindow.EndInit();
            trayIcon.Visible = true;
            trayMenuWindow.Show();
        }
    }
}