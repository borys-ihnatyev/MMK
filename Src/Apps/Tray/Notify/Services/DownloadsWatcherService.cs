using System;
using MMK.ApplicationServiceModel;
using MMK.Notify.Model.Service;
using MMK.Notify.Services.DownloadWatcher;

namespace MMK.Notify.Services
{
    public class DownloadsWatcherService : InitializableService, IDownloadsWatcher
    {
        private IDownloadsWatcher downloadsWatcher;

        protected override void OnInitialize()
        {
            downloadsWatcher = new ChromeDownloadsWatcherService();
        }

        protected override void OnStart()
        {
            CheckInitialized();
            downloadsWatcher.Start();
        }

        protected override void OnStop()
        {
            CheckInitialized();
            downloadsWatcher.Stop();
        }

        public event EventHandler<FileDownloadedEventArgs> FileDownloaded
        {
            add
            {
                CheckInitialized();
                downloadsWatcher.FileDownloaded += value;
            }
            remove
            {
                CheckInitialized();
                downloadsWatcher.FileDownloaded -= value;
            }
        }
    }
}