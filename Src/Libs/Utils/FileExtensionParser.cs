using System.IO;

namespace MMK.Utils
{
    public static class FileExtensionParser
    {
        private static bool HasExtension(string filePath, string extension)
        {
            var fileExtension = Path.GetExtension(filePath);
            return fileExtension != null && fileExtension.ToLower() == extension;
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
