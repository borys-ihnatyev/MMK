using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MMK.Notify.Model;
using MMK.Notify.Model.Launchers;
using MMK.Notify.View;
using MMK.Wpf;

namespace MMK.Notify.ViewModel.TrayMenu
{
    public class TrayMenuViewModel : Wpf.ViewModel.ViewModel
    {
        private bool isVisible = true;
        private Window hashTagFoldersWindow;

        private ToggleMenuItemViewModel enableDisableHotKeysMenuItem;
        private ToggleMenuItemViewModel startStopMusicDownloadWatching;

        public TrayMenuViewModel()
        {
            CloseCommand = new Command(CloseCommandAction);
            MenuItems = new ObservableCollection<MenuItemViewModel>();
        }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

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


        protected override void OnLoadData()
        {
            LoadMenuItems();
        }
       
        private void LoadMenuItems()
        {
            MenuItems.Add(
                new MenuItemViewModel("Music collection manager",
                () => MusicCollectionManagerLauncher.Instance.Launch(),
                @"pack://siteoforigin:,,,/Resources/musiccollectionmanagerlogo.ico")
            );

            startStopMusicDownloadWatching = CreateStartStopDownloadsWatchingMenuItem();
            MenuItems.Add(startStopMusicDownloadWatching);

            enableDisableHotKeysMenuItem = CreateEnableDisableHotKeysMenuItem();
            MenuItems.Add(enableDisableHotKeysMenuItem);

            MenuItems.Add(new MenuItemViewModel(
                "Save configuration",
                SaveCurrentSettingsCommandAction,
                @"pack://siteoforigin:,,,/Resources/settings-26.png"
                )
            );

            MenuItems.Add(new MenuItemViewModel(
                "Music folder collection manager",
                OpenHashTagFoldersWindowCommandAction
                )
            );
            
            MenuItems.Add(new MenuItemViewModel(
                "Close",
                ExitCommandAction,
                @"pack://siteoforigin:,,,/Resources/exit-26.png"
                )
            );
        }

        

        private static ToggleMenuItemViewModel CreateEnableDisableHotKeysMenuItem()
        {
            var enableHotKeysMenuItem = new MenuItemViewModel(
                "Hot keys",
                App.Current.StartListenShortcuts,
                @"pack://siteoforigin:,,,/Resources/keyboard-26.png"
                );

            var disableHotKeysMenuItem = new MenuItemViewModel(
                "Hot keys",
                App.Current.StopListenShortcuts,
                @"pack://siteoforigin:,,,/Resources/keyboard-26.png"
                );

            var hotKeysMenuItem = new ToggleMenuItemViewModel(
                enableHotKeysMenuItem,
                disableHotKeysMenuItem
                );

            if (App.Settings.IsEnableHotKeysMenuItem)
                hotKeysMenuItem.Press();

            return hotKeysMenuItem; 
        }

        private static ToggleMenuItemViewModel CreateStartStopDownloadsWatchingMenuItem()
        {
            var startMusicDownloadWatching = new MenuItemViewModel(
                "Downloads watching",
                App.Current.MusicDownloadsWatcher.Start,
                @"pack://siteoforigin:,,,/Resources/glasses-26.png"
                );

            var stopMusicDownloadWatching = new MenuItemViewModel(
                "Downloads watching",
                App.Current.MusicDownloadsWatcher.Stop,
                @"pack://siteoforigin:,,,/Resources/glasses-26.png"
                );

            var musicDownloadWatching = new ToggleMenuItemViewModel(
                startMusicDownloadWatching,
                stopMusicDownloadWatching
            );

            if (App.Settings.IsStartMusicDownloadWatching)
                musicDownloadWatching.Press();

            return musicDownloadWatching;
        }

        private void SaveCurrentSettingsCommandAction()
        {
            App.Settings.IsStartMusicDownloadWatching = startStopMusicDownloadWatching.IsPressed;
            App.Settings.IsEnableHotKeysMenuItem = enableDisableHotKeysMenuItem.IsPressed;
            App.Settings.FolderCollection = App.Current.FolderCollection;

            App.Settings.Save();
        }

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

        private void ExitCommandAction()
        {
            IsVisible = false;
            App.Current.Shutdown();
        }

        public ICommand CloseCommand { get; private set; }

        private void CloseCommandAction()
        {
            IsVisible = false;
        }
    }
}