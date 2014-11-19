using System.Linq;
using MMK.MusicCollectionManager.Model;
using System.IO;

namespace MMK.MusicCollectionManager.ViewModel
{
    public class FolderItemViewModel : ItemViewModel
    {
        public FolderItemViewModel(string path)
            : base(path)
        {

        }

        public override bool Process()
        {
            var filePaths = Directory.GetFiles(Path, "*" + FileProcessing.Mp3Extension);
            return filePaths.Aggregate(true, (current, filePathCopy) => current & OnProcessItem(ref filePathCopy));
        }
    }
}
