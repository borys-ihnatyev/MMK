using System;
using System.IO;
using MMK.Utils;

namespace MMK.Notify.Model
{
    /// <summary>
    /// TODO : provide for each browser specific (now is only for CHROME)
    /// </summary>
    sealed public class MusicDownloadsWatcher
    {
        private static readonly string DefaultDownloadsPath = Environment.ExpandEnvironmentVariables("%USERPROFILE%") + @"\Downloads\";

        public MusicDownloadsWatcher()
        {
            fileWatcher = new FileSystemWatcher(DefaultDownloadsPath);
            fileWatcher.Renamed += OnRenamed;
        }

        private readonly FileSystemWatcher fileWatcher;

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

        public void Start()
        {
            fileWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            fileWatcher.EnableRaisingEvents = false;
        }

        public event Action<string> FileDownloaded;
    }
}
