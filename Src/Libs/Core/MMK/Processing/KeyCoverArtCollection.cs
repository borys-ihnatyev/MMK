using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MMK.Processing
{
    public class KeyCoverArtCollection
    {
        private readonly DirectoryInfo coverArtsDir;
        
        public KeyCoverArtCollection(string directory)
        {
            if(String.IsNullOrWhiteSpace(directory))
                throw new ArgumentException("invalid directory name","directory");
            coverArtsDir = new DirectoryInfo(directory);

            if (coverArtsDir.Exists) return;
            
            coverArtsDir.Create();
            coverArtsDir.Refresh();
        }

        /// <summary>
        /// returns path to coverart for specific key,
        /// if not founded it creates new image and saves it
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string RetriveImagePath(Key key)
        {
            var imagePath = GetImagePath(key);
            if (!File.Exists(imagePath))
                CreateImage(key, imagePath);
            return imagePath;
        }

        private string GetImagePath(Key key)
        {
            return Path.Combine(coverArtsDir.FullName, String.Format(@"{0}.png", key.ToString(KeyNotation.CamelotWithoutTone)));
        }

        private static void CreateImage(Key key, string imagePath)
        {
            var image = new Bitmap(500, 500);
            var g = Graphics.FromImage(image);
            g.FillRectangle(Brushes.Transparent, 0, 0, 500, 500);
            var keyBrush = CreateKeyBrush(key);
            g.FillEllipse(keyBrush, 0, 0, 500, 500);
            image.Save(imagePath, ImageFormat.Png);
        }

        private static Brush CreateKeyBrush(Key key)
        {
            var color = KeyColorConverter.ToArgbByteModel(key);
            return new SolidBrush(Color.FromArgb(color.Argb));
        }
    }
}