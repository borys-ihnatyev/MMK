using System.IO;
using MMK.Marking.Representation;
using MMK.MusicCollectionManager.Model;
using IOPath = System.IO.Path;

namespace MMK.MusicCollectionManager.ViewModel
{
    public class FileItemViewModel : ItemViewModel
    {
        public FileItemViewModel(string path)
            : base(path)
        {
            Update(path);
        }

        private string clearFileName;
        private string hashTagModelString;

        private void Update(string path)
        {
            var newClearFileName = IOPath.GetFileNameWithoutExtension(path);
            HashTagModelString = HashTagModel.Parser.All(ref newClearFileName).ToString();
            ClearFileName = newClearFileName;
        }

        public string FileName { 
            get; 
            private set; 
        }

        public string ClearFileName
        {
            get { return clearFileName; }
            set
            {
                if (value == clearFileName) return;

                clearFileName = value;
                NotifyPropertyChanged();
                TryUpdateFilePath();
            }
        }

        private void TryUpdateFilePath()
        {
            try
            {
                UpdateFilePath();
            }
            catch (IOException)
            {
                
            }
        }

        private void UpdateFilePath()
        {
            FileName = clearFileName + hashTagModelString + FileProcessing.Mp3Extension;
            var newPath = IOPath.Combine(FolderName, FileName);
            File.Move(Path, newPath);
            Path = newPath;
        }

        public string HashTagModelString
        {
            get { return hashTagModelString; }
            set
            {
                if (value == hashTagModelString) return;
                hashTagModelString = value;
                NotifyPropertyChanged();
            }
        }

        public override bool Process()
        {
            var path = Path;
            var isSuccess = OnProcessItem(ref path);
            Update(path);
            return isSuccess;
        }
    }
}
