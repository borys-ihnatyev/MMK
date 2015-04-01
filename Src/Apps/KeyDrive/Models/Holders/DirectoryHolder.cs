using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace MMK.KeyDrive.Models.Holders
{
    [Serializable]
    public class DirectoryHolder : Holder
    {
        private bool includeSubdirectories;

        internal DirectoryHolder(string path) : base(path)
        {
            if(!Directory.Exists(path))
                throw new NotFoundException(new DirectoryNotFoundException(path));

            DirInfo = new DirectoryInfo(path);
            includeSubdirectories = true;
        }

        public override FileSystemInfo Info
        {
            get
            {
                return DirInfo;
            }
        }

        public DirectoryInfo DirInfo { get; private set; }

        public bool IncludeSubdirectories
        {
            get { return includeSubdirectories; }
            set
            {
                if(includeSubdirectories && value) return;
                includeSubdirectories = value;
                NotifyPropertyChanged();
            }
        }

        public IList<FileHolder> Files
        {
            get
            {
                return FilePaths
                    .Where(FileHolder.IsSupported)
                    .Select(p => new FileHolder(p))
                    .ToList()
                    .AsReadOnly();
            }
        }

        private IEnumerable<string> FilePaths
        {
            get
            {
                return Directory.EnumerateFiles(
                    Info.FullName, 
                    "*",
                    includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                );
            }
        }

        public bool Contains(string path)
        {
            if(path == null)
                return false;
            if (!File.Exists(path) && !Directory.Exists(path))
                return false;
            
            path = PathExtension.Normalize(path);
            return path.StartsWith(Info.FullName,StringComparison.OrdinalIgnoreCase);
        }

        public bool Contains(FileSystemInfo info)
        {
            if (info == null)
                return false;

            info.Refresh();
            
            if (!info.Exists)
                return false;

            return info.FullName.StartsWith(Info.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public bool Contains(Holder holder)
        {
            return holder.Info.FullName.StartsWith(Info.FullName, StringComparison.OrdinalIgnoreCase);
        }

        #region Serialization

        private const string IncludeSubdirectoriesName = "IncludeSubdirectories";

        protected DirectoryHolder(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (!Directory.Exists(info.GetString(PathName)))
                throw new NotFoundException(new DirectoryNotFoundException("While deserialize " + info.GetString(PathName)));
            Contract.EndContractBlock();

            DirInfo = new DirectoryInfo(info.GetString(PathName));
            includeSubdirectories = info.GetBoolean(IncludeSubdirectoriesName);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(IncludeSubdirectoriesName, includeSubdirectories);
        }

        #endregion
    }
}