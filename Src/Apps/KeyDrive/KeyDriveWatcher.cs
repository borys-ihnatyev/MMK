using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Windows;
using MMK.KeyDrive.Models.Holders;
using MMK.KeyDrive.Models.Layout;
using MMK.KeyDrive.Observing.Tasks;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Remoting;
using MMK.Notify.Observer.Tasking;
using MMK.Notify.Observer.Tasking.Common;

namespace MMK.KeyDrive
{
    public class KeyDriveWatcher : IDisposable
    {
        private bool isDisposed;
        private readonly string watchRoot;

        private readonly FilesLayoutModel layout;
        private readonly List<FileSystemWatcher> watchers;

        public KeyDriveWatcher(string watchRoot, string layoutRoot)
        {
            isDisposed = false;
            this.watchRoot = watchRoot;

            layout = new FilesLayoutModel(layoutRoot);
            watchers = new List<FileSystemWatcher>(FileHolder.SupportedTypes.Length);
            Initialize();
        }

        private static INotifyObserver observer;

        private static INotifyObserver Observer
        {
            get
            {
                if (observer == null)
                {
                    var connector = new NotifyObserverConnector(Application.Current);
                    connector.Connect();
                    observer = connector.NotifyObserver;
                }
                return observer;
            }
        }

        private void Initialize()
        {
            foreach (var supportedType in FileHolder.SupportedTypes)
                watchers.Add(CreateWatcher("*" + supportedType));
        }

        private FileSystemWatcher CreateWatcher(string fileFilter)
        {
            var watcher = new FileSystemWatcher(watchRoot)
            {
                Filter = fileFilter,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true
            };

            watcher.Renamed += WatcherOnNotify;
            watcher.Created += WatcherOnNotify;
            watcher.Error += WatcherOnError;

            return watcher;
        }

        private void WatcherOnNotify(object sender, FileSystemEventArgs e)
        {
            Observe(e.FullPath);
        }

        private void Observe(string path)
        {
            var holder = Holder.Build(path);
            Observer.Observe(CreateHolderTasks(holder));
        }

        private IEnumerable<Task> CreateHolderTasks(Holder holder)
        {
            if (holder is FileHolder)
                return new[] {new LayoutResolveTask(holder.Info.FullName, layout)};

            Contract.Assume(holder is DirectoryHolder);

            return (holder as DirectoryHolder).Files
                .Select(f => new LayoutResolveTask(f.Info.FullName, layout))
                .ToArray();
        }

        private static void WatcherOnError(object sender, ErrorEventArgs e)
        {
            Observer.Observe(new NotifyTask(
                new NotifyableException(e.GetException())
                {
                    CommonDescription = "MMK KeyDrive"
                })
                );
        }

        public void Start()
        {
            watchers.ForEach(w => w.EnableRaisingEvents = true);
        }

        public void Stop()
        {
            watchers.ForEach(w => w.EnableRaisingEvents = false);
        }

        ~KeyDriveWatcher()
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