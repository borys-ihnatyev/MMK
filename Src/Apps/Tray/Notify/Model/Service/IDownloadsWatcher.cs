using System;
using MMK.ApplicationServiceModel;

namespace MMK.Notify.Model.Service
{
    internal interface IDownloadsWatcher : IService
    {
        void Initialize();
        event EventHandler<FileDownloadedEventArgs> FileDownloaded;
    }
}