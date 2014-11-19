using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace MMK.KeyDrive.Models.Holders
{
    [TestFixture]
    public class DirectoryHolderTests : HolderUnitTest
    {
        private DirectoryHolder directoryVm;

        protected override void OnBeforeTest()
        {
            directoryVm = Holder.Build(TempDir.FullName) as DirectoryHolder;
        }

        [Test]
        public void Buid_WhenPathIsDirectory()
        {
            Assert.NotNull(directoryVm);
        }

        [Test]
        public void MakeItemsCollection_TopDirOnly_WhenAllFilesHasSupportedType()
        {
            var expected = FillDir(TempDir, 10, i => SupportedExtension);
            var actual = directoryVm.Files;
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [Test]
        public void MakeItemsCollection_TopDirOnly_WhenHalfFilesHasSupportedType()
        {
            const int filesCount = 10;
            var expected = FillDir(TempDir, filesCount, PairSupportedExtension);
            var actual = directoryVm.Files;

            Assert.AreEqual(expected.Count/2, actual.Count);
        }

        [Test]
        public void MakeItemsCollection_AllDirs_WhenAllFilesHasSupportedType()
        {
            directoryVm.IncludeSubdirectories = true;
            var tempSubDir = TempDir.CreateSubdirectory(Guid.NewGuid().ToString());

            var expected = new List<string>();
            var tempDirFiles = FillDir(TempDir, 10, i => SupportedExtension);
            var tempSubDirFiles = FillDir(tempSubDir, 12, i => SupportedExtension);

            expected.AddRange(tempDirFiles);
            expected.AddRange(tempSubDirFiles);

            var actual = directoryVm.Files;
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [Test]
        public void MakeItemsCollection_AllDirs_WhenHalfFilesHasSupportedType()
        {
            directoryVm.IncludeSubdirectories = true;

            var tempSubDir = TempDir.CreateSubdirectory(Guid.NewGuid().ToString());

            const int filesCount = 10;

            var expected = new List<string>();
            var tempDirFiles = FillDir(TempDir, filesCount, PairSupportedExtension);
            var tempSubDirFiles = FillDir(tempSubDir, filesCount + 2, PairSupportedExtension);

            expected.AddRange(tempDirFiles);
            expected.AddRange(tempSubDirFiles);

            var actual = directoryVm.Files;

            Assert.AreEqual(expected.Count/2, actual.Count);
        }

        private static string PairSupportedExtension(int i)
        {
            return i%2 == 0 ? SupportedExtension : UnsupportedExtension;
        }

        [Test]
        public void Contains_WhenFileExistsAndIsChild()
        {
            var tempDirFiles = FillDir(TempDir, 1, i => SupportedExtension);
            Assert.IsTrue(directoryVm.Contains(tempDirFiles[0]));
            tempDirFiles.ForEach(path => Assert.IsTrue(directoryVm.Contains(path)));
        }

        [Test]
        public void NotContains_WhenFileExistsAndNotIsChild()
        {
            var tempDir = CreateTempDir();                
            var tempDirFiles = FillDir(tempDir, 10, i => SupportedExtension);
            tempDirFiles.ForEach(path => Assert.IsFalse(directoryVm.Contains(path)));   
        }

        [Test]
        public void Contains_WhenDirectoryExistsAndIsChild()
        {
            var tempSubDir = TempDir.CreateSubdirectory(Guid.NewGuid().ToString());
            Assert.IsTrue(directoryVm.Contains(tempSubDir.FullName));
        }

        [Test]
        public void NotContains_WhenDirectoryExistsAndNotIsChild()
        {
            var tempDir = CreateTempDir();
            Assert.IsFalse(directoryVm.Contains(tempDir.FullName)); 
        }

        [Test]
        public void Contains_WhenFileNotExists()
        {
            directoryVm.Contains("Dkshd)))==s==");
        }

        [Test]
        public void Serialization_WhenDirectoryNotMoved()
        {
            var serializatioFileName = Guid.NewGuid() + ".bin";
            var expected = directoryVm;

            Assert.NotNull(expected);

            Serialize(expected, serializatioFileName);
            var actualVm = Deserialize<DirectoryHolder>(serializatioFileName);

            Assert.AreEqual(expected, actualVm);
            Assert.AreEqual(expected.IncludeSubdirectories, actualVm.IncludeSubdirectories);
        }

        [Test]
        [ExpectedException(typeof (Holder.NotFoundException))]
        public void Serialization_WhenDirectoryGone()
        {
            var serializatioFileName = Guid.NewGuid() + ".bin";
            var expected = directoryVm;

            Assert.NotNull(expected);

            expected.IncludeSubdirectories = true;

            Serialize(expected, serializatioFileName);

            TempDir.Delete();

            Deserialize<DirectoryHolder>(serializatioFileName);
        }
    }
}