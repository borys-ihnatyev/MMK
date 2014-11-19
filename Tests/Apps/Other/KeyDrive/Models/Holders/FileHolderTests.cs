using System;
using NUnit.Framework;

namespace MMK.KeyDrive.Models.Holders
{
    [TestFixture]
    public class FileHolderTests : HolderUnitTest
    {
        FileHolder fileVm;

        protected override void OnBeforeTest()
        {
            fileVm = Holder.Build(TempFile.FullName) as FileHolder;
        }

        [Test]
        public void Buid_WhenPathIsFile_WithSupportedExtension()
        {
            Assert.NotNull(fileVm);
        }

        [Test]
        [ExpectedException(typeof(Holder.NotSupportedException))]
        public void Buid_WhenPathIsFile_WithNotSupportedExtension()
        {
            Holder.Build(TempUnsupportedFile.FullName);
        }

        [Test]
        public void Serialization_WhenFileNotMoved()
        {
            var serializatioFileName = Guid.NewGuid() + ".bin";
            var expected = fileVm;
            
            Serialize(expected, serializatioFileName);
            var actual = Deserialize<FileHolder>(serializatioFileName);

            Assert.AreEqual(expected, actual);
        }
        

        [Test]
        [ExpectedException(typeof(Holder.NotFoundException))]
        public void Serialization_WhenFileGone()
        {
            var serializatioFileName = Guid.NewGuid() + ".bin";
            var expected = fileVm;

            Serialize(expected, serializatioFileName);
            
            TempFile.Delete();
            Deserialize<FileHolder>(serializatioFileName);
        }   
    }
}
