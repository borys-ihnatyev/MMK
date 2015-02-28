using System;
using MMK.ApplicationServiceModel;
using MMK.Marking.Representation;
using MMK.Notify.Model.Service;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking.Common;
using MMK.Notify.Observer.Tasking.Contexts;
using MMK.Processing.AutoFolder;

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
            var deelay = TimeSpan.FromSeconds(4);
            var pipe = observer.Using(FileContext.Build(e.FilePath));

            if (IsAudioMixFile(e.FilePath))
                pipe.Pipe(new AddHashTagModelTask(HashTagModel.Parser.All("#mixes")))
                    .Pipe(new FolderCollectionResolveFileTask(IoC.Get<HashTagFolderCollection>()));
            else
                pipe.Pipe(new AddHashTagModelTask(HashTagModel.Parser.All("#unch #new")));

            pipe.Observe(deelay);
        }

        private static bool IsAudioMixFile(string filePath)
        {
            using (var file = TagLib.File.Create(filePath))
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