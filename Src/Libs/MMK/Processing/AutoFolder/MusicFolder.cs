using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using IOPath = System.IO.Path;

namespace MMK.Processing.AutoFolder
{
    public class MusicFolder
    {
        public readonly TimeSpan InnerFolderCreatePeriod = TimeSpan.FromDays(14);
        private readonly string path;

        public MusicFolder(string path, bool createNew = false)
        {
            path = PathExtension.Normalize(path);
            if (!Directory.Exists(path))
                if (!createNew)
                    throw new DirectoryNotFoundException();
                else
                    Directory.CreateDirectory(path);

            this.path = path;
        }

        public string Path
        {
            get { return path; }
        }

        public string LastYearPath
        {
            get { return IOPath.Combine(path, DateTime.Now.ToString("yyyy")); }
        }

        public int LastInnerFolderNameNumber { get; private set; }

        public string LastInnerFolderPath { get; private set; }


        public ResultInfo MoveFile(string filePath)
        {
            Initialize();

            filePath = PathExtension.Normalize(filePath);
            var fileInfo = new FileInfo(filePath);

            Contract.Assume(fileInfo.Directory != null);
            Contract.Assume(fileInfo.Directory.Parent != null);

            var newFilePath = IOPath.Combine(LastInnerFolderPath, fileInfo.Name);
            fileInfo.MoveTo(newFilePath);

            return BuildResultInfo(newFilePath);
        }

        private void Initialize()
        {
            CreateLastYearFolderIfNotExists();
            CreateLastInnerFolderIfNotExists();
        }

        private void CreateLastYearFolderIfNotExists()
        {
            if (!Directory.Exists(LastYearPath))
                Directory.CreateDirectory(LastYearPath);
        }

        private void CreateLastInnerFolderIfNotExists()
        {
            LastInnerFolderNameNumber = 0;
            var lastInnerFolderInfo = new DirectoryInfo(IOPath.Combine(LastYearPath, "00"));

            foreach (
                var info in
                    Directory.EnumerateDirectories(LastYearPath).Select(directory => new DirectoryInfo(directory)))
            {
                int innerFolderNameNumber;
                int.TryParse(info.Name, out innerFolderNameNumber);

                if (innerFolderNameNumber <= LastInnerFolderNameNumber) continue;

                LastInnerFolderNameNumber = innerFolderNameNumber;
                lastInnerFolderInfo = info;
            }

            var createOffset = DateTime.Now - lastInnerFolderInfo.CreationTime;

            if (createOffset > InnerFolderCreatePeriod)
                lastInnerFolderInfo =
                    new DirectoryInfo(IOPath.Combine(LastYearPath, (++LastInnerFolderNameNumber).ToString("D2")));

            if (!lastInnerFolderInfo.Exists)
                lastInnerFolderInfo.Create();

            LastInnerFolderPath = lastInnerFolderInfo.FullName;
        }


        public bool HasFile(string filePath)
        {
            return HasFile(new FileInfo(PathExtension.Normalize(filePath)));
        }

        public bool HasFile(FileInfo fileInfo)
        {
            fileInfo.Refresh();

            if (!fileInfo.Exists)
                return false;

            if (fileInfo.Directory == null)
                return false;

            if (fileInfo.Directory.Parent == null)
                return false;

            return LastYearPath.Equals(fileInfo.Directory.Parent.FullName, StringComparison.OrdinalIgnoreCase);
        }


        public ResultInfo MoveFile(string filePath, MusicFolder currentFolder)
        {
            Contract.Requires(currentFolder != null);
            Contract.EndContractBlock();

            if (currentFolder == this)
                return BuildResultInfo(filePath);

            var fileInfo = new FileInfo(filePath);
            var fileInnerFolderNameNumber = GetFileInnerFolderNameNumber(fileInfo);

            if (fileInnerFolderNameNumber > LastInnerFolderNameNumber)
                return MoveFile(filePath);

            var newFilePath = IOPath.Combine(Path, fileInnerFolderNameNumber.ToString("D2"), fileInfo.Name);
            fileInfo.MoveTo(newFilePath);

            return BuildResultInfo(newFilePath); 
        }

        private int GetFileInnerFolderNameNumber(FileInfo fileInfo)
        {
            var lastInnerFolderNameNumber = LastInnerFolderNameNumber;

            if (fileInfo.Directory != null)
                Int32.TryParse(fileInfo.Directory.Name, out lastInnerFolderNameNumber);

            return lastInnerFolderNameNumber;
        }


        private ResultInfo BuildResultInfo(string newFilePath)
        {
            return new ResultInfo
            {
                NewFilePath = newFilePath,
                MusicFolderInnerPath = LastInnerFolderPath,
                MusicFolderRootPath = Path
            };
        }

        public class Comparer : IComparer<MusicFolder>
        {
            public int Compare(MusicFolder x, MusicFolder y)
            {
                if (x == null && y == null)
                    return 0;
                if (y == null)
                    return 1;
                if (x == null)
                    return -1;

                return string.Compare(x.Path, y.Path, StringComparison.OrdinalIgnoreCase);
            }
        }

        public class ResultInfo
        {
            internal ResultInfo()
            {
            }

            public string NewFilePath { get; internal set; }
            public string MusicFolderRootPath { get; internal set; }
            public string MusicFolderInnerPath { get; internal set; }
        }
    }
}