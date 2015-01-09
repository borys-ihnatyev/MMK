using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MMK.Marking.Representation;
using MMK.Processing.AutoFolder;
using NUnit.Framework;

namespace MMK.Tests.Processing.AutoFolder
{
    [TestFixture]
    public class HashTagFolderCollectionTest
    {
        // ReSharper disable ObjectCreationAsStatement

        [Test]
        [ExpectedException(typeof (HashTagFolderCollection.PatternAlreadyExistsException))]
        public void ThrowsException_OnAddFolderWithSinglePatternThatExists()
        {
            new HashTagFolderCollection
            {
                {@"H:\music\pop\", "#pop"},
                {@"H:\music\deep\", "#pop"},
            };
        }


        [Test]
        [ExpectedException(typeof (HashTagFolderCollection.PatternAlreadyExistsException))]
        public void ThrowsException_OnAddFolderWithMultiplePatterns_WhichSomeExists()
        {
            new HashTagFolderCollection
            {
                {@"H:\music\house\", "#pop #house"},
                {@"H:\music\house\", "#house"},
                {@"H:\music\pop\", "#pop #house"},
                {@"H:\music\pop\", "#pop"}
            };
        }

        [Test]
        [ExpectedException(typeof (HashTagFolderCollection.PatternAlreadyExistsException))]
        public void ThrowsException_OnAddFolderWithSinglePatternThatExists_Advenced()
        {
            new HashTagFolderCollection
            {
                {@"H:\music\pop\", "#pop #house"},
                {@"H:\music\deep\", "#house #pop"}
            };
        }

        // ReSharper restore ObjectCreationAsStatement

        [Test]
        public void NotThrowsException_OnAddFolderWithSinglePatternThatHasSameHashTag()
        {
            var collection = new HashTagFolderCollection
            {
                {@"H:\music\pop\", "#pop #house"}
            };
            collection.Add(@"H:\music\deep\", "#pop");
        }

        [Test]
        public void GetMatchPath_WhenExists_1()
        {
            var popFolderPath = PathExtension.Normalize(@"H:\music\pop\");
            var houseFolderPath = PathExtension.Normalize(@"H:\music\house\");
            var mainFolderPath = PathExtension.Normalize(@"H:\music\deep\");
            var specFolderPath = PathExtension.Normalize(@"H:\music\spec\");

            var collection = new HashTagFolderCollection
            {
                {popFolderPath, "#pop"},
                {houseFolderPath, "#house"},
                {houseFolderPath, "#house #pop"},
                {mainFolderPath, "#deep"},
                {mainFolderPath, "#deep #house"},
                {mainFolderPath, "#deep #house #pop"},
                {mainFolderPath, "#newdisco"},
                {specFolderPath, "#spec"},
                {specFolderPath, "#trap"},
                {specFolderPath, "#rnb"},
                {specFolderPath, "#pop #spec"},
                {specFolderPath, "#house #spec"},
                {specFolderPath, "#house #spec #pop"},
            };

            Assert.AreEqual(houseFolderPath, collection.GetMatchPath(HashTagModel.Parser.All("#pop #house #entrophy")));
            Assert.AreEqual(popFolderPath, collection.GetMatchPath(HashTagModel.Parser.All("#entrophy #pop")));
            Assert.AreEqual(houseFolderPath, collection.GetMatchPath(HashTagModel.Parser.All("#house #entrophy")));
            Assert.AreEqual(specFolderPath, collection.GetMatchPath(HashTagModel.Parser.All("#spec #entrophy")));
            Assert.AreEqual(specFolderPath,
                collection.GetMatchPath(HashTagModel.Parser.All("#spec #pop #house #entrophy")));
            Assert.AreEqual(specFolderPath,
                collection.GetMatchPath(HashTagModel.Parser.All("#spec #pop #house #deep #entrophy")));
            Assert.AreEqual(mainFolderPath,
                collection.GetMatchPath(HashTagModel.Parser.All("#pop #house #deep #entrophy")));
        }

        [Test]
        public void MustMoveToMorePrioritySubsetofModel()
        {
            const string popPath = @"H:\music\pop";
            const string housePath = @"H:\music\house";
            const string mainPath = @"H:\music\deep";

            var collection = new HashTagFolderCollection
            {
                {housePath, "#house #deep"},
                {popPath, "#pop"},
                {housePath, "#house", 1},
                {mainPath, "#deep", 2},
            };

            const string expectedPath = mainPath;
            var actualPath = collection.GetMatchPath(HashTagModel.Parser.All("#pop #house #deep"));

            Assert.AreEqual(expectedPath, actualPath);
        }

        [Test]
        public void Serialization()
        {
            const string collectionFileName = "Test_FolderCollection.bin";
            var expectedCollection = MakeTestCollection();

            var serializer = new BinaryFormatter();
            using (var stream = File.Create(collectionFileName))
            {
                serializer.Serialize(stream, expectedCollection);
            }

            using (var stream = File.OpenRead(collectionFileName))
            {
                var actualCollection = (HashTagFolderCollection) serializer.Deserialize(stream);

                var expectedEnumer = expectedCollection.GetEnumerator();
                var actualEnumer = actualCollection.GetEnumerator();

                while (expectedEnumer.MoveNext())
                {
                    actualEnumer.MoveNext();
                    Assert.AreEqual(expectedEnumer.Current.Key.Model, actualEnumer.Current.Key.Model);
                    Assert.AreEqual(expectedEnumer.Current.Key.Priority, actualEnumer.Current.Key.Priority);
                    Assert.AreEqual(expectedEnumer.Current.Value, actualEnumer.Current.Value);
                }
            }
        }

        private static HashTagFolderCollection MakeTestCollection()
        {
            var popFolderPath = PathExtension.Normalize(@"H:\music\pop\");
            var houseFolderPath = PathExtension.Normalize(@"H:\music\house\");
            var mainFolderPath = PathExtension.Normalize(@"H:\music\deep\");
            var specFolderPath = PathExtension.Normalize(@"H:\music\spec\");
            var mixesFolderPath = PathExtension.Normalize(@"H:\music\mixes\");

            return new HashTagFolderCollection
            {
                {popFolderPath, "#pop"},
                {popFolderPath, "#rus"},
                {popFolderPath, "#pop #rus"},
                {popFolderPath, "#ukr"},
                {popFolderPath, "#pop #ukr"},
                {houseFolderPath, "#house"},
                {houseFolderPath, "#house #pop"},
                {mainFolderPath, "#deep"},
                {mainFolderPath, "#deep #house"},
                {mainFolderPath, "#deep #house #pop"},
                {mainFolderPath, "#newdisco"},
                {specFolderPath, "#spec"},
                {specFolderPath, "#trap"},
                {specFolderPath, "#rnb"},
                {specFolderPath, "#pop #spec"},
                {specFolderPath, "#house #spec"},
                {specFolderPath, "#house #spec #pop"},
                {mixesFolderPath, "#mix"},
                {mixesFolderPath, "#mix #pop"},
                {mixesFolderPath, "#mix #deep"},
                {mixesFolderPath, "#mix #house"},
                {mixesFolderPath, "#mix #deep #house"},
                {mixesFolderPath, "#mix #deep #house #pop"},
                {mixesFolderPath, "#mix #newdisco"},
            };
        }
    }
}