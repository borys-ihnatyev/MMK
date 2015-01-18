using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Input;
using MMK.Presentation.Windows.Interop;

namespace MMK.Presentation.Windows.Input
{
    public class GlobalShortcutProvider : IGlobalShortcutProvider
    {
        private readonly IHwndSource hwndSource;
        private readonly GlobalShortcut shortcut;

        public event Action Pressed;

        public GlobalShortcutProvider(IHwndSource hwndSource, ModifierKeys modifier, System.Windows.Input.Key key)
        {
            if(hwndSource == null)
                throw new ArgumentNullException("hwndSource");
            Contract.EndContractBlock();

            this.hwndSource = hwndSource;
            shortcut = new GlobalShortcut(modifier, key, hwndSource.Handle);
        }

        public bool IsListening { get; private set; }

        private void OnHotKeyPressed()
        {
            if (Pressed != null)
                Pressed();
        }

        public void StartListen()
        {
            if (IsListening) return;

            shortcut.Register();
            hwndSource.AddHook(WndProc);
            IsListening = true;
        }

        public void StopListen()
        {
            if (!IsListening) return;

            hwndSource.RemoveHook(WndProc);
            shortcut.Unregister();
            IsListening = false;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (CanHandleMessage(msg, wParam.ToInt64()))
            {
                OnHotKeyPressed();
                handled = true;
            }
            return IntPtr.Zero;
        }

        private bool CanHandleMessage(int msg, long id)
        {
            return msg == GlobalShortcut.WmHotkey && id == shortcut.Id;
        }

        public override int GetHashCode()
        {
            return shortcut.GetHashCode();
        }
    }
}