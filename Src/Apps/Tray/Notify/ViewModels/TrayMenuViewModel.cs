using System.Windows.Input;
using MMK.Notify.Model.Service;
using MMK.Notify.Properties;
using MMK.Notify.Services;
using MMK.Presentation.Tools;
using MMK.Presentation.ViewModel;

namespace MMK.Notify.ViewModels
{
    public class TrayMenuViewModel : ViewModel
    {
        public TrayMenuViewModel(GlobalShortcutService shortcutService, IDownloadsWatcher downloadsWatcher)
        {
            StartListenShortcutsCommand = new Command(shortcutService.Start);
            StopListenShortcutsCommand = new Command(shortcutService.Stop);

            StartDownloadsWatchCommand = new Command(downloadsWatcher.Start);
            StopDownloadsWatchCommand = new Command(downloadsWatcher.Stop);
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

        protected override void OnLoadData()
        {
            if (IsEnableHotKeys)
                StartListenShortcutsCommand.Execute(null);

            if (IsEnableDownloadsWatch)
                StartDownloadsWatchCommand.Execute(null);
        }
    }
}