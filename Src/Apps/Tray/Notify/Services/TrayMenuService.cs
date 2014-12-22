using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using MMK.ApplicationServiceModel;
using MMK.Notify.Properties;
using MMK.Notify.ViewModels;
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
        private readonly TaskProgressService taskProgressService;

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

        public Window TrayMenuWindow
        {
            get { return trayMenuWindow; }
        }

        protected override void OnInitialize()
        {
            InitializeTrayIcon();
            InitializeTrayWindow();
            taskProgressService.IsActiveChanged += TaskProgressStateChanged;
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
            trayIcon.MouseClick += OnTrayIconMouseClick;
            Application.Current.Deactivated += OnAppDeactivated;
        }

        private void OnTrayIconMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                taskProgressService.Start();
            else
                ShowTrayMenu();
        }

        private void ShowTrayMenu()
        {
            trayMenuViewModel.IsVisible = true;
            trayMenuWindow.Activate();
        }

        private void OnAppDeactivated(object sender, EventArgs eventArgs)
        {
            trayMenuViewModel.IsVisible = false;
            trayIcon.MouseClick -= OnTrayIconMouseClick;
            
            taskProgressService.Stop();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(210),
                IsEnabled = false
            };

            timer.Tick += (s, a) =>
            {
                trayIcon.MouseClick += OnTrayIconMouseClick;
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
            taskProgressService.IsActiveChanged -= TaskProgressStateChanged;

            trayMenuWindow.Close();
            trayIcon.Dispose();
        }

        private void TaskProgressStateChanged(object sender, ChangedEventArgs<bool> e)
        {
            trayIcon.Icon = e.NewValue ? Resources.logo_processing : Resources.logo_normal;
        }
    }
}