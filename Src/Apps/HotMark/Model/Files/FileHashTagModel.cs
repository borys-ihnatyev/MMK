using MMK.Marking.Representation;
using IOPath = System.IO.Path;

namespace MMK.HotMark.Model.Files
{
    public class FileHashTagModel
    {
        private readonly string path;
        private string clearName;
        private HashTagModel hashTagModel;

        public FileHashTagModel(string path)
        {
            this.path = path;
        }

        public HashTagModel HashTagModel
        {
            get
            {
                if (hashTagModel == null)
                    WakeUp();
                return hashTagModel;
            }
        }

        public string Path
        {
            get { return path; }
        }

        public string ClearName
        {
            get
            {
                if (clearName == null)
                    WakeUp();
                return clearName;
            }
        }

        private void WakeUp()
        {
            clearName = IOPath.GetFileNameWithoutExtension(path);
            hashTagModel = HashTagModel.Parser.All(ref clearName);
        }

        public static implicit operator FileHashTagModel(string filePath)
        {
            var fileItemViewModel = new FileHashTagModel(filePath);
            return fileItemViewModel;
        }

        public static implicit operator string(FileHashTagModel fileHashTagModel)
        {
            return fileHashTagModel.Path;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}