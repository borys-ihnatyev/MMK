using System.Drawing;
using System.Linq;
using MMK.Marking;
using MMK.Marking.Representation;

namespace MMK.Processing
{
    class KeyCoverArt : CoverArt
    {
        private readonly Key mainKey;

        public KeyCoverArt(HashTagModel hashTagModel) : base(hashTagModel)
        {
            mainKey = hashTagModel.OfType<KeyHashTag>().Select(h => h.Key).FirstOrDefault();
        }

        public override bool CanCreate
        {
            get { return mainKey != null; }
        }

        public override string ImageName
        {
            get
            {
                _CheckState();
                return mainKey.ToString(KeyNotation.CamelotWithoutTone);
            }
        }

        protected override void OnDrawImage(Graphics graphics)
        {
            graphics.FillEllipse(CreateKeyBrush(mainKey), FigureRect);
        }

        private static Brush CreateKeyBrush(Key key)
        {
            var color = KeyColorConverter.ToArgbByteModel(key);
            return new SolidBrush(Color.FromArgb(color.Argb));
        }
    }
}