using System;
using System.IO;
using MMK.ApplicationServiceModel;
using MMK.Notify.Model.Service;
using MMK.Utils;
using Newtonsoft.Json;

namespace MMK.Notify.Services.DownloadWatcher
{
    public sealed class ChromeDownloadsWatcherService : InitializableService, IDownloadsWatcher
    {
        private string downloadsPath;

        private FileSystemWatcher fileWatcher;

        protected override void OnInitialize()
        {
            downloadsPath = GetChromeDownloadsPath();
            fileWatcher = new FileSystemWatcher(downloadsPath);
            fileWatcher.Renamed += OnRenamed;
        }

        private static string GetChromeDownloadsPath()
        {
            var settingsDirPath = Environment.ExpandEnvironmentVariables("%USERPROFILE%") +
                                  @"\AppData\Local\Google\Chrome\User Data\Default";

            if (!Directory.Exists(settingsDirPath))
                throw new DirectoryNotFoundException("Chrome settings directory not found");

            var preferencesFilePath = Path.Combine(settingsDirPath, "Preferences");

            if (!File.Exists(preferencesFilePath))
                throw new FileNotFoundException("Chrome settings prefences file not found");

            string settingsJson;

            using (var reader = new StreamReader(preferencesFilePath))
                settingsJson = reader.ReadToEnd();

            dynamic settings = JsonConvert.DeserializeObject(settingsJson);

            return settings.download.default_directory;
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (FileExtensionParser.IsChromeDownload(e.OldFullPath) && FileExtensionParser.IsMp3(e.FullPath))
                OnFileDownloaded(e.FullPath);
        }

        private void OnFileDownloaded(string filePath)
        {
            var handler = FileDownloaded;
            if (handler != null)
                handler(this, new FileDownloadedEventArgs(filePath));
        }

        protected override void OnStart()
        {
            fileWatcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
            fileWatcher.EnableRaisingEvents = false;
        }

        public event EventHandler<FileDownloadedEventArgs> FileDownloaded;
    }
}