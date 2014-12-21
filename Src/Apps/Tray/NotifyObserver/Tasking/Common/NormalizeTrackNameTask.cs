using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using MMK.Notify.Observer.Tasking.Common.Base;

namespace MMK.Notify.Observer.Tasking.Common
{
    [Serializable]
    public class NormalizeTrackNameTask : Mp3FileChangeTask
    {
        public static IEnumerable<Task> Many(IEnumerable<string> paths)
        {
            return paths.Select(p => new NormalizeTrackNameTask(p));
        }  

        public NormalizeTrackNameTask(string oldPath)
            : base(oldPath)
        {
        }

        protected override void Initialize()
        {
            BaseInitializeSkipCancel();
       
            var directory = OldFile.DirectoryName;
            Contract.Assert(directory != null);
            var newFilePath = Path.Combine(directory, NameModel + OldFile.Extension);
            NewFile = new FileInfo(newFilePath);
        }

        private void BaseInitializeSkipCancel()
        {
            try
            {
                base.Initialize();
            }
            catch (Cancel)
            {
            }
        }

        protected override string CommonDescription
        {
            get { return "Track name normalization"; }
        }

        protected override string DetailedDescription
        {
            get
            {
                return string.Format(
                    "From : \"{0}\"\nTo : \"{1}\"", 
                    CleanName,
                    NameModel.FullName
                );
            }
        }
    }
}