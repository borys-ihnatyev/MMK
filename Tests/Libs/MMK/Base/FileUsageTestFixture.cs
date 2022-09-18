using System;
using System.IO;
using NUnit.Framework;

namespace MMK.Base
{
    public abstract class FileUsageTestFixture
    {
        private DirectoryInfo filesTempDir;

        [SetUp]
        public virtual void SetUp()
        {
            filesTempDir = new DirectoryInfo(Guid.NewGuid().ToString());
            filesTempDir.Create();
            filesTempDir.Refresh();
            Environment.CurrentDirectory = filesTempDir.FullName;
        }

        [TearDown]
        public virtual void TearDown()
        {
            Environment.CurrentDirectory = filesTempDir.Parent.FullName;
            filesTempDir.ForceDelete();
        }

        protected string CreateFile()
        {
            var fileName = Guid.NewGuid().ToString();
            File.Create(fileName).Close();
            return fileName;
        }
    }
}
