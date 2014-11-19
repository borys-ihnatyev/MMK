namespace System.Windows.Media
{
    public struct ColorHSV
    {
        public double H { get; set; }
        public double S { get; set; }
        public double V { get; set; }

        public static implicit operator Color(ColorHSV color)
        {
            double r, g, b;
            if (color.H == 1.0)
            {
                color.H = 0.0;
            }

            double step = 1.0 / 6.0;
            double vh = color.H / step;

            int i = (int)Math.Floor(vh);

            double f = vh - i;
            double p = color.V * (1.0 - color.S);
            double q = color.V * (1.0 - (color.S * f));
            double t = color.V * (1.0 - (color.S * (1.0 - f)));

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
                    {
                        // not possible - if we get here it is an internal error
                        throw new ArgumentException();
                    }
            }
            return Color.FromScRgb(1.0f, (float)r, (float)g, (float)b);
        }
    }
}
