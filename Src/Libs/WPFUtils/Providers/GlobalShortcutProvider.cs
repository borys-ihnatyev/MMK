using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Interop;

namespace MMK.Wpf.Providers
{
    public class GlobalShortcutProvider : IGlobalShortcutProvider
    {
        private Window window;
        private GlobalShortcut shortcut;
        
        public event Action Pressed;

        public GlobalShortcutProvider(Window window, KeyModifyers modifyer, int keyCode)
        {
            this.window = window;
            shortcut = new GlobalShortcut(modifyer, keyCode, WndSource.Handle);
        }

        protected GlobalShortcutProvider(KeyModifyers modifyer, int keyCode)
        {
            shortcut = new GlobalShortcut(modifyer, keyCode);
        }

        private HwndSource WndSource
        {
            get
            {
                Contract.Ensures(Contract.Result<HwndSource>() != null);
                Contract.EndContractBlock();

                return PresentationSource.FromVisual(window) as HwndSource;
            }
        }

        public bool IsListening { get; private set; }

        private void OnHotKeyPressed()
        {
            if (Pressed != null)
                Pressed();
        }

        private bool IsWndSourceSetted
        {
            get { return WndSource != null; }
        }

        public void StartListen()
        {
            if (IsListening) return;

            if (!IsWndSourceSetted)
                throw new InvalidOperationException("wndSource not setted");

            shortcut.Register();
            WndSource.AddHook(WndProc);
            IsListening = true;
        }

        public void StopListen()
        {
            if (!IsListening) return;

            WndSource.RemoveHook(WndProc);
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
            return msg == GlobalShortcut.WM_HOTKEY && id == shortcut.Id;
        }

        public override int GetHashCode()
        {
            return shortcut.GetHashCode();
        }

        void IGlobalShortcutProvider.SetWindow(Window ownerWindow)
        {
            if (ownerWindow == null)
                throw new ArgumentNullException("ownerWindow");
            Contract.EndContractBlock();

            window = ownerWindow;

            var wasListening = IsListening;
            
            if (IsListening) StopListen();
            
            shortcut = new GlobalShortcut(shortcut, WndSource.Handle);
            
            if(wasListening) StartListen();
        }

        void IGlobalShortcutProvider.StartListen()
        {
            StartListen();
        }

        void IGlobalShortcutProvider.StopListen()
        {
            StopListen();
        }
    }
}