using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MMK.Marking;
using NUnit.Framework;

namespace MMK.Tests.Marking
{
    [TestFixture]
    public class HashTagTest
    {
        public void TestHashTagSerialization()
        {
            var expectedHashTag = new HashTag("HelloWorld");
            var serializer = new BinaryFormatter();
            using (var stream = new StreamWriter("HelloWorld.xml"))
            {
                serializer.Serialize(stream.BaseStream, expectedHashTag);
            }

            using (var stream = new StreamReader("HelloWorld.xml"))
            {
                var deserializedHashTag = (HashTag)serializer.Deserialize(stream.BaseStream);
                Assert.AreEqual(expectedHashTag, deserializedHashTag);
            }
        }

        [Test]
        public void TestKeyHashTagSerialization()
        {   
            var expectedHashTag = new KeyHashTag(new Key(Note.Ais,Tone.Moll));
            var serializer = new BinaryFormatter();
            using (var stream = new StreamWriter("AisMoll.xml"))
            {
                serializer.Serialize(stream.BaseStream, expectedHashTag);
            }

            using (var stream = new StreamReader("AisMoll.xml"))
            {
                var deserializedHashTag = (KeyHashTag)serializer.Deserialize(stream.BaseStream);
                Assert.AreEqual(expectedHashTag.TagValue, deserializedHashTag.TagValue);
            }
        }
    }
}
