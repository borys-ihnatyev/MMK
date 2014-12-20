using System;
using System.IO;
using MMK.MagicPlaylist.Base;
using NUnit.Framework;

namespace MMK.MagicPlaylist.Models
{
    [TestFixture]
    public class MagicPlayListGeneratorTest : FileUsageTestFixture
    {
        private MagicPlaylistGenerator playlistGenerator;

        [SetUp]
        public override void SetUp()
        {
            playlistGenerator = new MagicPlaylistGenerator();
            base.SetUp();
        }

        [Test]
        public void Generate_Empty_Playlist_When_No_Files()
        {
            var paths = playlistGenerator.Generate();
            Assert.IsEmpty(paths);
        }

        [Test]
        [ExpectedException(typeof (FileNotFoundException))]
        public void Throws_NotFound_When_FileNotExists()
        {
            playlistGenerator.Add(Guid.NewGuid().ToString());
        }

        [Test]
        [ExpectedException(typeof (MagicPlaylistGenerator.UnsupportedFormatException))]
        public void Throws_UnsupportedFormat_When_NotIsMp3()
        {
            var file = CreateFile(".mp4");
            File.Create(file).Close();
            playlistGenerator.Add(file);
        }

        [Test]
        public void Ok_When_AddExistedMp3File()
        {
            playlistGenerator.Add(CreateFile(".mp3"));
        }
    }
}