using System;
using System.IO;
using MMK.ApplicationServiceModel;
using MMK.Notify.Model.Service;
using MMK.Utils;

namespace MMK.Notify.Services
{
    public sealed class ChromeDownloadsWatcherService : Service, IDownloadsWatcher
    {
        private static readonly string DefaultDownloadsPath;

        private readonly FileSystemWatcher fileWatcher;

        static ChromeDownloadsWatcherService()
        {
            DefaultDownloadsPath = Environment
                .ExpandEnvironmentVariables("%USERPROFILE%") + @"\Downloads\";
        }

        public ChromeDownloadsWatcherService()
        {
            fileWatcher = new FileSystemWatcher(DefaultDownloadsPath);
            fileWatcher.Renamed += OnRenamed;
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (FileExtensionParser.IsChromeDownload(e.OldFullPath) && FileExtensionParser.IsMp3(e.FullPath))
                OnFileDownloaded(e.FullPath);
        }

        private void OnFileDownloaded(string filePath)
        {
            if (FileDownloaded != null)
                FileDownloaded(filePath);
        }

        public override void Start()
        {
            fileWatcher.EnableRaisingEvents = true;
        }

        public override void Stop()
        {
            fileWatcher.EnableRaisingEvents = false;
        }

        public event Action<string> FileDownloaded;
    }
}