using System.Collections.Generic;
using NUnit.Framework;

namespace MMK.Processing
{
    [TestFixture]
    public class KeyCoverArtCollectionTest
    {
        private KeyCoverArtFactory keyCoverArtFactory;

        [TestFixtureSetUp]
        public void SetUp()
        {
            keyCoverArtFactory = new KeyCoverArtFactory("__KeyColorsTest");
        }

        [Test, TestCaseSource("GetAllKeys")]
        public void TestCreateImage(Key key)
        {
            keyCoverArtFactory.RetriveImagePath(key);
        }

        public IEnumerable<Key> GetAllKeys()
        {
            return CircleOfFifths.AllKeys;
        }
    }
}