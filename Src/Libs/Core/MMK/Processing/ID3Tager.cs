using System;
using System.IO;
using MMK.Marking.Representation;
using MMK.Utils;
using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;
using IOFile = System.IO.File;
using Tag = TagLib.Tag;

namespace MMK.Processing
{
    public partial class Id3Tager
    {
        private static string GetKeyColorImagePath(Key key)
        {
            return string.Format(@"KeyColors\{0}.png", key.ToString(KeyNotation.IsMollDur));
        }

        public Id3Tager(string filePath, TrackNameModel trackNameModel)
        {
            if (trackNameModel == null)
                throw new ArgumentNullException("trackNameModel");

            CheckFileMatch(filePath);

            this.filePath = filePath;
            this.trackNameModel = trackNameModel;
        }

        public Id3Tager(string filePath)
        {
            CheckFileMatch(filePath);

            FilePath = filePath;
        }

        private static void CheckFileMatch(string filePath)
        {
            if (!IOFile.Exists(filePath))
                throw new FileNotFoundException("File Not Found", filePath);

            if (!FileExtensionParser.IsMp3(filePath))
                throw new NotMp3FileException();
        }

        private string filePath;

        private TrackNameModel trackNameModel;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (!IOFile.Exists(value))
                    throw new FileNotFoundException("File not found ", value);

                filePath = value;
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                trackNameModel = TrackNameModel.Parser.Parse(fileNameWithoutExtension);
            }
        }

        private void TrySetId3Tag()
        {
            using (var mp3File = File.Create(filePath))
            {
                SetId3Tag(mp3File);
                mp3File.Save();
            }
        }

        private void SetId3Tag(File mp3File)
        {
            var tag = ReadOrCreateId3Tag(mp3File);

            SetTagMainInfo(tag);

            if (trackNameModel.HashTagModel.IsEmpty())
                return;

            if (trackNameModel.MainKey != null)
                SetTagMainKey(tag);

            tag.Comment = trackNameModel.HashTagModel.ToString();
        }

        private static Tag ReadOrCreateId3Tag(File mp3File)
        {
            var tag = mp3File.GetTag(TagTypes.Id3v1 | TagTypes.Id3v2, true);

            if (tag != null)
                return tag;

            tag = new TagLib.Id3v2.Tag();
            tag.CopyTo(mp3File.Tag, true);
            tag = mp3File.Tag;

            return tag;
        }

        private void SetTagMainInfo(Tag tag)
        {
            tag.Title = trackNameModel.FullTitle;
            tag.Performers = new[] {trackNameModel.ArtistsString};
            tag.AlbumArtists = new[] {"VA"};
        }

        private void SetTagMainKey(Tag tag)
        {
            SetTagCoverArt(tag);

            var id3V2Tag = tag as TagLib.Id3v2.Tag;

            if (id3V2Tag == null)
                return;

            var tagKeyFrame = TextInformationFrame.Get(id3V2Tag , "TKEY", true);

            tagKeyFrame.Text = new[]
            {
                trackNameModel.MainKey.ToString(KeyNotation.Sharp_M)
            };
        }

        private void SetTagCoverArt(Tag tag)
        {
            tag.Pictures = new IPicture[]
            {
                new Picture(GetKeyColorImagePath(trackNameModel.MainKey))
                {
                    Type = PictureType.FrontCover
                }
            };
        }

        public void Tag(bool removeOldTags = false)
        {
            if (removeOldTags)
                RemoveTags();

            TrySetId3Tag();
        }

        public void RemoveTags()
        {
            using (var mp3File = File.Create(filePath))
            {
                mp3File.RemoveTags(TagTypes.AllTags);
                mp3File.Save();
            }
        }
    }
}