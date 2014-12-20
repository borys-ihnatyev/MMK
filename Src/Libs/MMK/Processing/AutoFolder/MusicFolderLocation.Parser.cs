using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;

namespace MMK.Processing.AutoFolder
{
    public partial class MusicFolderLocation
    {
        public class Parser
        {
            private readonly FileInfo file;
            private readonly MusicFolderLocation folderLocation;
            private DirectoryInfo innerFolder;
            private DirectoryInfo yearFolder;

            public Parser(string path)
            {
                file = new FileInfo(path);
                if (!file.Exists)
                    throw new FileNotFoundException("not found", path);

                folderLocation = new MusicFolderLocation();
            }

            public MusicFolderLocation Parse()
            {
                folderLocation.InnerFolder = ExtractInnerFolderNumber();
				folderLocation.Year = ExtractYearNumber();
				folderLocation.RootPath = ExtractRootPath();
                return folderLocation;
            }

            private int ExtractInnerFolderNumber()
            {
                innerFolder = file.Directory; 

                if (innerFolder == null)
                    throw new ParseException("file has no parent directory");
                int innerFolderNumber;

                if (!Int32.TryParse(innerFolder.Name, out innerFolderNumber))
                    throw new ParseException("inner directory name was not numeric format");

                return innerFolderNumber;
            }

            private int ExtractYearNumber()
            {
                Contract.Requires(innerFolder != null);
				Contract.EndContractBlock();

                yearFolder = innerFolder.Parent;
                if(yearFolder == null)
                    throw new ParseException("inner folder has no year directory");
                
                int yearNumber;
				if(!Int32.TryParse(yearFolder.Name, out yearNumber))
                    throw new ParseException("year directory name was not numeric format");

                return yearNumber;
            }

            private string ExtractRootPath()
            {
                Contract.Requires(yearFolder != null);
				Contract.EndContractBlock();
                var rootFolder = yearFolder.Parent;
				if(rootFolder == null)
                    throw new ParseException("year folder has no root directory");

                return rootFolder.FullName;
            }

            [Serializable]
            public class ParseException : Exception
            {
                public ParseException(string message) : base(message)
                {
                }

                public ParseException()
                {
                }

                protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
                {
                }
            }
        }
    }
}