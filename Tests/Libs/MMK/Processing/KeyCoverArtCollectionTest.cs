using System.Collections.Generic;
using NUnit.Framework;

namespace MMK.Processing
{
    [TestFixture]
    public class KeyCoverArtCollectionTest
    {
        private KeyCoverArtCollection keyCoverArtCollection;

        [TestFixtureSetUp]
        public void SetUp()
        {
            keyCoverArtCollection = new KeyCoverArtCollection("__KeyColorsTest");
        }

        [Test, TestCaseSource("GetAllKeys")]
        public void TestCreateImage(Key key)
        {
            keyCoverArtCollection.RetriveImagePath(key);
        }

        public IEnumerable<Key> GetAllKeys()
        {
            return CircleOfFifths.AllKeys;
        }
    }
}