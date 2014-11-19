using System;
using NUnit.Framework;

namespace MMK.KeyDrive.Models.Holders
{
    [TestFixture]
    public class HolderTests : HolderUnitTest
    {
        [Test]
        [ExpectedException(typeof(Holder.NotFoundException))]
        public void MustThrowException_OnFileNotFound()
        {
            Holder.Build(Guid.NewGuid().ToString());
        }

        [Test]
        public void AreEqualPaths_BetweenFullAndShortPath()
        {
            var holder1 = Holder.Build(TempRootDirName +"\\" +TempFile.Name);
            var holder2 = Holder.Build(TempFile.FullName);
            Assert.AreEqual(holder1, holder2);

            holder1 = Holder.Build(TempRootDirName +"\\"+TempDir.Name);
            holder2 = Holder.Build(TempDir.FullName);
            Assert.AreEqual(holder1, holder2);
        }

        [Test]
        public void CaseIgnorePath()
        {
            var holder1 = Holder.Build(TempRootDirName + "\\" + TempFile.Name.ToUpper());
            var holder2 = Holder.Build(TempFile.FullName);
            Assert.AreEqual(holder1, holder2);

            holder1 = Holder.Build(TempRootDirName + "\\" + TempDir.Name);
            holder2 = Holder.Build(TempDir.FullName.ToUpper());
            Assert.AreEqual(holder1, holder2);
        }
    }
}
