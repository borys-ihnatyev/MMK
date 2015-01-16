using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MMK.Wpf.Providers.Key
{
    class MuscalKeyGlobalShortcutRegister
    {
        private readonly IntPtr hwnd;
        private readonly LinkedList<MusicalKeyGlobalShortcut> shortcuts;

        public MuscalKeyGlobalShortcutRegister(IntPtr hwnd)
        {
            this.hwnd = hwnd;
            shortcuts = new LinkedList<MusicalKeyGlobalShortcut>();
            CreateGlobalHotKeys();
        }

        private void CreateGlobalHotKeys()
        {
            CircleOfFifths.AllKeys
                .Select(key => new MusicalKeyGlobalShortcut(key, hwnd))
                .ForEach(globakHotkey => shortcuts.AddLast(globakHotkey));
        }

        public void RegisterGlobalHotkeys()
        {
            foreach (var globalHotkey in shortcuts)
            {
                if (globalHotkey.Register()) continue;

                var errorMsg = string.Format("Not registered global shortcut : id {0}. Error Code : {1}", globalHotkey.GetHashCode(), GetLastError());
                OnError(new UnhandledExceptionEventArgs(new Exception(errorMsg), false));
            }
        }

        public void UnregisterGlobalHotkeys()
        {
            foreach (var globalHotkey in shortcuts)
            {
                if (globalHotkey.Unregister()) continue;

                var errorMsg = string.Format("Not unregistered global shortcut : id {0}. Error Code : {1}", globalHotkey.GetHashCode(), GetLastError());
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
