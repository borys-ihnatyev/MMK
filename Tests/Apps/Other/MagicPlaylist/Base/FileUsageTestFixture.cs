using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MMK.MagicPlaylist.Base
{
    public abstract class FileUsageTestFixture
    {
        private DirectoryInfo filesTempDir;

        protected string CreateFile(string extension)
        {
            var fileName = Guid.NewGuid() + extension;
            var filePath = Path.Combine(filesTempDir.FullName, fileName);
            File.Create(filePath).Close();
            return filePath;
        }

        protected string CreateDir()
        {
            var dirName = Guid.NewGuid().ToString();
            var dirPath = Path.Combine(filesTempDir.FullName, dirName);
            return Directory.CreateDirectory(dirPath).FullName;
        }

        [SetUp]
        public virtual void SetUp()
        {
            filesTempDir = new DirectoryInfo(Guid.NewGuid().ToString());
            filesTempDir.Create();
            filesTempDir.Refresh();
        }

        [TearDown]
        public virtual void TearDown()
        {
            filesTempDir.ForceDelete();
        }
    }
}
