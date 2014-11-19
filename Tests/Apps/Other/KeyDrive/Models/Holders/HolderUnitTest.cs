using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace MMK.KeyDrive.Models.Holders
{
    public abstract class HolderUnitTest : UnitTest
    {
        public static void Serialize<T>(T expectedVm, string serializatioFileName) where T : Holder
        {
            Assert.NotNull(expectedVm);

            var serializer = new BinaryFormatter();

            using (var stream = File.Create(serializatioFileName))
            {
                serializer.Serialize(stream, expectedVm);
            }
        }

        public static T Deserialize<T>(string serializatioFileName) where T : Holder
        {
            var stream = File.OpenRead(serializatioFileName);
            try
            {
                var actualVm = new BinaryFormatter().Deserialize(stream) as T;
                Assert.NotNull(actualVm);
                return actualVm;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is Holder.NotFoundException)
                    throw ex.InnerException;
                throw;
            }
            finally
            {
                stream.Close();                
                File.Delete(serializatioFileName);
            }
        }
    }
}