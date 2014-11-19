using System;
using System.Collections.Generic;
using System.ComponentModel;
using MMK.MusicCollectionManager.Model;
using System.IO;

namespace MMK.MusicCollectionManager.ViewModel
{
    public class MergedItemViewModel : INotifyPropertyChanged, IEnumerable<string>
    {
        public MergedItemViewModel(string path)
        {
            this.path = path;
            progressValue = 0;
        }

        private string path;

        private double progressValue;

        private IList<string> filePaths;

        public int Count
        {
            get { return filePaths.Count; }
        }

        public string Path
        {
            get { return path; }
            set
            {
                if (value != path)
                {
                    path = value;
                    NotifyChange("Path");
                }
            }
        }

        public double ProgressValue
        {
            get { return progressValue; }
            set
            {
                if (!(Math.Abs(value - progressValue) > 0.01)) return;
                progressValue = value;
                NotifyChange("ProgressValue");
            }
        }

        public void Load()
        {
            filePaths = FileProcessing.IsDirectory(path)
                ? Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories)
                : new[] { path };
        }

        public void Unload()
        {
            if (Count > 1)
                filePaths = null;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return filePaths.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public static implicit operator MergedItemViewModel(string path)
        {
            return new MergedItemViewModel(path);
        }
    }
}
