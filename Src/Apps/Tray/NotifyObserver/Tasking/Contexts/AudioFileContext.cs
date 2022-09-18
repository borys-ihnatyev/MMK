using System;
using System.IO;
using System.Runtime.Serialization;
using MMK.Marking.Representation;

namespace MMK.Notify.Observer.Tasking.Contexts
{
    [Serializable]
    public class AudioFileContext : FileContext
    {
        [NonSerialized] private TrackNameModel trackNameModel;

        protected internal AudioFileContext(string filePath) : base(filePath)
        {
        }

        public TrackNameModel NameModel
        {
            get { return trackNameModel ?? (trackNameModel = TrackNameModel.Parser.Parse(FileNameWithoutExtension)); }
        }

        #region Serialization

        protected AudioFileContext(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion

        public string NormalizedFilePath
        {
            get { return Path.Combine(FileInfo.DirectoryName, NameModel.FullName + FileInfo.Extension); }
        }

        public bool IsNormalizedFileName
        {
            get { return FileInfo.FullName.Equals(NormalizedFilePath, StringComparison.OrdinalIgnoreCase); }
        }
    }
}