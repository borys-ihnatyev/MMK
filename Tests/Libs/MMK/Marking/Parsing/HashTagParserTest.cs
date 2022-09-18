using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MMK.Marking.Parsing
{
    public class HashTagParserTest
    {
        [Test]
        [TestCaseSource("RandomHashTagsSimple")]
        [TestCaseSource("RandomHashTagsWithMeta")]
        public void Parse(string hashTag)
        {
            var hashTagString = RandomHashTagSimple();

            var hashTagEntry = HashTag.Parser.First(hashTagString);
            Assert.IsNotNull(hashTagEntry);
            Assert.IsNotNull(hashTagEntry.HashTag);
            Assert.AreEqual(hashTagString, hashTagEntry.HashTag.ToString());
        }

        private static IEnumerable<string> RandomHashTagsSimple()
        {
            return Enumerable.Range(0, 30).Select(i => RandomHashTagSimple());
        }

        private static IEnumerable<string> RandomHashTagsWithMeta()
        {
            return Enumerable.Range(0, 30).Select(i => RandomHashTagWithMeta());
        }

        private static string RandomHashTagSimple()
        {
            return HashTag.Hash + Guid.NewGuid();
        }

        private static string RandomHashTagWithMeta()
        {
            return RandomHashTagSimple() + HashTag.Meta + Guid.NewGuid();
        }
    }
}