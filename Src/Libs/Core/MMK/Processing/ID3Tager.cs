using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using MMK.Marking.Representation;
using MMK.Utils;
using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;
using IOFile = System.IO.File;

namespace MMK.Processing
{
    public partial class Id3Tager
    {
        private static readonly CoverArtResovler CoverArtResovler = new CoverArtResovler(@"KeyColors");

        private readonly string filePath;

        private TagLib.Id3v2.Tag tag;

        private readonly TrackNameModel trackNameModel;

        public Id3Tager(string filePath, TrackNameModel trackNameModel)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");
            if (!IOFile.Exists(filePath))
                throw new FileNotFoundException("File Not Found", filePath);
            if (!FileExtensionParser.IsMp3(filePath))
                throw new NotMp3FileException();
            if (trackNameModel == null)
                throw new ArgumentNullException("trackNameModel");
            Contract.EndContractBlock();

            this.filePath = filePath;
            this.trackNameModel = trackNameModel;
        }

        public string FilePath
        {
            get { return filePath; }
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

        private void TrySetId3Tag()
        {
            using (var mp3File = File.Create(filePath))
            {
                SetTags(mp3File);
                mp3File.Save();
            }
        }

        private void SetTags(File mp3File)
        {
            tag = (TagLib.Id3v2.Tag) mp3File.GetTag(TagTypes.Id3v2, true);
            SetTagMainInfo();
            SetTagMainKey();
            SetTagCoverArt();
        }

        private void SetTagMainInfo()
        {
            tag.Title = trackNameModel.FullTitle;
            tag.Performers = new[] {trackNameModel.ArtistsString};
            tag.AlbumArtists = new[] {"VA"};
            tag.Comment = trackNameModel.HashTagModel.ToString();

            //todo refactor hardcoded behavior
            if (trackNameModel.HashTagModel.Select(h => h.TagValue).Any(t => t.StartsWith("mix")))
                tag.Album = "mixes";
        }

        private void SetTagMainKey()
        {
            if (trackNameModel.MainKey == null)
                return;
            var tagKeyFrame = TextInformationFrame.Get(tag, "TKEY", true);
            tagKeyFrame.Text = new[] {trackNameModel.MainKey.ToString(KeyNotation.Sharp_M)};
            tag.AddFrame(tagKeyFrame);
        }

        private void SetTagCoverArt()
        {
            var imagePath = CoverArtResovler.ResolveImagePath(trackNameModel.HashTagModel);
            if (imagePath == null)
                return;
            tag.Pictures = new IPicture[] {new Picture(imagePath) {Type = PictureType.FrontCover}};
        }
    }
}