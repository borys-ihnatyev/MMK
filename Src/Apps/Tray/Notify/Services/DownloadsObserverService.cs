using System;
using System.IO;
using MMK.ApplicationServiceModel;
using MMK.Marking.Representation;
using MMK.Notify.Model.Service;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking;
using MMK.Notify.Observer.Tasking.Common;

namespace MMK.Notify.Services
{
    public class DownloadsObserverService : IService
    {
        private readonly IDownloadsWatcher watcher;
        private readonly INotifyObserver observer;
        private bool isStarted;

        public DownloadsObserverService(INotifyObserver observer, IDownloadsWatcher watcher)
        {
            isStarted = false;
            this.observer = observer;
            this.watcher = watcher;
        }

        private void OnFileDownloaded(object sender, FileDownloadedEventArgs e)
        {
            observer.Observe(GetTaskForFile(new FileInfo(e.FilePath)), TimeSpan.FromSeconds(4));
        }

        private static Task GetTaskForFile(FileInfo fileInfo)
        {
            if (IsAudioMixFile(fileInfo))
                return new AddHashTagModelTask(fileInfo.FullName, HashTagModel.Parser.All("#mixes"));
            return new AddHashTagModelTask(fileInfo.FullName, HashTagModel.Parser.All("#unch"));
        }

        private static bool IsAudioMixFile(FileInfo fileInfo)
        {
            using (var file = TagLib.File.Create(fileInfo.FullName))
                return file.Properties.Duration >= MixMinimumDuration;
        }

        public static readonly TimeSpan MixMinimumDuration = TimeSpan.FromMinutes(25);

        public void Start()
        {
            if (isStarted)
                return;
            watcher.FileDownloaded += OnFileDownloaded;
            isStarted = true;
        }

        public void Stop()
        {
            if (!isStarted)
                return;
            watcher.FileDownloaded -= OnFileDownloaded;
            isStarted = false;
        }
    }
}