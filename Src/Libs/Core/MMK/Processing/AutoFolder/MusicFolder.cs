using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using MMK.Marking;
using IOPath = System.IO.Path;

namespace MMK.Processing.AutoFolder
{
    public class MusicFolder
    {
        private const string DeprecatedFolderName = "deprecated";
        public static readonly HashTag DeprecatedHashTag = new HashTag(DeprecatedFolderName);

        public readonly TimeSpan InnerFolderCreatePeriod = TimeSpan.FromDays(14);
        private readonly string path;

        private MusicFolder deprecatedFolder;

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

        public MusicFolder DeprecatedFolder
        {
            get
            {
                if (deprecatedFolder == null)
                {
                    var deprecatedFolderPath = CreateObsoleteFolderPath();
                    deprecatedFolder = new MusicFolder(deprecatedFolderPath, true);
                }
                return deprecatedFolder;
            }
        }

        private string CreateObsoleteFolderPath()
        {
            var pathInfo = new DirectoryInfo(path);
            Contract.Assert(pathInfo.Parent != null);
            return IOPath.Combine(pathInfo.Parent.FullName, DeprecatedFolderName, pathInfo.Name);
        }

        public ResultInfo MoveFile(string filePath, bool saveStructure = false)
        {
            var location = GetNewLocation(filePath, saveStructure);
            var newFilePath = location.Move(filePath);
            return new ResultInfo
            {
                MusicFolderInnerPath = location.Path,
                MusicFolderRootPath = Path,
                NewFilePath = newFilePath
            };
        }

        private MusicFolderLocation GetNewLocation(string filePath, bool saveStructure)
        {
            var fileLocation = MusicFolderLocation.TryParse(filePath);
            if (fileLocation == null)
                return CalcLastLocation();

            if (fileLocation.RootPath.Equals(Path, StringComparison.OrdinalIgnoreCase))
                return fileLocation;

            return GetSimilarLocation(fileLocation, saveStructure);
        }

        private MusicFolderLocation CalcLastLocation()
        {
            var lastLocation = GetLastLocation(DateTime.Now.Year);

            var lastLocationInfo = new DirectoryInfo(lastLocation.Path);
            if (lastLocationInfo.Exists && DateTime.Now >= lastLocationInfo.CreationTime + InnerFolderCreatePeriod)
                ++lastLocation;

            return lastLocation;
        }

        private MusicFolderLocation GetSimilarLocation(MusicFolderLocation fileLocation, bool saveStructure)
        {
            var newLocation = GetLastLocation(fileLocation.Year);

            if (saveStructure || newLocation.InnerFolder >= fileLocation.InnerFolder)
                newLocation.InnerFolder = fileLocation.InnerFolder;

            return newLocation;
        }

        private MusicFolderLocation GetLastLocation(int year)
        {
            var lastLocation = new MusicFolderLocation
            {
                RootPath = Path,
                Year = year
            };

            lastLocation.InnerFolder = GetLastInnerFolder(lastLocation);

            return lastLocation;
        }

        private static int GetLastInnerFolder(MusicFolderLocation location)
        {
            if (!Directory.Exists(location.YearPath))
                return 0;

            int lastInnerFolderNumber;

            var folderNumbers = Directory.GetDirectories(location.YearPath)
                .Select(IOPath.GetFileName)
                .Where(innerFolder => Int32.TryParse(innerFolder, out lastInnerFolderNumber))
                .Select(Int32.Parse)
                .OrderBy(n => n)
                .ToList();

            return folderNumbers.LastOrDefault();
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