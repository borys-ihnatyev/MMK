using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MMK.Marking.Representation;
using NUnit.Framework;

namespace MMK.Marking
{
    [TestFixture]
    public class HashTagModelTest
    {
        [Test]
        public void TestSerialization()
        {
            var expectedHashTagModel = HashTagModel.Parser.All("#dismoll #pop #house");

            var serializer = new BinaryFormatter();
            using (var stream = new StreamWriter("TestHashTagModel.bin"))
            {
                serializer.Serialize(stream.BaseStream, expectedHashTagModel);
            }

            using (var stream = new StreamReader("TestHashTagModel.bin"))
            {
                var deserializedHashTagModel = (HashTagModel)serializer.Deserialize(stream.BaseStream);
                Assert.AreEqual(expectedHashTagModel, deserializedHashTagModel);
            }

        }
    }
}
