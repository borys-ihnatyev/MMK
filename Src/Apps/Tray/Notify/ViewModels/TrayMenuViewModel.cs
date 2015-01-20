using System.Windows;
using System.Windows.Input;
using MMK.ApplicationServiceModel;
using MMK.Notify.Model.Service;
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

        public TrayMenuViewModel()
        {
            StartListenShortcutsCommand = new Command(IoC.Get<GlobalShortcutService>().Start);
            StopListenShortcutsCommand = new Command(IoC.Get<GlobalShortcutService>().Stop);

            StartDownloadsWatchCommand = new Command(IoC.Get<IDownloadsWatcher>().Start);
            StopDownloadsWatchCommand = new Command(IoC.Get<IDownloadsWatcher>().Stop);
        }

        public bool IsEnableHotKeys
        {
            get { return Settings.Default.IsEnableHotKeys; }
            set
            {
                if (value == IsEnableHotKeys)
                    return;
                Settings.Default.IsEnableHotKeys = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnableDownloadsWatch
        {
            get { return Settings.Default.IsEnableDownloadsWatch; }
            set
            {
                if (value == IsEnableDownloadsWatch)
                    return;
                Settings.Default.IsEnableDownloadsWatch = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand StartListenShortcutsCommand { get; set; }
        public ICommand StopListenShortcutsCommand { get; set; }
        public ICommand StartDownloadsWatchCommand { get; private set; }
        public ICommand StopDownloadsWatchCommand { get; private set; }
        public ICommand OpenHashTagFoldersWindowCommand { get; private set; }

        protected override void OnLoadData()
        {
            if (IsEnableHotKeys)
                StartListenShortcutsCommand.Execute(null);

            if (IsEnableDownloadsWatch)
                StartDownloadsWatchCommand.Execute(null);
        }

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