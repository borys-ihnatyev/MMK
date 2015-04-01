using System.Collections.Generic;
using System.Linq;
using MMK.ApplicationServiceModel;
using MMK.KeyDrive.Models.IO;

namespace MMK.KeyDrive.Services
{
    public class KeyDriveWatcherService : IService
    {
        private bool isDisposed;
        private bool isRunning;
        private readonly LinkedList<KeyDriveWatcher> watchers;

        public KeyDriveWatcherService()
        {
            watchers = new LinkedList<KeyDriveWatcher>();
        }

        public void AddWatcher(string watchRoot, string layoutRoot)
        {
            var watcher = GetWatcher(watchRoot);
            if (watcher != null)
                return;
            watcher = new KeyDriveWatcher(watchRoot, layoutRoot);
            watcher.Initialize();
            watchers.AddLast(watcher);
            if (isRunning)
                watcher.Start();
        }

        private KeyDriveWatcher GetWatcher(string watchRoot)
        {
            return watchers.FirstOrDefault(w => w.WatchRoot == watchRoot);
        }

        public void RemoveWatcher(string watchRoot)
        {
            var watcher = GetWatcher(watchRoot);
            if (watcher == null)
                return;
            if (watcher.IsRunning)
                watcher.Stop();
            watchers.Remove(watcher);
        }

        public void Start()
        {
            watchers.ForEach(w => w.Start());
            isRunning = true;
        }

        public void Stop()
        {
            watchers.ForEach(w => w.Stop());
            isRunning = false;
        }

        ~KeyDriveWatcherService()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (isDisposed) return;

            watchers.ForEach(w => w.Dispose());
            isDisposed = true;
        }
    }
}