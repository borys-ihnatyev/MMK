using System;
using System.Diagnostics.Contracts;
using System.IO;
using IOPath = System.IO.Path;

namespace MMK.Processing.AutoFolder
{
    public partial class MusicFolderLocation
    {
        public MusicFolderLocation()
        {
            
        }

        public MusicFolderLocation(string rootPath)
        {
            RootPath = rootPath;
        }

        public static MusicFolderLocation Parse(string path)
        {
            var parser = new Parser(path);
            return parser.Parse();
        }

        public static MusicFolderLocation TryParse(string path)
        {
            try
            {
                return Parse(path);
            }
            catch (Parser.ParseException)
            {
                return null;
            }
        }

        public int Year { get; set; }

        public string YearPath
        {
            get { return IOPath.Combine(RootPath, Year.ToString("D4")); }
        }

        public int InnerFolder { get; set; }

        public string RootPath { get; set; }

        public string Path
        {
            get { return IOPath.Combine(RootPath, Year.ToString("D4"), InnerFolder.ToString("D2")); }
        }

        public string Move(string filePath)
        {
            CreateIfNotExists();

            filePath = PathExtension.Normalize(filePath);
            var fileInfo = new FileInfo(filePath);

            Contract.Assume(fileInfo.Directory != null);
            Contract.Assume(fileInfo.Directory.Parent != null);

            var newFilePath = IOPath.Combine(Path, fileInfo.Name);
            fileInfo.MoveTo(newFilePath);

            return newFilePath;
        }

        private void CreateIfNotExists()
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
        }

        public static MusicFolderLocation operator ++(MusicFolderLocation folderLocation)
        {
            if (folderLocation.Year < DateTime.Now.Year)
            {
                folderLocation.Year = DateTime.Now.Year;
                folderLocation.InnerFolder = 1;
            }
            else
            {
                ++folderLocation.InnerFolder;
            }

            return folderLocation;
        }
    }
}