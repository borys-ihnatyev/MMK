using System;
using MMK.SwiftSearch.SearchHandlers;
using NUnit.Framework;

namespace MMK.SwiftSearch.Tests.SearchHandlers
{
    [TestFixture]
    public class MultiSearchHandlerTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            
        }

        [Test]
        [TestCase("@vk @g hello world")]
        [TestCase("@vk hello world @g")]
        [TestCase("hello world @vk @g")]
        [TestCase("hello@vk  world@g")]
        [TestCase("@vkhello world@g")]
        public void MustRemoveAllSearchHandlerIdentifyers(string search)
        {
            var handler = new MultiSearchHandler(search);
            var searchWithoutIdentifyers = handler.GetPureSearch();
            Assert.False(searchWithoutIdentifyers.Contains("@"),"all handlers not removed");
        }

        [Test]
        public void TestRefArgs()
        {

        }
    }
}
