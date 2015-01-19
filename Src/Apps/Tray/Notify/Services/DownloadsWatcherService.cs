using System;
using MMK.ApplicationServiceModel;
using MMK.Marking.Representation;
using MMK.Notify.Model.Service;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking.Common;
using MMK.Notify.Services.DownloadWatcher;

namespace MMK.Notify.Services
{
    public class DownloadsWatcherService : InitializableService, IDownloadsWatcher
    {
        private IDownloadsWatcher watcher;
        private readonly INotifyObserver observer;

        public DownloadsWatcherService(INotifyObserver observer)
        {
            this.observer = observer;
        }

        protected override void OnInitialize()
        {
            watcher = GetDefaultBrowserDownloadsWatcher();
            watcher.Initialize();
            watcher.FileDownloaded += OnFileDownloaded;
        }

        private static IDownloadsWatcher GetDefaultBrowserDownloadsWatcher()
        {
            return new ChromeDownloadsWatcherService();
        }

        private void OnFileDownloaded(object sender, FileDownloadedEventArgs e)
        {
            var task = new AddHashTagModelTask(e.FilePath, HashTagModel.Parser.All("#unch"));
            observer.Observe(task, TimeSpan.FromSeconds(4));
        }

        protected override void OnStart()
        {
            CheckInitialized();
            watcher.Start();
        }

        protected override void OnStop()
        {
            CheckInitialized();
            watcher.Stop();
        }

        public event EventHandler<FileDownloadedEventArgs> FileDownloaded
        {
            add
            {
                CheckInitialized();
                watcher.FileDownloaded += value;
            }
            remove
            {
                CheckInitialized();
                watcher.FileDownloaded -= value;
            }
        }
    }
}