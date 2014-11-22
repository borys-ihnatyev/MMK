using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;

namespace MMK.Notify.Observer.Tasking.Common.Base
{
    [Serializable]
    public abstract class FileChangeTask : Task
    {
        protected FileChangeTask(string oldPath)
        {
            if(oldPath == null)
                throw new ArgumentNullException();
            Contract.EndContractBlock();
            
            OldFile = new FileInfo(oldPath);
        }

        public FileInfo OldFile { get; private set; }
        public FileInfo NewFile { get; protected set; }

        public string CleanName { get; private set; }

        protected override void Initialize()
        {
            CleanName = Path.GetFileNameWithoutExtension(OldFile.FullName);
        }

        protected override sealed void OnRun()
        {
            TryFileChange();
        }

        private void TryFileChange()
        {
            try
            {
                OnFileChange();
            }
            catch (FileNotFoundException)
            {
                throw new Cancel();
            }
            catch (IOException ex)
            {
                ThrowAsNotifyableException(ex, true);
            }
        }

        protected virtual void OnFileChange()
        {
            Contract.Requires(NewFile != null);
            Contract.EndContractBlock();

            if (OldFile.FullName.Equals(NewFile.FullName, StringComparison.Ordinal))
                throw new Cancel();

            OldFile.MoveTo(NewFile);
        }

        protected override string TargetObject
        {
            get { return string.Format("{0}\n{1}", OldFile.Directory, CleanName); }
        }

        #region Serialization

        protected const string OldPathVar = "OldPath";

        protected FileChangeTask(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            OldFile = new FileInfo(info.GetString(OldPathVar));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(OldPathVar,OldFile.FullName);
        }

        #endregion
    }
}