using System.Linq;
using MMK.Marking.Representation;
using NUnit.Framework;

namespace MMK.Marking.Parsing
{
    public class HashTagModelParserTest
    {
        [Test]
        public void Parsing_WithRedundantSpaces()
        {
            const string hashTagModelString = " #amoll  #pop #dance  #gift";

            var hashTagModel = HashTagModel.Parser.All(hashTagModelString);
            Assert.AreEqual(4,hashTagModel.Count);
        }

        [Test]
        public void Parsing_WithMergedHashTags()
        {
            const string hashTagModelString = " #amoll  #pop#dance  #gift";
            var hashTagModel = HashTagModel.Parser.All(hashTagModelString);
            Assert.AreEqual(3, hashTagModel.Count);
        }

        [Test]
        public void Parsing_WithMetaHashTags()
        {
            const string hashTagModelString = " #amoll  #pop@dance  #gift";
            var hashTagModel = HashTagModel.Parser.All(hashTagModelString);
            Assert.AreEqual(3, hashTagModel.Count);
        }

        [Test]
        public void Parsing_WithMergedKeyHashTags()
        {
            const string hashTagModelString = " #amoll#pop #dance#gift";

            var hashTagModel = HashTagModel.Parser.All(hashTagModelString);
            Assert.AreEqual(2, hashTagModel.Count);
            Assert.AreEqual(0,hashTagModel.OfType<KeyHashTag>().Count());
        }

        [Test]
        public void Parsed_KeyHashTagsProperly()
        {
            const string hashTagModelString = " #amoll  #pop #dance  #gift";

            var hashTagModel = HashTagModel.Parser.All(hashTagModelString);
            var keyHashTags = hashTagModel.OfType<KeyHashTag>().ToList();

            Assert.AreEqual(1, keyHashTags.Count);
            Assert.AreEqual(new Key(Note.A, Tone.Moll), keyHashTags[0].Key);
        }
    }
}
