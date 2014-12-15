using System.Windows;
using System.Windows.Input;
using MMK.Notify.Properties;
using MMK.Notify.View;
using MMK.Wpf;

namespace MMK.Notify.ViewModel.TrayMenu
{
    public class TrayMenuViewModel : Wpf.ViewModel.ViewModel
    {
        private bool isVisible = true;
        private Window hashTagFoldersWindow;

        private bool isEnableHotKeys;
        private bool isEnableDownloadsWatch;

        public TrayMenuViewModel()
        {
            OpenHashTagFoldersWindowCommand = new Command(OpenHashTagFoldersWindowCommandAction);
            
            StartListenShortcutsCommand = new Command(App.Current.StartListenShortcuts);
            StopListenShortcutsCommand = new Command(App.Current.StopListenShortcuts);
            
            StartDownloadsWatchingCommand = new Command(App.Current.MusicDownloadsWatcher.Start);
            StopDownloadsWatchingCommand = new Command(App.Current.MusicDownloadsWatcher.Stop);

            ExitCommand = new Command(ExitCommandAction);
            HideCommand = new Command(HideCommandAction);
        }

        protected override void OnLoadData()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            IsEnableHotKeys = Settings.Default.IsEnableHotKeys;
            IsEnableDownloadsWatch = Settings.Default.IsEnableDownloadsWatch;

            if(isEnableHotKeys)
                StartListenShortcutsCommand.Execute(null);

            if(IsEnableDownloadsWatch)
                StartDownloadsWatchingCommand.Execute(null);
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (value == isVisible) return;

                isVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnableHotKeys
        {
            get { return isEnableHotKeys; }
            set
            {
                if(value == isEnableHotKeys)
                    return;
                isEnableHotKeys = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnableDownloadsWatch
        {
            get { return isEnableDownloadsWatch; }
            set
            {
                if(value == isEnableDownloadsWatch)
                    return;
                isEnableDownloadsWatch = value;
                NotifyPropertyChanged();
            }
        }


        public ICommand StartListenShortcutsCommand { get; set; }
        public ICommand StopListenShortcutsCommand { get; set; }

        public ICommand StartDownloadsWatchingCommand { get; private set; }
        public ICommand StopDownloadsWatchingCommand { get; private set; }


        public ICommand OpenHashTagFoldersWindowCommand { get; private set; }

        private void OpenHashTagFoldersWindowCommandAction()
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


        public ICommand ExitCommand { get; private set; }

        private void ExitCommandAction()
        {
            IsVisible = false;
            Application.Current.Shutdown();
        }


        public ICommand HideCommand { get; private set; }

        private void HideCommandAction()
        {
            IsVisible = false;
        }
    }
}