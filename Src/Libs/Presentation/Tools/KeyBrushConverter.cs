using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MMK.Marking;
using MMK.Processing;

namespace MMK.Presentation.Tools
{
    public class KeyBrushConverter : IValueConverter
    {
        public KeyBrushConverter()
        {
            DefaultBrush = Brushes.Transparent;
        }

        public Brush DefaultBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var keyHashTag = value as KeyHashTag;

            if (keyHashTag == null)
                return DefaultBrush;

            return Convert(keyHashTag.Key);
        }

        public static SolidColorBrush Convert(Key key)
        {
            var color = KeyColorConverter.ToArgbDoubleModel(key);

            return new SolidColorBrush(
                Color.FromScRgb(
                    (float) color.A,
                    (float) color.R,
                    (float) color.G,
                    (float) color.B
                    )
                );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}