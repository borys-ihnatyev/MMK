using System;
using System.Diagnostics.Contracts;

namespace MMK.Utils.Media
{
    public struct ColorHsvModel
    {
        public double H { get; set; }
        public double S { get; set; }
        public double V { get; set; }

        [Pure]
        public ColorArgbDoubleModel ToArgbDoubleModel()
        {
            if (Math.Abs(H - 1.0) < 0.0001)
                H = 0.0;

            const double step = 1.0 / 6.0;
            var vh = H / step;

            var i = (int)Math.Floor(vh);

            var f = vh - i;
            var p = V * (1.0 - S);
            var q = V * (1.0 - (S * f));
            var t = V * (1.0 - (S * (1.0 - f)));

            double r, g, b;

            switch (i)
            {
                case 0:
                    {
                        r = V;
                        g = t;
                        b = p;
                        break;
                    }
                case 1:
                    {
                        r = q;
                        g = V;
                        b = p;
                        break;
                    }
                case 2:
                    {
                        r = p;
                        g = V;
                        b = t;
                        break;
                    }
                case 3:
                    {
                        r = p;
                        g = q;
                        b = V;
                        break;
                    }
                case 4:
                    {
                        r = t;
                        g = p;
                        b = V;
                        break;
                    }
                case 5:
                    {
                        r = V;
                        g = p;
                        b = q;
                        break;
                    }
                default:
                    throw new Exception("Unreachable code");
            }

            return new ColorArgbDoubleModel {A = 1.0, R = r, G = g, B = b};
        }

        [Pure]
        public ColorArgbByteModel ToArgbByteModel()
        {
            var color = ToArgbDoubleModel();

            return new ColorArgbByteModel
            {
                A = ToByteColorPart(color.A),
                R = ToByteColorPart(color.R),
                G = ToByteColorPart(color.G),
                B = ToByteColorPart(color.B)
            };
        }
        
        [Pure]
        private static Byte ToByteColorPart(double colorPart)
        {
            return (Byte) (255*colorPart);
        }
    }
}