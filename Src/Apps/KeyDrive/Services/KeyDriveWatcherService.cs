using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using MMK.ApplicationServiceModel;
using MMK.KeyDrive.Models.Holders;
using MMK.KeyDrive.Models.Layout;
using MMK.KeyDrive.Observing.Tasks;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking;
using MMK.Notify.Observer.Tasking.Common;

namespace MMK.KeyDrive.Services
{
    public class KeyDriveWatcherService : InitializableService, IDisposable
    {
        private bool isDisposed;
        private readonly string watchRoot;

        private readonly INotifyObserver observer;
        private readonly FilesLayoutModel layout;
        private readonly List<FileSystemWatcher> watchers;

        public KeyDriveWatcherService(string watchRoot, string layoutRoot)
        {
            observer = IoC.Get<INotifyObserver>();
            isDisposed = false;
            this.watchRoot = watchRoot;

            layout = new FilesLayoutModel(layoutRoot);
            watchers = new List<FileSystemWatcher>(FileHolder.SupportedTypes.Length);
        }

        protected override void OnInitialize()
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
            observer.Observe(CreateHolderTasks(holder));
        }

        private IEnumerable<Task> CreateHolderTasks(Holder holder)
        {
            if (holder is FileHolder)
                return new[] {new LayoutResolveTask(holder.Info.FullName, layout)};

            Contract.Assume(holder is DirectoryHolder);

            return ((DirectoryHolder) holder).Files
                .Select(f => new LayoutResolveTask(f.Info.FullName, layout))
                .ToArray();
        }

        private void WatcherOnError(object sender, ErrorEventArgs e)
        {
            observer.Observe(new NotifyTask(
                new NotifyableException(e.GetException())
                {
                    CommonDescription = "MMK KeyDrive"
                })
                );
        }

        protected override void OnStart()
        {
            watchers.ForEach(w => w.EnableRaisingEvents = true);
        }

        protected override void OnStop()   
        {
            watchers.ForEach(w => w.EnableRaisingEvents = false);
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