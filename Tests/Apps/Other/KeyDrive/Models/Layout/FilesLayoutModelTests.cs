using System.Collections.Generic;
using System.IO;
using MMK.KeyDrive.Models.Holders;
using MMK.Marking;
using MMK.Marking.Representation;
using NUnit.Framework;

namespace MMK.KeyDrive.Models.Layout
{
    [TestFixture]
    public class FilesLayoutModelTests : UnitTest
    {
        private DirectoryInfo layotRootDir;
        private FilesLayoutModel layoutModel;
        protected override void OnBeforeTest()
        {
            layotRootDir = CreateTempDir();
            layoutModel = new FilesLayoutModel(layotRootDir.FullName);
        }

        // ReSharper disable ObjectCreationAsStatement
        [Test]
        public void Create_WhenDirectoryExists()
        {

        }
        
        [Test]
        [ExpectedException(typeof(Holder.NotFoundException))]
        public void Create_WhenDirectoryNotExists()
        {
            layotRootDir.Delete(true);
            new FilesLayoutModel(layotRootDir.FullName);
        }
        // ReSharper restore ObjectCreationAsStatement

        [Test]
        [TestCaseSource("BuildHashTagModelWithParalelKeys")]
        public void All_DirectoriesAreUnique_WhenRelativeKeysInHashTagModel(HashTagModel hashTagModel)
        {
            Assert.AreEqual(1, layoutModel[hashTagModel].Length);
        }

        private static IEnumerable<HashTagModel> BuildHashTagModelWithParalelKeys()
        {
            foreach (var key in CircleOfFifths.MinorKeys)
            {
                var hashTagModel = new HashTagModel();
                var paralel = CircleOfFifths.GetParalel(key);
                hashTagModel.Add(new KeyHashTag(key));
                hashTagModel.Add(new KeyHashTag(paralel));
                yield return hashTagModel;
            } 
        }

        [Test]
        public void Same_KeyFolder_ForParalelKeys()
        {
            foreach (var minorKey in CircleOfFifths.MinorKeys)
            {
                var majKey = CircleOfFifths.GetParalel(minorKey);

                var minKeyFolder = layoutModel[new HashTagModel {new KeyHashTag(minorKey)}];
                var majKeyFolder = layoutModel[new HashTagModel {new KeyHashTag(majKey)}];

                Assert.AreEqual(minKeyFolder,majKeyFolder);
            }
        }   

    }
}
