using System;
using System.Runtime.InteropServices;

namespace MMK.Presentation.Windows
{
    public partial class Taskbar
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct AppBarData
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public Abe uEdge;
            public Rect rc;
            public int lParam;
        }
    }
}