using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using MMK.Marking;
using MMK.Marking.Representation;

namespace MMK.Processing
{
    public class KeyCoverArtFactory : CoverArtFactory
    {
        public KeyCoverArtFactory(string directory) : base(directory)
        {
        }

        public override string RetriveImagePath(HashTagModel hashTagModel)
        {
            if (hashTagModel == null)
                throw new ArgumentNullException("hashTagModel");

            var keyHashTag = hashTagModel.OfType<KeyHashTag>().FirstOrDefault();
            if (keyHashTag == null)
                return null;

            return RetriveImagePath(keyHashTag.Key);
        }

        /// <summary>
        /// returns path to coverart for specific key,
        /// if not founded it creates new image and saves it
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string RetriveImagePath(Key key)
        {
            if (key == null)
                throw new ArgumentNullException(@"key");
            Contract.EndContractBlock();

            var imagePath = GetImagePath(key);
            if (!File.Exists(imagePath))
                CreateAndSaveImage(key, imagePath);

            return imagePath;
        }

        private string GetImagePath(Key key)
        {
            return Path.Combine(CoverArtsDir.FullName,
                String.Format(@"{0}.png", key.ToString(KeyNotation.CamelotWithoutTone)));
        }

        private static void CreateAndSaveImage(Key key, string imagePath)
        {
            var image = CreateImage(key);
            image.Save(imagePath, ImageFormat.Png);
        }

        private static Bitmap CreateImage(Key key)
        {
            var image = new Bitmap(500, 500);
            var g = Graphics.FromImage(image);
            g.FillRectangle(Brushes.SeaShell, 0, 0, 500, 500);
            var keyBrush = CreateKeyBrush(key);
            g.FillEllipse(keyBrush, 30, 30, 440, 440);
            return image;
        }

        private static Brush CreateKeyBrush(Key key)
        {
            var color = KeyColorConverter.ToArgbByteModel(key);
            return new SolidBrush(Color.FromArgb(color.Argb));
        }
    }
}