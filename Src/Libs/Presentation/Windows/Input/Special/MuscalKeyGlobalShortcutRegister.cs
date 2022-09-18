using System;
using System.Collections.Generic;
using System.Linq;

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
            shortcuts.ForEach(s => s.Register());
        }

        public void Unregister()
        {
            shortcuts.ForEach(s => s.Unregister());
        }
    }
}