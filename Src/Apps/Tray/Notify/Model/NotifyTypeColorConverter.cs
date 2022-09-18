using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MMK.Notify.Observer;
using MMK.PresentationStyles;

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
            return Styles.Get<SolidColorBrush>(notifyType + "ColorBrush");
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}