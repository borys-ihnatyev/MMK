using System;
using System.IO;

namespace MMK.Utils
{
    public static class FileExtensionParser
    {
        public static bool HasExtension(string filePath, string extension)
        {
            extension = extension.Trim(' ','*');
            var fileExtension = Path.GetExtension(filePath);
            return extension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsMp3(string filePath)
        {
            return HasExtension(filePath, ".mp3");
        }

        public static bool IsChromeDownload(string filePath)
        {
            return HasExtension(filePath, ".crdownload");
        }
    }
}
