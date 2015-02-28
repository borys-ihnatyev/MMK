using System;
using System.IO;
using System.Linq;
using MMK.KeyDrive.Models.Holders;
using MMK.KeyDrive.Models.Layout;
using MMK.Notify.Observer;
using MMK.Notify.Observer.Tasking.Common.Base;

namespace MMK.KeyDrive.Observing.Tasks
{
    public sealed class LayoutResolveTask : FileTask
    {
        private readonly FilesLayoutModel layout;
        private DirectoryInfo[] targetDirectories;

        public LayoutResolveTask(string path, FilesLayoutModel layout)
            : base(path)
        {
            this.layout = layout;
        }

        public LayoutResolveTask(FilesLayoutModel layout)
        {
            this.layout = layout;
        }

        protected override INotifyable OnFileChange()
        {
            Initialize();
            Move();
            CleanUp();
            return new NotifyMessage {CommonDescription = "Layout resolved"};
        }

        private void Initialize()
        {
            targetDirectories = layout[FileContext.FileHashTagModel];
        }

        private void Move()
        {
            if (targetDirectories.Length == 1)
                MoveSingleDir();
            else
                MoveMultiDirs();
        }

        private void MoveSingleDir()
        {
            var dir = new DirectoryHolder(targetDirectories[0].FullName);
            var newFile = new FileInfo(Path.Combine(dir.Info.FullName, FileContext.FileInfo.Name));

            if (newFile.FullName.Equals(FileContext.FileInfo.FullName, StringComparison.Ordinal))
                throw new Cancel("Target file is already in layout FileContext");

            if (newFile.Exists)
            {
                newFile.Delete();
                FileContext.FileInfo.MoveTo(dir.DirInfo);
                throw new Cancel();
            }

            FileContext.FileInfo.MoveTo(dir.DirInfo);
        }

        private void MoveMultiDirs()
        {
            var allCopied = targetDirectories.Aggregate(
                true,
                (current, dir) => current & (FileContext.FileInfo.TryCopyTo(dir) != null)
                );

            if (!allCopied) return;

            FileContext.FileInfo.Wait();
            FileContext.FileInfo.Delete();
        }

        private void CleanUp()
        {
            if (layout.RootHolder.Contains(FileContext.FileInfo))
                layout.CleanUp();
        }
    }
}