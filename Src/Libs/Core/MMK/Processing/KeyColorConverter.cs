using MMK.Utils.Media;

namespace MMK.Processing
{
    public static class KeyColorConverter
    {
        private static ColorHsvModel ToHsvColorModel(Key key)
        {
            var color = new ColorHsvModel {H = 0.0, S = 1.0, V = 1.0};
            const double hStep = ColorHsvModel.HueMax/NoteFactory.NotesCount;

            var keyToneEnumer = (key.IsMoll() ? CircleOfFifths.MinorKeys : CircleOfFifths.MajorKeys).GetEnumerator();

            while (keyToneEnumer.MoveNext())
            {
                if (keyToneEnumer.Current == key)
                    break;
                color.H += hStep;

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