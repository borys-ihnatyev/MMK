// ReSharper disable once CheckNamespace
namespace System.Windows.Media
{
    public struct ColorHsv
    {
        public double H { get; set; }
        public double S { get; set; }
        public double V { get; set; }

        public static implicit operator Color(ColorHsv color)
        {
            if (Math.Abs(color.H - 1.0) < 0.0001)
                color.H = 0.0;

            const double step = 1.0/6.0;
            var vh = color.H/step;

            var i = (int) Math.Floor(vh);

            var f = vh - i;
            var p = color.V*(1.0 - color.S);
            var q = color.V*(1.0 - (color.S*f));
            var t = color.V*(1.0 - (color.S*(1.0 - f)));

            double r, g, b;

            switch (i)
            {
                case 0:
                {
                    r = color.V;
                    g = t;
                    b = p;
                    break;
                }
                case 1:
                {
                    r = q;
                    g = color.V;
                    b = p;
                    break;
                }
                case 2:
                {
                    r = p;
                    g = color.V;
                    b = t;
                    break;
                }
                case 3:
                {
                    r = p;
                    g = q;
                    b = color.V;
                    break;
                }
                case 4:
                {
                    r = t;
                    g = p;
                    b = color.V;
                    break;
                }
                case 5:
                {
                    r = color.V;
                    g = p;
                    b = q;
                    break;
                }
                default:
                    throw new Exception("Unreachable code");
            }

            return Color.FromScRgb(1.0f, (float) r, (float) g, (float) b);
        }
    }
}