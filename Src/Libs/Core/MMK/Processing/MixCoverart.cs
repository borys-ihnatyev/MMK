using System.Drawing;
using System.Linq;
using MMK.Marking.Representation;

namespace MMK.Processing
{
    class MixCoverArt : CoverArt
    {
        private readonly bool canCreate;

        public MixCoverArt(HashTagModel hashTagModel)
            : base(hashTagModel)
        {
            canCreate = hashTagModel.Select(h => h.TagValue).Any(t => t.StartsWith("mix"));
        }

        public override bool CanCreate
        {
            get { return canCreate; }
        }

        public override string ImageName
        {
            get { return "mixes"; }
        }

        protected override void OnDrawImage(Graphics graphics)
        {
            graphics.FillEllipse(Brushes.Black, FigureRect);
        }
    }
}