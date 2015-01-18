using System;
using System.Diagnostics.Contracts;
using System.Windows;
using MMK.Presentation.Windows.Interop;

namespace MMK.Presentation.Windows.Input.Special
{
    public class MusicalKeyGlobalShortcutProvider : IGlobalShortcutProvider
    {
        private readonly IHwndSource hwndSource;
        private readonly MuscalKeyGlobalShortcutRegister shortcutRegister;

        public MusicalKeyGlobalShortcutProvider(IHwndSource hwndSource)
        {
            if (hwndSource == null)
                throw new ArgumentNullException("hwndSource");
            Contract.EndContractBlock();

            this.hwndSource = hwndSource;
            shortcutRegister = new MuscalKeyGlobalShortcutRegister(hwndSource.Handle);
        }


        public void StartListen()
        {
            if (IsListening) return;

            shortcutRegister.Register();
            hwndSource.AddHook(WndProc);
            IsListening = true;
        }

        public void StopListen()
        {
            if (!IsListening) return;

            hwndSource.RemoveHook(WndProc);
            shortcutRegister.Unregister();
            IsListening = false;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == GlobalShortcut.WmHotkey)
            {
                var key = MusicalKeyGlobalShortcut.DecodeKey(wParam.ToInt32());
                OnPressed(key);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void OnPressed(Key key)
        {
            if (Pressed != null)
                Pressed(key);
        }

        public event Action<Key> Pressed;

        public bool IsListening { get; private set; }
    }
}