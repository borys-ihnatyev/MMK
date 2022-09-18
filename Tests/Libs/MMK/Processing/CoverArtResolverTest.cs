using System.Collections.Generic;
using System.Linq;
using MMK.Marking;
using MMK.Marking.Representation;
using NUnit.Framework;

namespace MMK.Processing
{
    [TestFixture]
    public class CoverArtResolverTest
    {
        private CoverArtResovler resolver;

        [TestFixtureSetUp]
        public void SetUp()
        {
            resolver = new CoverArtResovler("__KeyColorsTest");
        }

        [Test, TestCaseSource("GetAllKeys")]
        public void TestCreateImageForKeys(HashTagModel hashTagModel)
        {
            resolver.ResolveImagePath(hashTagModel);
        }

        public IEnumerable<HashTagModel> GetAllKeys()
        {
            return CircleOfFifths.AllKeys.Select(key => new HashTagModel {new KeyHashTag(key)});
        }

        [Test]
        public void TestCreateImageForMixes()
        {
            resolver.ResolveImagePath(HashTagModel.Parser.All("#mixes"));
        }
    }
}