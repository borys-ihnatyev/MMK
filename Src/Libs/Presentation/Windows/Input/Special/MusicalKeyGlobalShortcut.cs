using System;
using System.Windows;
using System.Windows.Input;
using MMK.Marking;

namespace MMK.Presentation.Windows.Input.Special
{
    class MusicalKeyGlobalShortcut : GlobalShortcut
    {
        public MusicalKeyGlobalShortcut(Key key)
            : this(key, IntPtr.Zero)
        {
        }

        public MusicalKeyGlobalShortcut(Key key, IntPtr hwnd)
            : base(ModifyersFrom(key), KeyCodeFrom(key), hwnd)
        {
            Key = key;
        }

        public Key Key { get; private set; }

        private static ModifierKeys ModifyersFrom(Key key)
        {
            var modifyers = key.IsMoll() ? ModifierKeys.Control : ModifierKeys.Shift;

            if (key.IsSharpness())
                modifyers |= ModifierKeys.Alt;

            return modifyers;
        }

        private static int KeyCodeFrom(Key key)
        {
            return key.Note.ToString()[0];
        }

        public override int GetHashCode()
        {
            return (KeyCode << 4) | (int) Modifiers;
        }

        public static Key DecodeKey(int id)
        {
            var keyStr = string.Empty;
            keyStr += (char) (id >> 4);

            if ((id & (int) ModifierKeys.Alt) == (int) ModifierKeys.Alt)
                keyStr += '#';

            if ((id & (int) ModifierKeys.Control) == (int) ModifierKeys.Control)
                keyStr += 'm';

            return KeyHashTag.Parser.First("#" + keyStr).HashTag.Key;
        }
    }
}