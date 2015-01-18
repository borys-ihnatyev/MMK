using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MMK.Marking;
using MMK.Presentation.Windows.Media;

namespace MMK.Presentation.Tools
{
    public class KeyBrushConverter : IValueConverter
    {
        public KeyBrushConverter()
        {
            DefaultBrush = Brushes.Transparent;
        }

        public Brush DefaultBrush
        {
            get; set;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var keyHashTag = value as KeyHashTag;
            
            if (keyHashTag == null)
                return DefaultBrush;

            return Convert(keyHashTag.Key);
        }

        public static SolidColorBrush Convert(Key key) 
        {
            var color = new ColorHsv { H = 0.0, S = 1.0, V = 1.0 };
            const double hStep = 1.0 / 12;

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

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
