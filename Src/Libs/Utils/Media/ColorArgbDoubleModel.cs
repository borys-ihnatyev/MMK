using System;

namespace MMK.Utils.Media
{
    public struct ColorArgbDoubleModel : IColorArgbModel<double>
    {
        private double b;
        private double g;
        private double r;
        private double a;

        public double A
        {
            get { return a; }
            set { a = Sanitize(value); }
        }

        public double R
        {
            get { return r; }
            set { r = Sanitize(value); }
        }

        public double G
        {
            get { return g; }
            set { g = Sanitize(value); }
        }

        public double B
        {
            get { return b; }
            set { b = Sanitize(value); }
        }

        private static double Sanitize(double value)
        {
            if (value < 0)
                value = Math.Abs(value);
            return value > 1 ? 1 : value;
        }
    }
}