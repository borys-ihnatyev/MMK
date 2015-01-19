using System;
using System.Windows;
using System.Windows.Forms;
using MMK.ApplicationServiceModel;
using MMK.Notify.Properties;
using MMK.Notify.ViewModels;
using MMK.Notify.Views;

namespace MMK.Notify.Services
{
    public class TrayMenuService : InitializableService, IDisposable
    {
        private readonly TaskProgressService taskProgressService;
        private readonly NotifyIcon trayIcon;
        private readonly TrayMenuViewModel trayMenuViewModel;
        private TrayMenuView trayMenuView;

        public TrayMenuService(TaskProgressService taskProgressService)
        {
            trayIcon = new NotifyIcon
            {
                Icon = Resources.logo_normal,
                Text = @"MMK Notify"
            };

            trayMenuViewModel = new TrayMenuViewModel();
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
            taskProgressService.StateChanged += OnTaskProgressStateChanged;
        }

        private void InitializeTrayWindow()
        {
            trayMenuView = new TrayMenuView {DataContext = trayMenuViewModel};

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

        private void OnTaskProgressStateChanged(object sender, ChangedEventArgs<bool> e)
        {
            trayIcon.Icon = e.NewValue ? Resources.logo_processing : Resources.logo_normal;
        }

        #endregion

        protected override void OnStart()
        {
            CheckInitialized();

            trayIcon.Visible = true;
            trayMenuView.Show();
        }

        protected override void OnStop()
        {
            trayMenuView.Hide();
            trayIcon.Visible = false;
        }

        public void Dispose()
        {
            taskProgressService.StateChanged -= OnTaskProgressStateChanged;

            trayMenuView.Close();
            trayIcon.Dispose();
        }
    }
}