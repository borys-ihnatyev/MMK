using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MMK.Presentation.Windows.Input.Special
{
    class MuscalKeyGlobalShortcutRegister
    {
        private readonly IntPtr hwnd;
        private readonly LinkedList<MusicalKeyGlobalShortcut> shortcuts;

        public MuscalKeyGlobalShortcutRegister(IntPtr hwnd)
        {
            this.hwnd = hwnd;
            shortcuts = new LinkedList<MusicalKeyGlobalShortcut>();
            CreateGlobalShortcuts();
        }

        private void CreateGlobalShortcuts()
        {
            CircleOfFifths.AllKeys
                .Select(key => new MusicalKeyGlobalShortcut(key, hwnd))
                .ForEach(shortcut => shortcuts.AddLast(shortcut));
        }

        public void Register()
        {
            foreach (var shortcut in shortcuts)
            {
                if (shortcut.Register()) continue;

                var errorMsg = string.Format("Not registered global shortcut : id {0}. Error Code : {1}", shortcut.GetHashCode(), GetLastError());
                OnError(new UnhandledExceptionEventArgs(new Exception(errorMsg), false));
            }
        }

        public void Unregister()
        {
            foreach (var shortcut in shortcuts)
            {
                if (shortcut.Unregister()) continue;

                var errorMsg = string.Format("Not unregistered global shortcut : id {0}. Error Code : {1}", shortcut.GetHashCode(), GetLastError());
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
