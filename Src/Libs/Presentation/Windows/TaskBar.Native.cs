using System;
using System.Runtime.InteropServices;

namespace MMK.Presentation.Windows
{
    public partial class Taskbar
    {
        private static class Native
        {
            [DllImport("shell32.dll", SetLastError = true)]
            public static extern IntPtr SHAppBarMessage(Abm dwMessage, [In] ref AppBarData pData);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        }
    }

}