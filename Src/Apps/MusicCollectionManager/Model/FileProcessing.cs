using System;
using System.IO;
using MMK.Processing;

namespace MMK.MusicCollectionManager.Model
{
    public class FileProcessing
    {
        public const string Mp3Extension = ".mp3";

        public static bool IsMp3File(string path)
        {
            var extension = Path.GetExtension(path);
            return extension != null && extension.ToLower() == Mp3Extension;
        }

        public static bool IsDirectory(string path)
        {
            var attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public static bool TryWriteId3Tags(string filePath)
        {
            try
            {
                new Id3Tager(filePath).Tag();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryDeleteId3Tags(string filePath)
        {
            try
            {
                new Id3Tager(filePath).RemoveTags();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
