using System.Runtime.InteropServices;

namespace MMK.Utils.Media
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ColorArgbByteModel : IColorArgbModel<byte>
    {
        [FieldOffset(0)] private readonly int argb;
        [FieldOffset(0)] private byte a;
        [FieldOffset(1)] private byte r;
        [FieldOffset(2)] private byte g;
        [FieldOffset(3)] private byte b;

        public byte A
        {
            get { return a; }
            set { a = value; }
        }

        public byte R
        {
            get { return r; }
            set { r = value; }
        }

        public byte G
        {
            get { return g; }
            set { g = value; }
        }

        public byte B
        {
            get { return b; }
            set { b = value; }
        }

        public int Argb
        {
            get { return argb; }
        }
    }
}