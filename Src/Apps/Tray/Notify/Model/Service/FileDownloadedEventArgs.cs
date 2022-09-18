using System;

namespace MMK.Notify.Model.Service
{
    public class FileDownloadedEventArgs : EventArgs
    {
        public FileDownloadedEventArgs(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; private set; }
    }
}