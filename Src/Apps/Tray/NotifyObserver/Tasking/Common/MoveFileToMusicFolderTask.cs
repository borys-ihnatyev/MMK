using System;
using System.IO;
using System.Runtime.Serialization;
using MMK.Notify.Observer.Tasking.Common.Base;
using MMK.Processing.AutoFolder;

namespace MMK.Notify.Observer.Tasking.Common
{
    [Serializable]
    internal sealed class MoveFileToMusicFolderTask : Mp3FileChangeTask
    {
        private readonly HashTagFolderCollection folderCollection;

        private MusicFolder.ResultInfo resultInfo;

        public MoveFileToMusicFolderTask(string oldPath, HashTagFolderCollection folderCollection) : base(oldPath)
        {
            this.folderCollection = folderCollection;
        }

        protected override void Initialize()
        {
            base.Initialize();
            NewFile = OldFile;
        }

        protected override void OnFileChange()
        {
            try
            {
                resultInfo = folderCollection.MoveFile(OldFile.FullName, NameModel.HashTagModel);
                NewFile = new FileInfo(resultInfo.NewFilePath);

                if(OldFile.FullName.Equals(NewFile.FullName,StringComparison.OrdinalIgnoreCase))
                    throw new Cancel();
            }
            catch (HashTagFolderCollection.NoMatchPatternException)
            {
                throw new NotifyableException
                {
                    TargetObject = TargetObject,
                    CommonDescription = "Matched pattern was not founded.",
                    DetailedDescription = "Matched pattern for " + NameModel.HashTagModel + " was not found.",
                };
            }
        }

        #region INotifyable

        protected override string CommonDescription
        {
            get { return "MoveTo file(s) to folder"; }
        }

        protected override string DetailedDescription
        {
            get
            {
                if (resultInfo == null) return string.Empty;

                return string.Format(
                    "File ({0})\n moved to ({1})", 
                    CleanName,
                    resultInfo.MusicFolderInnerPath
                );
            }
        }

        #endregion

        #region Serialization

        private const string FolderCollectionVar = "FolderCollection";

        private MoveFileToMusicFolderTask(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            folderCollection =
                info.GetValue(FolderCollectionVar, typeof (HashTagFolderCollection)) as HashTagFolderCollection;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(FolderCollectionVar,folderCollection,typeof(HashTagFolderCollection));
        }

        #endregion
    }
}
