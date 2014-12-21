using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MMK.Notify.Observer;

namespace MMK.Notify.Model
{
    public class NotifyTypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is NotifyType))
                return null;

            return Convert((NotifyType) value);
        }

        public static SolidColorBrush Convert(NotifyType notifyType)
        {
            var color = ColorConvert(notifyType);
            return new SolidColorBrush(color);
        }

        private static Color ColorConvert(NotifyType notifyType)
        {
            switch (notifyType)
            {
                case NotifyType.Success:
                    return Color.FromRgb(39, 255, 39);
                case NotifyType.Info:
                    return Color.FromRgb(90, 71, 255);
                case NotifyType.Warning:
                    return Color.FromRgb(247, 109, 60);
                default:
                    return Color.FromRgb(247, 39, 71);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}