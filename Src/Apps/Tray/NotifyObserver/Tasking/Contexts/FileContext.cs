using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using MMK.Marking.Representation;
using MMK.Utils;

namespace MMK.Notify.Observer.Tasking.Contexts
{
    [Serializable]
    public class FileContext : ITaskContext
    {
        protected FileContext(string filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException("filePath");
            Contract.EndContractBlock();
            FileInfo = new FileInfo(filePath);
        }

        public FileInfo FileInfo { get; private set; }

        public HashTagModel FileHashTagModel
        {
            get { return HashTagModel.Parser.All(FileNameWithoutExtension); }
        }

        public string FileNameWithoutExtension
        {
            get { return Path.GetFileNameWithoutExtension(FileInfo.FullName); }
        }

        public string FileNameWithoutHashTagModel
        {
            get
            {
                var fileName = FileNameWithoutExtension;
                HashTagModel.Parser.All(ref fileName);
                return fileName;
            }
        }

        #region Serializetion

        private const string FilePathPropertyName = "FileInfo";

        protected FileContext(SerializationInfo info, StreamingContext context)
            : this(info.GetString(FilePathPropertyName))
        {
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(FilePathPropertyName, FileInfo.FullName);
        }

        #endregion

        public override string ToString()
        {
            return FileNameWithoutHashTagModel;
        }

        public static FileContext Build(string filePath)
        {
            return FileExtensionParser.IsMp3(filePath)
                ? new AudioFileContext(filePath)
                : new FileContext(filePath);
        }
    }
}