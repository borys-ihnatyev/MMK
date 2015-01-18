using System.Windows;
using System.Windows.Forms;
using MMK.ApplicationServiceModel;
using MMK.Notify.Model.Launchers;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking.Common;
using MMK.Presentation.Providers;
using MMK.Processing.AutoFolder;

namespace MMK.Notify.Services
{
    public class GlobalShortcutService : Service
    {
        private readonly INotifyObserver observer;
        private readonly HashTagFolderCollection folderCollection;
        private readonly GlobalShortcutProviderCollection shortcutProviders;

        public GlobalShortcutService(
            INotifyObserver observer,
            HashTagFolderCollection folderCollection,
            GlobalShortcutProviderCollection shortcutProviders)
        {
            this.shortcutProviders = shortcutProviders;
            this.observer = observer;
            this.folderCollection = folderCollection;
        }

        protected override void OnInitialize()
        {
            var trayWindow = IoC.Get<TrayMenuService>().TrayMenuView;
            (shortcutProviders as IGlobalShortcutProvider).SetWindow(trayWindow);

            shortcutProviders.Add(new HotMarkLauncherGlobalShortcutProvider(KeyModifyers.Ctrl, (int) Keys.T));
            shortcutProviders.Add(KeyModifyers.Ctrl | KeyModifyers.Shift, (int) Keys.T, AddNormalizeHotKeyTasks);
            shortcutProviders.Add(KeyModifyers.Ctrl | KeyModifyers.Shift, (int) Keys.M, AddMoveFileToCollectionTasks);

            var swiftSearchLauncher = new SwiftSearchLauncherGlobalShortcutProvider();
            shortcutProviders.Add(swiftSearchLauncher);

            swiftSearchLauncher.SetStartShortcut(KeyModifyers.Ctrl, Keys.Space);
            swiftSearchLauncher.SetStartFromClipboardShortcut(KeyModifyers.Ctrl | KeyModifyers.Shift, Keys.V);
        }

        private void AddNormalizeHotKeyTasks()
        {
            var filePaths = Explorer.GetForegroundSelectedItemsFileTree(".mp3");
            observer.Observe(NormalizeTrackNameTask.Many(filePaths));
        }

        private void AddMoveFileToCollectionTasks()
        {
            var filePaths = Explorer.GetForegroundSelectedItemsFileTree(".mp3");
            observer.Observe(MoveFileToMusicFolderTask.Many(filePaths, folderCollection));
        }

        public override void Start()
        {
            shortcutProviders.StartListen();
        }

        public override void Stop()
        {
            shortcutProviders.StopListen();
        }
    }
}