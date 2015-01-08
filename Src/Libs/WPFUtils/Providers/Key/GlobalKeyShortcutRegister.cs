using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MMK.Wpf.Providers.Key
{
    class GlobalKeyShortcutRegister
    {
        private readonly IntPtr hwnd;
        private readonly LinkedList<GlobalKeyShortcut> globalHotKeys;

        public GlobalKeyShortcutRegister(IntPtr hwnd)
        {
            this.hwnd = hwnd;
            globalHotKeys = new LinkedList<GlobalKeyShortcut>();
            CreateGlobalHotKeys();
        }

        private void CreateGlobalHotKeys()
        {
            CircleOfFifths.AllKeys
                .Select(key => new GlobalKeyShortcut(key, hwnd))
                .ForEach(globakHotkey => globalHotKeys.AddLast(globakHotkey));
        }

        public void RegisterGlobalHotkeys()
        {
            foreach (var globalHotkey in globalHotKeys)
            {
                if (globalHotkey.Register()) continue;

                var errorMsg = string.Format("Not registered global hotkey : id {0}. Error Code : {1}", globalHotkey.GetHashCode(), GetLastError());
                OnError(new UnhandledExceptionEventArgs(new Exception(errorMsg), false));
            }
        }

        public void UnregisterGlobalHotkeys()
        {
            foreach (var globalHotkey in globalHotKeys)
            {
                if (globalHotkey.Unregister()) continue;

                var errorMsg = string.Format("Not unregistered global hotkey : id {0}. Error Code : {1}", globalHotkey.GetHashCode(), GetLastError());
                OnError(new UnhandledExceptionEventArgs(new Exception(errorMsg), false));
            }
        }

        protected virtual void OnError(UnhandledExceptionEventArgs e)
        {
            if (Error != null)
                Error(this, e);
            else
                throw (Exception) e.ExceptionObject;
        }

        public event EventHandler<UnhandledExceptionEventArgs> Error;

        [DllImport("Kernel32.dll")]
        private static extern Int32 GetLastError();
    }
}
