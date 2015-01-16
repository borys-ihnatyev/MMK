using System;
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

            trayMenuView.Show();
        }

        private void InitializeTrayIcon()
        {
            trayIcon.MouseClick += OnTrayIconMouseClick;
        }

        private void OnTrayIconMouseClick(object sender, MouseEventArgs e)
        {
            trayMenuView.Show();
        }

        private void TaskProgressStateChanged(object sender, ChangedEventArgs<bool> e)
        {
            trayIcon.Icon = e.NewValue ? Resources.logo_processing : Resources.logo_normal;
        }

        #endregion

        public override void Start()
        {
            if (!IsInitialized)
                throw new Exception("Service was not Initialized");

            trayIcon.Visible = true;
            trayMenuView.Show();
        }

        public override void Stop()
        {
            trayMenuView.Hide();
            trayIcon.Visible = false;
        }

        public void Dispose()
        {
            taskProgressService.StateChanged -= TaskProgressStateChanged;

            trayMenuView.Close();
            trayIcon.Dispose();
        }
    }
}