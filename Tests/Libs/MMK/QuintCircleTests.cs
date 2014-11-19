using NUnit.Framework;

namespace MMK.Tests
{
    [TestFixture]
    public class QuintCircleTest
    {
        private readonly string[] notesInQuintOrder = 
        {
            "f",
            "c",
            "g",
            "d",
            "a",
            "e",
            "b",
            "fis",
            "cis",
            "gis",
            "dis",
            "ais"
        };

        [Test]
        [TestCase(Tone.Moll)]
        [TestCase(Tone.Dur)]
        public void Line(Tone tone)
        {

            var key = new Key(Note.F, tone);
            for (int i = 0; i < notesInQuintOrder.Length; i++)
            { 
                Assert.AreEqual(notesInQuintOrder[i] + tone.ToString().ToLower(), key.ToString());
                key = CircleOfFifths.GetNext(key);
            }
        }
    }
}
