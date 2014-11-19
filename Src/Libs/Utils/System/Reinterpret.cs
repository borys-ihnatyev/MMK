using System.Runtime.InteropServices;

namespace System
{
    public static class Reinterpret
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct ArrayUnion
        {
            [FieldOffset(0)]
            public byte[] asByte;
            [FieldOffset(0)]
            public int[] asInt;

            public unsafe ArrayUnion AcceptLength(int newBlockSize, int originalBlockSize = 1)
            {
                if ((asByte.Length * originalBlockSize) % newBlockSize != 0) 
                    throw new InvalidCastException("Invalid new block size.");
                fixed (byte* pFirstByte = asByte)
                {
                    var size = (int*)(pFirstByte - 4);
                    *size = (asByte.Length * originalBlockSize) / newBlockSize;
                }
                return this;
            }
        }

        public static int[] Cast(byte[] value)
        {
            return new ArrayUnion { asByte = value }.AcceptLength(sizeof(int)).asInt;
        }

        public static byte[] Cast(int[] value)
        {
            return new ArrayUnion { asInt = value }.AcceptLength(sizeof(byte), sizeof(int)).asByte;
        }
    }
}
