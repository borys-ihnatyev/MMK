using System.Globalization;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public static class ControlExtension
    {
        public static Size MeasureText(this TextBox control)
        {
            return MeasureText(control, control.Text);
        }

        public static Size MeasureText(this Control control, string text)
        {
            var formattedText = new FormattedText(
                MakeStringEtalone(text),
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    control.FontFamily,
                    control.FontStyle,
                    control.FontWeight,
                    control.FontStretch),
                control.FontSize,
                control.Foreground
                );

            return new Size(formattedText.Width, formattedText.Height);
        }

        private static string MakeStringEtalone(string str)
        {
            return str.Replace(' ', '_');
        }

        public static Size MeasureText(this TextBlock control)
        {
            var formattedText = new FormattedText(
                MakeStringEtalone(control.Text),
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    control.FontFamily,
                    control.FontStyle,
                    control.FontWeight,
                    control.FontStretch),
                control.FontSize,
                control.Foreground
                );

            return new Size(formattedText.Width, formattedText.Height);
        }

        
    }
}
