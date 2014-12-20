using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MMK.MagicPlaylist.Base;
using NUnit.Framework;

namespace MMK.MagicPlaylist.ViewModels
{
    [TestFixture]
    public class DirectoryCollectionTest : FileUsageTestFixture
    {
        private DirectoryCollection collection;
        
        [SetUp]
        public override void SetUp()
        {
            collection = new DirectoryCollection();
            base.SetUp();
        }

        [Test]
        public void Add_ExistedDirectory()
        {
            var dir = CreateDir();
            collection.Add(dir);
            Assert.IsNotEmpty(collection);
        }

        [Test]
        public void Add_ExistedSameDirectory()
        {
            var dir = CreateDir();
            collection.Add(dir);
            collection.Add(dir);
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void Add_IgnoreDirectoryWhenNotFound()
        {
            collection.Add(Guid.NewGuid().ToString());
            Assert.IsEmpty(collection);
        }

        [Test]
        public void Add_IgnoreInnerDirectories()
        {
            var dir = new DirectoryInfo(CreateDir());
            var subDir = dir.CreateSubdirectory(Guid.NewGuid().ToString());

            collection.Add(dir.FullName);
            collection.Add(subDir.FullName);

            const int expectedDirsCount = 1;
            Assert.AreEqual(expectedDirsCount, collection.Count);
        }

        [Test]
        public void AddParent_IgnoreInnerDirectories()
        {
            var dir = new DirectoryInfo(CreateDir());
            var subDir = dir.CreateSubdirectory(Guid.NewGuid().ToString());

            collection.Add(subDir.FullName);
            collection.Add(dir.FullName);

            const int expectedDirsCount = 1;
            Assert.AreEqual(expectedDirsCount, collection.Count);
        }

        [Test]
        [TestCase(10, 1)]
        [TestCase(12, 12)]
        [TestCase(10, 6)]
        [TestCase(6, 5)]
        public void SourceDirs_OnSanitize_RemoveDeletedDirs(int generateDirs, int deleteDirs)
        {
            var dirs = Enumerable.Range(0, generateDirs).Select(i => CreateDir()).ToList();
            dirs.ForEach(collection.Add);
            Enumerable.Range(0, deleteDirs).ForEach(i => Directory.Delete(dirs[i]));

            collection.Sanitize();

            var expectedDirsCount = generateDirs - deleteDirs;
            Assert.AreEqual(expectedDirsCount, collection.Count);
        }
    }
}