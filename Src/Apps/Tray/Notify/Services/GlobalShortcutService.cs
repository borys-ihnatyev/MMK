using System.Windows;
using System.Windows.Input;
using MMK.ApplicationServiceModel;
using MMK.Notify.Model.Launchers;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking.Common;
using MMK.Presentation.Windows.Input;
using MMK.Processing.AutoFolder;
using IKey = System.Windows.Input.Key;

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
            var hotMarkLauncher = new HotMarkLauncherGlobalShortcutProvider(ModifierKeys.Control, IKey.T);
            shortcutProviders.Add(hotMarkLauncher);
            shortcutProviders.Add(ModifierKeys.Control | ModifierKeys.Shift, IKey.T, AddNormalizeHotKeyTasks);
            shortcutProviders.Add(ModifierKeys.Control | ModifierKeys.Shift, IKey.M, AddMoveFileToCollectionTasks);

            var swiftSearchLauncher = new SwiftSearchLauncherGlobalShortcutProvider();
            shortcutProviders.Add(swiftSearchLauncher);

            swiftSearchLauncher.SetStartShortcut(ModifierKeys.Control, IKey.Space);
            swiftSearchLauncher.SetStartFromClipboardShortcut(ModifierKeys.Control | ModifierKeys.Shift, IKey.V);
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