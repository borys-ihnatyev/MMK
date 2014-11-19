using System.Threading;

namespace System.IO
{
    public static class FileInfoExtension
    {
        public static bool IsFileLocked(this FileInfo fileInfo)
        {
            FileStream stream = null;

            try
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        public static bool Wait(this FileInfo fileInfo, TimeSpan wait)
        {
            var period = TimeSpan.FromSeconds(2);

            if (period > wait)
                period = wait;

            while (fileInfo.IsFileLocked() || wait.TotalMilliseconds <= 0)
            {
                Thread.Sleep(period);
                wait -= period;
            }

            return fileInfo.IsFileLocked();
        }

        public static void Wait(this FileInfo fileInfo)
        {
            var period = TimeSpan.FromSeconds(2);
            while (fileInfo.IsFileLocked())
            {
                Thread.Sleep(period);
            }
        }

        public static FileInfo TryCopyTo(this FileInfo file, DirectoryInfo dir)
        {
            try
            {
                file.Wait(TimeSpan.FromSeconds(10));
                return CopyTo(file, dir);
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static FileInfo CopyTo(this FileInfo file, DirectoryInfo dir)
        {
            var newPath = Path.Combine(dir.FullName, file.Name);
            return file.CopyTo(newPath);
        }

        public static bool TryMoveTo(this FileInfo file, DirectoryInfo dir)
        {
            try
            {
                file.Wait(TimeSpan.FromSeconds(10));
                MoveTo(file, dir);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public static void MoveTo(this FileInfo file, DirectoryInfo dir)
        {
            File.Move(file.FullName,Path.Combine(dir.FullName, file.Name));
        }

        public static void MoveTo(this FileInfo oldFile, FileInfo newFile)
        {
            File.Move(oldFile.FullName,newFile.FullName);
        }

        public static bool TryDelete(this FileInfo file)
        {
            try
            {
                file.Wait(TimeSpan.FromSeconds(7));
                file.Delete();
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}