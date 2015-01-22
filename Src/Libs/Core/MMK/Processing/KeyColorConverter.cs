using MMK.Utils.Media;

namespace MMK.Processing
{
    public static class KeyColorConverter
    {
        private static ColorHsvModel ToHsvColorModel(Key key)
        {
            var color = new ColorHsvModel {H = 0.0, S = 1.0, V = 1.0};
            const double hStep = 1.0/12;

            var keyToneEnumer = CircleOfFifths.MinorKeys.GetEnumerator();

            if (key.IsMoll())
            {
                color.S = 0.9;
                color.V = 0.9;
            }
            else
            {
                color.S = 1.0;
                color.V = 1.0;
                keyToneEnumer = CircleOfFifths.MajorKeys.GetEnumerator();
            }

            while (keyToneEnumer.MoveNext())
            {
                color.H += hStep;
                if (keyToneEnumer.Current == key)
                    break;
            }

            return color;
        }

        public static ColorArgbDoubleModel ToArgbDoubleModel(Key key)
        {
            return ToHsvColorModel(key).ToArgbDoubleModel();
        }

        public static ColorArgbByteModel ToArgbByteModel(Key key)
        {
            return ToHsvColorModel(key).ToArgbByteModel();
        }
    }
}