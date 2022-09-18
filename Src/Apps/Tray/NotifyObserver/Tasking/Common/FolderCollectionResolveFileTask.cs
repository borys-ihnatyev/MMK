using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MMK.Notify.Observer.Tasking.Common.Base;
using MMK.Processing.AutoFolder;

namespace MMK.Notify.Observer.Tasking.Common
{
    public sealed class FolderCollectionResolveFileTask : FileTask
    {
        private MusicFolder.ResultInfo result;
        private readonly HashTagFolderCollection folderCollection;

        public FolderCollectionResolveFileTask(string filePath, HashTagFolderCollection folderCollection) : base(filePath)
        {
            this.folderCollection = folderCollection;
        }

        public FolderCollectionResolveFileTask(HashTagFolderCollection folderCollection)
        {
            this.folderCollection = folderCollection;
        }

        protected override INotifyable OnFileChange()
        {
            try
            {
                var hashTagModel = FileContext.FileHashTagModel;
                var filePath = FileContext.FileInfo.FullName;
                result = folderCollection.MoveFile(FileContext.FileInfo.FullName, hashTagModel);

                if (filePath.Equals(result.NewFilePath, StringComparison.OrdinalIgnoreCase))
                    throw new Cancel("Old and new file full names are equals.");
                FileContext.FileInfo.MoveTo(result.NewFilePath);

                return new NotifyMessage
                {
                    CommonDescription = "Folder resolve",
                    DetailedDescription =
                        string.Format("File ({0})\n moved to ({1})", 
                            FileContext.FileNameWithoutHashTagModel,
                            result.MusicFolderInnerPath),
                    TargetObject = Context.ToString(),
                };
            }
            catch (HashTagFolderCollection.NoMatchPatternException)
            {
                throw new NotifyableException
                {
                    TargetObject = Context.ToString(),
                    CommonDescription = "Matched pattern was not founded.",
                    DetailedDescription = "Matched pattern for " + FileContext.FileHashTagModel + " was not found."
                };
            }
        }

        public static IEnumerable<Task> Many(IEnumerable<string> paths, HashTagFolderCollection folderCollection)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");
            if (folderCollection == null)
                throw new ArgumentNullException("folderCollection");
            Contract.EndContractBlock();

            return paths.Distinct().Select(p => new FolderCollectionResolveFileTask(p, folderCollection));
        }
    }
}