using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace MMK.KeyDrive
{
    public abstract class UnitTest
    {
        public const string TempRootDirName = "MMK.KeyDriveWatcher.TestData";
        public const string SupportedExtension = ".mp3";
        public const string UnsupportedExtension = ".mp2";

        private DirectoryInfo tempRootDir;

        protected FileInfo TempFile { get; private set; }
        protected FileInfo TempUnsupportedFile { get; private set; }
        protected DirectoryInfo TempDir { get; private set; }

        [TestFixtureSetUp]
        public void Initialize()
        {
            tempRootDir = Directory.Exists(TempRootDirName)
                ? new DirectoryInfo(TempRootDirName)
                : Directory.CreateDirectory(TempRootDirName);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            DeleteTempFsItem(tempRootDir);
        }

        [SetUp]
        public void BeforeTest()
        {
            TempFile = CreateTempFile(SupportedExtension);
            TempUnsupportedFile = CreateTempFile(UnsupportedExtension);
            TempDir = CreateTempDir();

            OnBeforeTest();
        }

        protected virtual void OnBeforeTest()
        {
        }

        protected FileInfo CreateTempFile(string extension)
        {
            var path = Path.Combine(tempRootDir.FullName,Guid.NewGuid() + extension);
            File.Create(path).Close();
            return new FileInfo(path);
        }

        protected DirectoryInfo CreateTempDir()
        {
            return tempRootDir.CreateSubdirectory(Guid.NewGuid().ToString());
        }

        protected static IList<string> FillDir(DirectoryInfo dir, int fileCount, Func<int, string> extension)
        {
            var files = new List<string>(fileCount);
            while (--fileCount >= 0)
            {
                files.Add(Path.Combine(dir.FullName, Guid.NewGuid() + extension(fileCount)));
                File.Create(files[files.Count - 1]).Close();
            }
            return files;
        }

        [TearDown]
        public void AfterTest()
        {
            OnAfterTest();
        }

        protected virtual void OnAfterTest()
        {
        }

        private static void DeleteTempFsItem(FileSystemInfo file)
        {
            if (!file.Exists) return;
            try
            {
                var dir = file as DirectoryInfo;
                if (dir != null)
                    dir.Delete(true);
                else
                    file.Delete();
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }
        }
    }
}