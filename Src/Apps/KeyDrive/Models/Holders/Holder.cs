using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using MMK.Presentation.ViewModel;

namespace MMK.KeyDrive.Models.Holders
{
    [Serializable]
    public abstract partial class Holder : ViewModel, ISerializable
    {
        protected Holder(string path)
        {
            Contract.Requires(path != null);
        }

        public abstract FileSystemInfo Info { get; }

        public static Holder Build(string path)
        {
            if (File.Exists(path))
                return new FileHolder(path);
            if (Directory.Exists(path))
                return new DirectoryHolder(path);
            throw new NotFoundException();
        }

        public override int GetHashCode()
        {
            return Info.FullName.ToLower().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var holder = obj as Holder;
            return Equals(holder);
        }

        public bool Equals(Holder holder)
        {
            if (holder == null) return false;
            return Info.FullName.Equals(holder.Info.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return Info.FullName;
        }

        #region Serialization

        protected const string PathName = "FullName";

        protected Holder(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            if (info.GetString(PathName) == null)
                throw new SerializationException("FullName is required");
            Contract.EndContractBlock();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(PathName, Info.FullName);
        }
        #endregion
    }
}