using System;
using System.Diagnostics.Contracts;
using System.Drawing.Imaging;
using System.IO;
using MMK.Marking.Representation;

namespace MMK.Processing
{
    public class CoverArtResovler
    {
        private readonly DirectoryInfo coverArtsDir;

        public CoverArtResovler(string directory)
        {
            if (String.IsNullOrWhiteSpace(directory))
                throw new ArgumentException("invalid directory name", "directory");
            Contract.EndContractBlock();

            coverArtsDir = new DirectoryInfo(directory);

            if (coverArtsDir.Exists) return;

            coverArtsDir.Create();
            coverArtsDir.Refresh();
        }

        public string ResolveImagePath(HashTagModel hashTagModel)
        {
            CoverArt coverArt = new MixCoverArt(hashTagModel);
            if (coverArt.CanCreate)
                return ResolveImagePath(coverArt);
            coverArt = new KeyCoverArt(hashTagModel);
            if (coverArt.CanCreate)
                return ResolveImagePath(coverArt);
            return null;
        }

        private string ResolveImagePath(CoverArt coverArt)
        {
            var imagePath = ExpandCoverArtImagePath(coverArt.ImageName);
            if (!File.Exists(imagePath))
                coverArt.CreateImage().Save(imagePath, ImageFormat.Png);
            return imagePath;
        }

        private string ExpandCoverArtImagePath(string imageName)
        {
            return Path.Combine(coverArtsDir.FullName, imageName + ".png");
        }
    }
}