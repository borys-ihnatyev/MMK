using NUnit.Framework;

namespace MMK
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
            foreach (var t in notesInQuintOrder)
            {
                Assert.AreEqual(t + tone.ToString().ToLower(), key.ToString());
                key = CircleOfFifths.GetNext(key);
            }
        }

        [Test]
        public void Relative_KeyTestMoll()
        {
            var key = new Key(Note.A, Tone.Moll);
            var expectedRelative = new Key(Note.C, Tone.Dur);
            Assert.AreEqual(expectedRelative, CircleOfFifths.GetParalel(key));
        }

        [Test]
        public void Relative_KeyTestDur()
        {
            var key = new Key(Note.C, Tone.Dur);
            var expectedRelative = new Key(Note.A, Tone.Moll);
            Assert.AreEqual(expectedRelative, CircleOfFifths.GetParalel(key));
        }
    }
}
