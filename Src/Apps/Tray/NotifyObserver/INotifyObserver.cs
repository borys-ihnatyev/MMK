using System.Collections.Generic;
using MMK.Marking.Representation;
using MMK.Notify.Observer.Tasking;
using MMK.Processing.AutoFolder;

namespace MMK.Notify.Observer
{
    public interface INotifyObserver
    {
        void TestConnection();

        void Observe(Task task);
        void Observe(IEnumerable<Task> task);

        void ChangeHashTagModel(string path, HashTagModel add, HashTagModel remove);
        void ChangeHashTagModel(IEnumerable<string> path, HashTagModel add, HashTagModel remove);

        void RewriteHashTagModel(string path, HashTagModel hashTagModel);
        void RewriteHashTagModel(IEnumerable<string> paths, HashTagModel hashTagModel);
        
        void AddHashTagModel(string path, HashTagModel hashTagModel);
        void AddHashTagModel(IEnumerable<string> paths, HashTagModel hashTagModel);

        void NormalizeTrackName(string path);
        void NormalizeTrackName(IEnumerable<string> paths);

        void MoveToCollectionFolder(string path, HashTagFolderCollection collection);
        void MoveToCollectionFolder(IEnumerable<string> paths, HashTagFolderCollection collection);
    }
}