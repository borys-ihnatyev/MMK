using System;
using MMK.Wpf.ViewModel;
using System.IO;
using IOPath = System.IO.Path;


namespace MMK.MusicCollectionManager.ViewModel
{
    public delegate bool FileItemProcessEvent(ref string path);

    /// <summary>
    /// Abstarct Factory
    /// </summary>
    public abstract class ItemViewModel : ObservableObject
    {
        #region Construct
        protected ItemViewModel(string path)
        {
            this.path = path;
            folderName = System.IO.Path.GetDirectoryName(path);
        }

        public static implicit operator ItemViewModel(string path)
        {
            if (File.Exists(path))
                return new FileItemViewModel(path);
            if (Directory.Exists(path))
                return new FolderItemViewModel(path);
            throw new ArgumentException(@"Path is not excists", "path");
        }
        #endregion

        private string path;
        private string folderName;

        private static FileItemProcessEvent processItem;

        public string Path
        {
            get { return path; }
            protected set
            {
                if (value == path) return;
                
                path = value;
                NotifyPropertyChanged();
                FolderName = IOPath.GetDirectoryName(path);
            }
        }

        public string FolderName
        {
            get { return folderName; }
            private set
            {
                if (value == folderName) return;

                folderName = value;
                NotifyPropertyChanged();
            }
        }

        public abstract bool Process();

        protected static bool OnProcessItem(ref string path)
        {
            if (processItem != null)
                return processItem(ref path);
            return false;
        }

        public static event FileItemProcessEvent ProcessItem
        {
            add
            {
                processItem = value;
            }
// ReSharper disable once ValueParameterNotUsed
            remove
            {
                processItem = null;
            }
        }
    }
}
