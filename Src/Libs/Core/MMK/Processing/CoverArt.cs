using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using MMK.Marking.Representation;

namespace MMK.Processing
{
    public abstract class CoverArt
    {
        private static readonly Rectangle ImageRect;
        protected static readonly Rectangle FigureRect;
        protected readonly HashTagModel HashTagModel;

        static CoverArt()
        {
            const int imageSize = 500;
            const int figureMargin = 30;
            const int figureSize = imageSize - figureMargin*2;

            ImageRect = new Rectangle(0, 0, imageSize, imageSize);
            FigureRect = new Rectangle(figureMargin, figureMargin, figureSize, figureSize);
        }

        protected CoverArt(HashTagModel hashTagModel)
        {
            if (hashTagModel == null)
                throw new ArgumentNullException("hashTagModel");
            Contract.EndContractBlock();

            HashTagModel = hashTagModel;
        }

        public abstract bool CanCreate { get; }

        public abstract string ImageName { get; }

        public Image CreateImage()
        {
            _CheckState();

            var image = new Bitmap(ImageRect.Width, ImageRect.Height);
            var g = Graphics.FromImage(image);
            g.FillRectangle(Brushes.SeaShell, ImageRect);
            OnDrawImage(g);

            return image;
        }

        protected void _CheckState()
        {
            if (!CanCreate)
                throw new InvalidOperationException("Can't crate image for target HashTagModel");
        }

        protected abstract void OnDrawImage(Graphics g);
    }
}