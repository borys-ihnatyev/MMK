using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using MMK.KeyDrive.Models.Holders;
using MMK.KeyDrive.Models.Layout;
using MMK.Marking.Representation;
using MMK.Notify.Observer.Tasking;

namespace MMK.KeyDrive.Observing.Tasks
{
    [Serializable]
    public sealed class LayoutResolveTask : Task
    {
        private readonly string path;
        private readonly FilesLayoutModel layout;
        private FileHolder holder;
        private DirectoryInfo[] targetDirectories;

        public LayoutResolveTask(string path, FilesLayoutModel layout)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (layout == null)
                throw new ArgumentNullException("layout");
            Contract.EndContractBlock();

            this.path = path;
            this.layout = layout;
        }

        protected override void Initialize()
        {
            try
            {
                holder = new FileHolder(path);
                var hashTagModel = HashTagModel.Parser.All(holder.NameWithoutExtension);
                targetDirectories = layout[hashTagModel];
            }
            catch (Holder.Exception)
            {
                throw new Cancel();
            }
        }

        protected override void OnRun()
        {
            Move();
            CleanUp();
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
            var newFile = new FileInfo(Path.Combine(dir.Info.FullName, holder.FileInfo.Name));

            if (newFile.FullName == holder.FileInfo.FullName)
                throw new Cancel();

            if (newFile.Exists)
            {
                newFile.Delete();
                holder.FileInfo.MoveTo(dir.DirInfo);
                throw new Cancel();
            }

            holder.FileInfo.MoveTo(dir.DirInfo);
        }

        private void MoveMultiDirs()
        {
            var allCopied = targetDirectories.Aggregate(
                true,
                (current, dir) => current & (holder.FileInfo.TryCopyTo(dir) != null)
                );

            if (!allCopied) return;

            holder.FileInfo.Wait();
            holder.FileInfo.Delete();
        }

        private void CleanUp()
        {
            if (layout.RootHolder.Contains(holder))
                layout.CleanUp();
        }

        #region INotifyable

        protected override string TargetObject
        {
            get { return holder.NameWithoutExtension; }
        }

        protected override string CommonDescription
        {
            get { return "Resolve item(s)"; }
        }

        protected override string DetailedDescription
        {
            get { return layout.RootHolder + " :\n" + String.Join(", ", targetDirectories.Select(d => d.Name)); }
        }

        #endregion

        #region Serialization

        private const string PathVarName = "path";
        private const string LayoutVarName = "layout";

        public LayoutResolveTask(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info.GetString(PathVarName) == null)
                throw new ArgumentNullException("info");
            if (!(info.GetValue(LayoutVarName, typeof (FilesLayoutModel)) is FilesLayoutModel))
                throw new ArgumentNullException(LayoutVarName);
            Contract.EndContractBlock();

            layout = (FilesLayoutModel) info.GetValue(LayoutVarName, typeof (FilesLayoutModel));
            path = info.GetString(PathVarName);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(PathVarName, path);
            info.AddValue(LayoutVarName, layout, layout.GetType());
        }

        #endregion
    }
}