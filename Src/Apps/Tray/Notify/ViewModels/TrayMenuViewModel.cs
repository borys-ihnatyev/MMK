using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MMK.ApplicationServiceModel;
using MMK.Notify.Model.Service;
using MMK.Notify.Model.Settings;
using MMK.Notify.Properties;
using MMK.Notify.Services;
using MMK.Notify.Views;
using MMK.Presentation.Tools;
using MMK.Presentation.ViewModel;

namespace MMK.Notify.ViewModels
{
    public class TrayMenuViewModel : ViewModel
    {
        private Window hashTagFoldersWindow;

        private bool isEnableHotKeys;
        private bool isEnableDownloadsWatch;

        public TrayMenuViewModel()
        {
            StartListenShortcutsCommand = new Command(IoC.Get<GlobalShortcutService>().Start);
            StopListenShortcutsCommand = new Command(IoC.Get<GlobalShortcutService>().Stop);

            StartDownloadsWatchCommand = new Command(IoC.Get<IDownloadsWatcher>().Start);
            StopDownloadsWatchCommand = new Command(IoC.Get<IDownloadsWatcher>().Stop);

            PropertyChanged += OnPropertyChanged;
        }

        [SettingsProperty]
        public bool IsEnableHotKeys
        {
            get { return isEnableHotKeys; }
            set
            {
                if (value == isEnableHotKeys)
                    return;
                isEnableHotKeys = value;
                NotifyPropertyChanged();
            }
        }

        [SettingsProperty]
        public bool IsEnableDownloadsWatch
        {
            get { return isEnableDownloadsWatch; }
            set
            {
                if (value == isEnableDownloadsWatch)
                    return;
                isEnableDownloadsWatch = value;
                NotifyPropertyChanged();
            }
        }


        protected override void OnLoadData()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            IsEnableHotKeys = Settings.Default.IsEnableHotKeys;
            IsEnableDownloadsWatch = Settings.Default.IsEnableDownloadsWatch;

            if (IsEnableHotKeys)
                StartListenShortcutsCommand.Execute(null);

            if (IsEnableDownloadsWatch)
                StartDownloadsWatchCommand.Execute(null);
        }


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;

            if (IsSettingsProperty(propertyName))
                SaveSettings();
        }

        private bool IsSettingsProperty(string propertyName)
        {
            return Attribute.IsDefined(GetType().GetProperty(propertyName), typeof (SettingsPropertyAttribute));
        }

        private void SaveSettings()
        {
            Settings.Default.IsEnableHotKeys = IsEnableHotKeys;
            Settings.Default.IsEnableDownloadsWatch = IsEnableDownloadsWatch;
            Settings.Default.Save();
        }


        public ICommand StartListenShortcutsCommand { get; set; }
        public ICommand StopListenShortcutsCommand { get; set; }

        public ICommand StartDownloadsWatchCommand { get; private set; }
        public ICommand StopDownloadsWatchCommand { get; private set; }

        public ICommand OpenHashTagFoldersWindowCommand { get; private set; }

        public void OpenHashTagFoldersWindow()
        {
            if (hashTagFoldersWindow == null)
            {
                hashTagFoldersWindow = new HashTagFoldersWindow();
                hashTagFoldersWindow.Closed += (sender, args) => hashTagFoldersWindow = null;
                hashTagFoldersWindow.Show();
            }
            else
                hashTagFoldersWindow.Focus();
        }
    }
}