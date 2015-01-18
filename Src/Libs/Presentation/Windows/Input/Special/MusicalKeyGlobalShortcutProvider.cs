using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace MMK.Presentation.Windows.Input.Special
{
    public class MusicalKeyGlobalShortcutProvider : IGlobalShortcutProvider
    {
        private HwndSource wndSource;
        private MuscalKeyGlobalShortcutRegister shortcutRegister;

        public MusicalKeyGlobalShortcutProvider(Window window)
        {
            wndSource = PresentationSource.FromVisual(window) as HwndSource;
        }

        public MusicalKeyGlobalShortcutProvider()
        {
        }

        private bool IsWndSourceSetted
        {
            get { return wndSource != null; }
        }

        public void StartListen()
        {
            if (IsListening) return;

            if (!IsWndSourceSetted)
                throw new InvalidOperationException("wndSource not setted");

            shortcutRegister = new MuscalKeyGlobalShortcutRegister(wndSource.Handle);
            shortcutRegister.Register();
            wndSource.AddHook(WndProc);
            IsListening = true;
        }

        public void StopListen()
        {
            if (!IsListening) return;

            wndSource.RemoveHook(WndProc);
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

        void IGlobalShortcutProvider.SetWindow(Window window)
        {
            wndSource = PresentationSource.FromVisual(window) as HwndSource;
            Debug.Assert(wndSource != null, "wndSource != null");
            if (IsListening)
                throw new InvalidOperationException("Cant change wndSource while is listening shortcuts");
        }
    }
}