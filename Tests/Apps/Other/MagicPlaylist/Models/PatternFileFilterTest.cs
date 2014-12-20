using MMK.MagicPlaylist.Base;
using MMK.Marking.Representation;
using NUnit.Framework;

namespace MMK.MagicPlaylist.Models
{
    [TestFixture]
    public class PatternFileFilterTest : FileUsageTestFixture
    {
        private PatternFileFilter filter;

        public override void SetUp()
        {
            filter = new PatternFileFilter();
            base.SetUp();
        }

        [Test]
        public void EmptyPattern_MatchAll()
        {
            for (var i = 0; i < 10; i++)
                Assert.IsTrue(filter.Match(CreateFile(".mp3")));
        }

        [Test]
        public void NotEmptyPattern_Match()
        {
            filter.Pattern = HashTagModel.Parser.All("#pop");
            for (var i = 0; i < 10; i++)
                Assert.IsTrue(filter.Match(CreateFile(" #entropy1 #pop #entropy.mp3")));
        }
    }
}