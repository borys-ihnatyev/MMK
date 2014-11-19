using System.Runtime.InteropServices;

namespace System.IO
{
    public static partial class DriveInfoFactory
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceBroadcastVolume
        {
            public int size;
            public int deviceType;
            public int reserved;
            public int unitMask;
        }
    }
}