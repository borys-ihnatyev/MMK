using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MMK.Notify.Model
{
    class BooleanToColorErrorConverter : IValueConverter
    {
        public BooleanToColorErrorConverter()
        {
            ErrorColor = Color.FromRgb(180, 10, 0);
            OkColor = Color.FromRgb(0, 0, 0);
        }

        public Color ErrorColor { get; set; }
        public Color OkColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hasError = !(bool) value;
            return new SolidColorBrush(hasError ? ErrorColor : OkColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as SolidColorBrush;
            if (brush == null) return null;

            return brush.Color == OkColor;
        }
    }
}
