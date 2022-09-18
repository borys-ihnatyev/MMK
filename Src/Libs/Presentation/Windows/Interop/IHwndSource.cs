using System;
using System.Windows.Interop;

namespace MMK.Presentation.Windows.Interop
{
    public interface IHwndSource
    {
        IntPtr Handle { get; }
        void AddHook(HwndSourceHook hook);
        void RemoveHook(HwndSourceHook hook);
    }
}