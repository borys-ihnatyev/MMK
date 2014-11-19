using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using IOPath = System.IO.Path;

namespace MMK.KeyDrive.Models.Holders
{
    [Serializable]
    public class FileHolder : Holder
    {
        public static readonly string[] SupportedTypes =
        {
            ".mp3", ".wav", ".aac", ".aif"
        };

        internal FileHolder(string path) : base(path)
        {
            Initialize(path);
        }

        protected FileHolder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Initialize(info.GetString(PathName));
        }

        private void Initialize(string path)
        {
            if (!IsSupported(path))
                throw new NotSupportedException(path);
            Contract.EndContractBlock();

            FileInfo = new FileInfo(path);
            NameWithoutExtension = IOPath.GetFileNameWithoutExtension(FileInfo.FullName);
        }

        public override FileSystemInfo Info
        {
            get
            {
                return FileInfo;
            }
        }

        public FileInfo FileInfo { get; private set; }

        public static bool IsSupported(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (!File.Exists(path))
                throw new NotFoundException(new FileNotFoundException("File NotFound", path));
            Contract.EndContractBlock();

            return SupportedTypes.Contains(IOPath.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }

        public string NameWithoutExtension { get; private set; }
    }
}