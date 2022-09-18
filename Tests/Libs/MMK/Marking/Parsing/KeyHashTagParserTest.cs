using NUnit.Framework;

namespace MMK.Marking.Parsing
{
    public class KeyHashTagParserTest
    {
        [Test]
        public void ParseKeyHashTagWithMeta()
        {
            const string meta = "test-meta";
            var key = new Key(Note.A, Tone.Moll);
            var keyHashTagWithMeta = HashTag.Hash + key + HashTag.Meta + meta;
            var hashTagEntry = KeyHashTag.Parser.First(keyHashTagWithMeta);
            Assert.IsNotNull(hashTagEntry);
            Assert.IsNotNull(hashTagEntry.HashTag);
            Assert.AreEqual(hashTagEntry.HashTag.Key, key);
            Assert.AreEqual(hashTagEntry.HashTag.TagMetaValue, meta);
        }
    }
}