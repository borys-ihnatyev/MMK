using System.Runtime.InteropServices;

namespace MMK.Wpf.Windows
{
    public partial class Taskbar
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public readonly int left;
            public readonly int top;
            public readonly int right;
            public readonly int bottom;
        }
    }
}