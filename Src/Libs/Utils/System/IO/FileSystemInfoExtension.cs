namespace System.IO
{
    public static class FileSystemInfoExtension
    {
        public static void ForceDelete(this FileSystemInfo info)
        {
            var dirInfo = info as DirectoryInfo;
            if(dirInfo != null)
                dirInfo.Delete(true);
            else
                info.Delete();
        }
    }
}
